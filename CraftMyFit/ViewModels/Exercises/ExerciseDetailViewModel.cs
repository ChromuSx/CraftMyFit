using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Exercises
{
    [QueryProperty(nameof(Exercise), "Exercise")]
    public class ExerciseDetailViewModel : BaseViewModel
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private Exercise? _exercise;
        private ObservableCollection<string> _requiredEquipment = [];
        private bool _isFavorite;
        private string _notes = string.Empty;

        public ExerciseDetailViewModel(
            IExerciseRepository exerciseRepository,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _exerciseRepository = exerciseRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            // Inizializza i comandi
            EditExerciseCommand = new Command(async () => await EditExercise());
            ToggleFavoriteCommand = new Command(async () => await ToggleFavorite());
            AddToWorkoutCommand = new Command(async () => await AddToWorkout());
            ViewVideoCommand = new Command(async () => await ViewVideo());
            ShareExerciseCommand = new Command(async () => await ShareExercise());
            GoBackCommand = new Command(async () => await GoBack());
        }

        #region Properties

        public Exercise? Exercise
        {
            get => _exercise;
            set
            {
                if(SetProperty(ref _exercise, value))
                {
                    OnExerciseChanged();
                }
            }
        }

        public ObservableCollection<string> RequiredEquipment
        {
            get => _requiredEquipment;
            set => SetProperty(ref _requiredEquipment, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // Proprietà calcolate
        public string ExerciseName => Exercise?.Name ?? "Esercizio";
        public string ExerciseDescription => Exercise?.Description ?? "Nessuna descrizione disponibile";
        public string MuscleGroup => Exercise?.MuscleGroup ?? "Non specificato";
        public bool HasImage => !string.IsNullOrEmpty(Exercise?.ImagePath);
        public bool HasVideo => !string.IsNullOrEmpty(Exercise?.VideoPath);
        public bool RequiresEquipment => RequiredEquipment.Any();
        public string EquipmentSummary => RequiresEquipment ? string.Join(", ", RequiredEquipment) : "Nessuna attrezzatura richiesta";

        #endregion

        #region Commands

        public ICommand EditExerciseCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand AddToWorkoutCommand { get; }
        public ICommand ViewVideoCommand { get; }
        public ICommand ShareExerciseCommand { get; }
        public ICommand GoBackCommand { get; }

        #endregion

        #region Private Methods

        private void OnExerciseChanged()
        {
            if(Exercise == null)
            {
                return;
            }

            Title = Exercise.Name;

            // Carica l'attrezzatura richiesta
            RequiredEquipment = [.. Exercise.RequiredEquipment];

            // TODO: Caricare se è nei preferiti e le note personali
            LoadPersonalData();
        }

        private void LoadPersonalData()
        {
            if(Exercise == null)
            {
                return;
            }

            // TODO: Implementare il caricamento dei dati personali dall'utente
            // Per ora utilizziamo valori di default
            IsFavorite = false;
            Notes = string.Empty;
        }

        private async Task EditExercise()
        {
            if(Exercise == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "Exercise", Exercise },
                    { "IsEdit", true }
                };

                await _navigationService.NavigateToAsync("exerciseform", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'apertura dell'editor: {ex.Message}");
            }
        }

        private async Task ToggleFavorite()
        {
            try
            {
                IsFavorite = !IsFavorite;

                // TODO: Salvare lo stato nei preferiti dell'utente

                string message = IsFavorite ? "Aggiunto ai preferiti" : "Rimosso dai preferiti";
                await _dialogService.ShowAlertAsync("Preferiti", message);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'aggiornamento dei preferiti: {ex.Message}");
            }
        }

        private async Task AddToWorkout()
        {
            if(Exercise == null)
            {
                return;
            }

            try
            {
                // TODO: Implementare l'aggiunta dell'esercizio a un piano di allenamento
                string[] options = new string[] { "Nuovo Piano", "Piano Esistente" };

                var choice = await _dialogService.ShowActionSheetAsync(
                    "Aggiungi a Piano",
                    "Annulla",
                    null,
                    options);

                switch(choice)
                {
                    case "Nuovo Piano":
                        Dictionary<string, object> parameters = new()
                        {
                            { "PreselectedExercise", Exercise }
                        };
                        await _navigationService.NavigateToAsync("createworkoutplan", parameters);
                        break;

                    case "Piano Esistente":
                        await _dialogService.ShowAlertAsync("In Arrivo", "Selezione piano esistente disponibile presto!");
                        break;
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'aggiunta al piano: {ex.Message}");
            }
        }

        private async Task ViewVideo()
        {
            if(Exercise == null || string.IsNullOrEmpty(Exercise.VideoPath))
            {
                await _dialogService.ShowAlertAsync("Video Non Disponibile", "Nessun video disponibile per questo esercizio");
                return;
            }

            try
            {
                // TODO: Implementare la riproduzione video
                await _dialogService.ShowAlertAsync("Video", $"Riproduzione video: {Exercise.VideoPath}");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella riproduzione del video: {ex.Message}");
            }
        }

        private async Task ShareExercise()
        {
            if(Exercise == null)
            {
                return;
            }

            try
            {
                string shareText = $"Esercizio: {Exercise.Name}\n" +
                               $"Gruppo Muscolare: {Exercise.MuscleGroup}\n" +
                               $"Descrizione: {Exercise.Description}\n\n" +
                               $"Condiviso da CraftMyFit";

                // TODO: Implementare la condivisione nativa
                await _dialogService.ShowAlertAsync("Condivisione", shareText);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella condivisione: {ex.Message}");
            }
        }

        private async Task GoBack() => await _navigationService.GoBackAsync();

        #endregion

        #region Public Methods

        public async Task SaveNotes()
        {
            if(Exercise == null)
            {
                return;
            }

            try
            {
                // TODO: Salvare le note personali per l'esercizio
                await _dialogService.ShowAlertAsync("Successo", "Note salvate");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio delle note: {ex.Message}");
            }
        }

        #endregion
    }
}