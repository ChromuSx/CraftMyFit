using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Progress;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Progress
{
    public class ProgressPhotosViewModel : BaseViewModel
    {
        private readonly IProgressPhotoRepository _progressPhotoRepository;
        private readonly IPhotoService _photoService;
        private readonly IAchievementRepository? _achievementRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private ObservableCollection<ProgressPhoto> _progressPhotos = [];
        private ObservableCollection<ProgressPhoto> _filteredPhotos = [];
        private ProgressPhoto? _selectedPhoto;
        private DateTime _selectedDate = DateTime.Today;
        private bool _isGroupedByMonth = true;
        private string _searchDescription = string.Empty;

        public ProgressPhotosViewModel(
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

            Title = "Foto di Progresso";

            // Inizializza i comandi
            LoadPhotosCommand = new Command(async () => await LoadPhotos());
            TakePhotoCommand = new Command(async () => await TakePhoto());
            PickPhotoCommand = new Command(async () => await PickPhoto());
            SelectPhotoCommand = new Command<ProgressPhoto>(async (photo) => await SelectPhoto(photo));
            DeletePhotoCommand = new Command<ProgressPhoto>(async (photo) => await DeletePhoto(photo));
            ViewPhotoCommand = new Command<ProgressPhoto>(async (photo) => await ViewPhoto(photo));
            ComparePhotosCommand = new Command(async () => await ComparePhotos());
            FilterByDateCommand = new Command<DateTime>(async (date) => await FilterByDate(date));
            ToggleGroupingCommand = new Command(async () => await ToggleGrouping());
            SearchCommand = new Command<string>(async (searchText) => await SearchPhotos(searchText));
            RefreshCommand = new Command(async () => await RefreshPhotos());

            // Carica i dati iniziali
            _ = Task.Run(LoadPhotos);
        }

        #region Properties

        public ObservableCollection<ProgressPhoto> FilteredPhotos
        {
            get => _filteredPhotos;
            set => SetProperty(ref _filteredPhotos, value);
        }

        public ProgressPhoto? SelectedPhoto
        {
            get => _selectedPhoto;
            set => SetProperty(ref _selectedPhoto, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if(SetProperty(ref _selectedDate, value))
                {
                    FilterByDateCommand.Execute(value);
                }
            }
        }

        public bool IsGroupedByMonth
        {
            get => _isGroupedByMonth;
            set
            {
                if(SetProperty(ref _isGroupedByMonth, value))
                {
                    ToggleGroupingCommand.Execute(null);
                }
            }
        }

        public string SearchDescription
        {
            get => _searchDescription;
            set
            {
                if(SetProperty(ref _searchDescription, value))
                {
                    SearchCommand.Execute(value);
                }
            }
        }

        public bool HasPhotos => FilteredPhotos.Any();
        public int PhotoCount => FilteredPhotos.Count;
        public string EmptyStateMessage => "Nessuna foto di progresso trovata";
        public string PhotoCountText => PhotoCount == 1 ? "1 foto" : $"{PhotoCount} foto";

        #endregion

        #region Commands

        public ICommand LoadPhotosCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand PickPhotoCommand { get; }
        public ICommand SelectPhotoCommand { get; }
        public ICommand DeletePhotoCommand { get; }
        public ICommand ViewPhotoCommand { get; }
        public ICommand ComparePhotosCommand { get; }
        public ICommand FilterByDateCommand { get; }
        public ICommand ToggleGroupingCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Private Methods

        private async Task LoadPhotos()
        {
            if(IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Ottieni l'ID utente corrente
                var currentUserId = _preferenceService.GetInt("current_user_id", 1);

                var photos = await _progressPhotoRepository.GetByUserIdAsync(currentUserId);
                _progressPhotos = new ObservableCollection<ProgressPhoto>(photos);

                await ApplyFilters();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento delle foto: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task TakePhoto()
        {
            try
            {
                var photoPath = await _photoService.TakePhotoAsync();
                if(string.IsNullOrEmpty(photoPath))
                {
                    await _dialogService.ShowAlertAsync("Errore", "Impossibile scattare la foto");
                    return;
                }

                await SaveProgressPhoto(photoPath);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nello scattare la foto: {ex.Message}");
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

                await SaveProgressPhoto(photoPath);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella selezione della foto: {ex.Message}");
            }
        }

        private async Task SaveProgressPhoto(string photoPath)
        {
            try
            {
                // Chiedi una descrizione per la foto
                var description = await _dialogService.ShowPromptAsync(
                    "Descrizione Foto",
                    "Aggiungi una descrizione per questa foto di progresso:",
                    "Es. Dopo 2 settimane di allenamento");

                if(string.IsNullOrWhiteSpace(description))
                {
                    description = $"Foto del {DateTime.Today:dd/MM/yyyy}";
                }

                // Crea l'oggetto ProgressPhoto
                var currentUserId = _preferenceService.GetInt("current_user_id", 1);
                ProgressPhoto progressPhoto = new()
                {
                    PhotoPath = photoPath,
                    Date = DateTime.Now,
                    Description = description,
                    UserId = currentUserId,
                    User = new Models.User { Id = currentUserId, Name = "User" } // Placeholder
                };

                // Salva nel database
                await _progressPhotoRepository.AddAsync(progressPhoto);

                // Controlla e sblocca achievement
                await CheckPhotoAchievements(currentUserId);

                // Ricarica le foto
                await LoadPhotos();

                await _dialogService.ShowAlertAsync("Successo", "Foto di progresso salvata!");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio della foto: {ex.Message}");
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
                await _achievementRepository.CheckAndUnlockAchievementsAsync(
                    userId,
                    Models.Gamification.AchievementType.PhotosUploaded,
                    photoCount);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo degli achievement: {ex.Message}");
            }
        }

        private async Task SelectPhoto(ProgressPhoto? photo)
        {
            if(photo == null)
            {
                return;
            }

            SelectedPhoto = photo;
            await ViewPhoto(photo);
        }

        private async Task DeletePhoto(ProgressPhoto? photo)
        {
            if(photo == null)
            {
                return;
            }

            try
            {
                var confirmed = await _dialogService.ShowConfirmAsync(
                    "Elimina Foto",
                    "Sei sicuro di voler eliminare questa foto di progresso?",
                    "Elimina",
                    "Annulla");

                if(confirmed)
                {
                    // Elimina il file fisico
                    await _photoService.DeletePhotoAsync(photo.PhotoPath);

                    // Elimina dal database
                    await _progressPhotoRepository.DeleteAsync(photo.Id);

                    // Ricarica le foto
                    await LoadPhotos();

                    await _dialogService.ShowAlertAsync("Successo", "Foto eliminata con successo");
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'eliminazione della foto: {ex.Message}");
            }
        }

        private async Task ViewPhoto(ProgressPhoto? photo)
        {
            if(photo == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "Photo", photo }
                };

                await _navigationService.NavigateToAsync("photogallery", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella visualizzazione della foto: {ex.Message}");
            }
        }

        private async Task ComparePhotos()
        {
            if(FilteredPhotos.Count < 2)
            {
                await _dialogService.ShowAlertAsync("Confronto Non Possibile", "Servono almeno 2 foto per il confronto");
                return;
            }

            // TODO: Implementare la funzionalità di confronto foto
            await _dialogService.ShowAlertAsync("In Arrivo", "La funzionalità di confronto sarà disponibile presto!");
        }

        private async Task FilterByDate(DateTime date) => await ApplyFilters();

        private async Task ToggleGrouping() => await ApplyFilters();

        private async Task SearchPhotos(string searchText) => await ApplyFilters();

        private async Task ApplyFilters()
        {
            try
            {
                IEnumerable<ProgressPhoto> filteredPhotos = _progressPhotos.AsEnumerable();

                // Filtro per descrizione
                if(!string.IsNullOrWhiteSpace(SearchDescription))
                {
                    string searchLower = SearchDescription.ToLower();
                    filteredPhotos = filteredPhotos.Where(p =>
                        p.Description.ToLower().Contains(searchLower));
                }

                // Filtro per data (se non è la data di oggi, mostra solo le foto di quel giorno)
                if(SelectedDate.Date != DateTime.Today)
                {
                    filteredPhotos = filteredPhotos.Where(p => p.Date.Date == SelectedDate.Date);
                }

                // Ordina per data (più recenti prima)
                filteredPhotos = filteredPhotos.OrderByDescending(p => p.Date);

                FilteredPhotos = [.. filteredPhotos];
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'applicazione dei filtri: {ex.Message}");
            }
        }

        private async Task RefreshPhotos() => await LoadPhotos();

        #endregion

        #region Public Methods

        public async Task OnPhotoAdded() =>
            // Chiamato quando viene aggiunta una nuova foto
            await LoadPhotos();

        #endregion
    }
}