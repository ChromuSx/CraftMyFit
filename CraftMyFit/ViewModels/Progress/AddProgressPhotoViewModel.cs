using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Progress;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Progress
{
    public class AddProgressPhotoViewModel : BaseViewModel
    {
        private readonly IProgressPhotoRepository _progressPhotoRepository;
        private readonly IPhotoService _photoService;
        private readonly IAchievementRepository? _achievementRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private string _description = string.Empty;
        private DateTime _selectedDate = DateTime.Today;
        private string? _selectedPhotoPath;
        private byte[]? _photoPreview;
        private bool _isPhotoSelected;
        private bool _isSaving;

        public AddProgressPhotoViewModel(
            IProgressPhotoRepository progressPhotoRepository,
            IPhotoService photoService,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService,
            IAchievementRepository? achievementRepository = null)
        {
            _progressPhotoRepository = progressPhotoRepository;
            _photoService = photoService;
            _achievementRepository = achievementRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Aggiungi Foto";

            // Inizializza i comandi
            TakePhotoCommand = new Command(async () => await TakePhoto());
            PickPhotoCommand = new Command(async () => await PickPhoto());
            SavePhotoCommand = new Command(async () => await SavePhoto(), CanSavePhoto);
            CancelCommand = new Command(async () => await Cancel());
            RemovePhotoCommand = new Command(RemovePhoto);

            // Imposta la descrizione predefinita
            Description = $"Foto del {DateTime.Today:dd/MM/yyyy}";
        }

        #region Properties

        public string Description
        {
            get => _description;
            set
            {
                if(SetProperty(ref _description, value))
                {
                    ((Command)SavePhotoCommand).ChangeCanExecute();
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string? SelectedPhotoPath
        {
            get => _selectedPhotoPath;
            set => SetProperty(ref _selectedPhotoPath, value);
        }

        public byte[]? PhotoPreview
        {
            get => _photoPreview;
            set => SetProperty(ref _photoPreview, value);
        }

        public bool IsPhotoSelected
        {
            get => _isPhotoSelected;
            set
            {
                if(SetProperty(ref _isPhotoSelected, value))
                {
                    ((Command)SavePhotoCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        // Proprietà di supporto
        public bool CanRemovePhoto => IsPhotoSelected;
        public string SaveButtonText => IsSaving ? "Salvataggio..." : "Salva Foto";

        #endregion

        #region Commands

        public ICommand TakePhotoCommand { get; }
        public ICommand PickPhotoCommand { get; }
        public ICommand SavePhotoCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RemovePhotoCommand { get; }

        #endregion

        #region Private Methods

        private async Task TakePhoto()
        {
            try
            {
                var photoPath = await _photoService.TakePhotoAsync();
                if(string.IsNullOrEmpty(photoPath))
                {
                    return;
                }

                await SetSelectedPhoto(photoPath);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Impossibile scattare la foto: {ex.Message}");
            }
        }

        private async Task PickPhoto()
        {
            try
            {
                var photoPath = await _photoService.PickPhotoAsync();
                if(string.IsNullOrEmpty(photoPath))
                {
                    return;
                }

                await SetSelectedPhoto(photoPath);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Impossibile selezionare la foto: {ex.Message}");
            }
        }

        private async Task SetSelectedPhoto(string photoPath)
        {
            try
            {
                SelectedPhotoPath = photoPath;

                // Carica l'anteprima della foto
                using var photoStream = await _photoService.LoadPhotoAsync(photoPath);
                if(photoStream != null)
                {
                    using MemoryStream memoryStream = new();
                    await photoStream.CopyToAsync(memoryStream);
                    PhotoPreview = memoryStream.ToArray();
                }

                IsPhotoSelected = true;
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento della foto: {ex.Message}");
            }
        }

        private bool CanSavePhoto() => IsPhotoSelected &&
                   !string.IsNullOrWhiteSpace(Description) &&
                   !IsSaving;

        private async Task SavePhoto()
        {
            if(!CanSavePhoto() || string.IsNullOrEmpty(SelectedPhotoPath))
            {
                return;
            }

            try
            {
                IsSaving = true;

                // Verifica se esiste già una foto per la data selezionata
                var currentUserId = _preferenceService.GetInt("current_user_id", 1);
                var hasExistingPhoto = await _progressPhotoRepository.HasPhotosForDateAsync(currentUserId, SelectedDate);

                if(hasExistingPhoto)
                {
                    var overwrite = await _dialogService.ShowConfirmAsync(
                        "Foto Esistente",
                        "Esiste già una foto per questa data. Vuoi aggiungerne un'altra?",
                        "Aggiungi",
                        "Annulla");

                    if(!overwrite)
                    {
                        return;
                    }
                }

                // Crea l'oggetto ProgressPhoto
                ProgressPhoto progressPhoto = new()
                {
                    PhotoPath = SelectedPhotoPath,
                    Date = SelectedDate,
                    Description = Description.Trim(),
                    UserId = currentUserId,
                    User = new Models.User { Id = currentUserId, Name = "User" } // Placeholder
                };

                // Salva nel database
                await _progressPhotoRepository.AddAsync(progressPhoto);

                // Controlla e sblocca achievement per le foto
                await CheckPhotoAchievements(currentUserId);

                await _dialogService.ShowAlertAsync("Successo", "Foto di progresso salvata con successo!");

                // Torna alla schermata precedente
                await _navigationService.GoBackAsync();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio della foto: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task CheckPhotoAchievements(int userId)
        {
            if(_achievementRepository == null)
            {
                return;
            }

            try
            {
                var photoCount = await _progressPhotoRepository.GetPhotoCountByUserIdAsync(userId);
                var unlocked = await _achievementRepository.CheckAndUnlockAchievementsAsync(
                    userId,
                    Models.Gamification.AchievementType.PhotosUploaded,
                    photoCount);

                if(unlocked)
                {
                    // Mostra una notifica per i nuovi achievement sbloccati
                    var recentAchievements = await _achievementRepository.GetRecentlyUnlockedAsync(userId, 1);
                    if(recentAchievements.Any())
                    {
                        var achievement = recentAchievements.First();
                        await _dialogService.ShowAlertAsync(
                            "🏆 Achievement Sbloccato!",
                            $"{achievement.Title}\n{achievement.Description}");
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo degli achievement: {ex.Message}");
            }
        }

        private async Task Cancel()
        {
            if(IsPhotoSelected || !string.IsNullOrWhiteSpace(Description))
            {
                var confirmCancel = await _dialogService.ShowConfirmAsync(
                    "Annulla",
                    "Sei sicuro di voler annullare? Le modifiche andranno perse.",
                    "Annulla Modifiche",
                    "Continua");

                if(!confirmCancel)
                {
                    return;
                }
            }

            // Pulisci la foto temporanea se esiste
            if(!string.IsNullOrEmpty(SelectedPhotoPath) && _photoService.PhotoExists(SelectedPhotoPath))
            {
                // Solo se è una foto temporanea (scattata ora)
                // Non eliminare foto selezionate dalla galleria
            }

            await _navigationService.GoBackAsync();
        }

        private void RemovePhoto()
        {
            SelectedPhotoPath = null;
            PhotoPreview = null;
            IsPhotoSelected = false;
        }

        #endregion

        #region Public Methods

        public void OnAppearing()
        {
            // Reset dello stato quando la pagina appare
            if(!IsPhotoSelected)
            {
                Description = $"Foto del {DateTime.Today:dd/MM/yyyy}";
                SelectedDate = DateTime.Today;
            }
        }

        public void OnDisappearing()
        {
            // Cleanup quando la pagina scompare
            if(IsSaving)
            {
                return; // Non fare cleanup se stiamo salvando
            }

            // Pulisci le risorse se necessario
        }

        #endregion
    }
}