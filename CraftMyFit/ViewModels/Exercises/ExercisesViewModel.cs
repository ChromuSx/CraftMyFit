using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Controls;
#endif

namespace CraftMyFit.ViewModels.Exercises
{
#if ANDROID || IOS || MACCATALYST || WINDOWS
    public class ExercisesViewModel : BaseViewModel, IQueryAttributable
#else
    public class ExercisesViewModel : BaseViewModel
#endif
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Exercise> _exercises = [];
        private ObservableCollection<Exercise> _filteredExercises = [];
        private ObservableCollection<string> _muscleGroups = [];
        private string _searchText = string.Empty;
        private string _selectedMuscleGroup = "Tutti";
        private bool _showOnlyWithoutEquipment = false;

        public ExercisesViewModel(
            IExerciseRepository exerciseRepository,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _exerciseRepository = exerciseRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "Esercizi";

            // Comandi
            LoadExercisesCommand = new Command(async () => await LoadExercises());
            SearchCommand = new Command<string>(async (searchText) => await SearchExercises(searchText));
            FilterByMuscleGroupCommand = new Command<string>(async (muscleGroup) => await FilterByMuscleGroup(muscleGroup));
            ToggleEquipmentFilterCommand = new Command(async () => await ToggleEquipmentFilter());
            SelectExerciseCommand = new Command<Exercise>(async (exercise) => await SelectExercise(exercise));
            RefreshCommand = new Command(async () => await RefreshExercises());
            CreateExerciseCommand = new Command(async () => await CreateExercise());

            // Carica i dati inizialmente
            _ = Task.Run(LoadExercises);
        }

        #region Properties

        public ObservableCollection<Exercise> FilteredExercises
        {
            get => _filteredExercises;
            set => SetProperty(ref _filteredExercises, value);
        }

        public ObservableCollection<string> MuscleGroups
        {
            get => _muscleGroups;
            set => SetProperty(ref _muscleGroups, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if(SetProperty(ref _searchText, value))
                {
                    SearchCommand.Execute(value);
                }
            }
        }

        public string SelectedMuscleGroup
        {
            get => _selectedMuscleGroup;
            set
            {
                if(SetProperty(ref _selectedMuscleGroup, value))
                {
                    FilterByMuscleGroupCommand.Execute(value);
                }
            }
        }

        public bool ShowOnlyWithoutEquipment
        {
            get => _showOnlyWithoutEquipment;
            set
            {
                if(SetProperty(ref _showOnlyWithoutEquipment, value))
                {
                    ToggleEquipmentFilterCommand.Execute(null);
                }
            }
        }

        public bool IsSelectionMode { get; set; } = false;

        #endregion

        #region Commands

        public ICommand LoadExercisesCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand FilterByMuscleGroupCommand { get; }
        public ICommand ToggleEquipmentFilterCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CreateExerciseCommand { get; }

        #endregion

        #region Private Methods

        private async Task LoadExercises()
        {
            if(IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Carica tutti gli esercizi
                var exercises = await _exerciseRepository.GetAllAsync();
                _exercises = new ObservableCollection<Exercise>(exercises);

                // Carica i gruppi muscolari
                var muscleGroups = await _exerciseRepository.GetAllMuscleGroupsAsync();
                List<string> allGroups = new()
                { "Tutti" };
                allGroups.AddRange(muscleGroups);
                MuscleGroups = [.. allGroups];

                // Applica i filtri correnti
                await ApplyFilters();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento degli esercizi: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchExercises(string searchText) => await ApplyFilters();

        private async Task FilterByMuscleGroup(string muscleGroup) => await ApplyFilters();

        private async Task ToggleEquipmentFilter() => await ApplyFilters();

        private async Task ApplyFilters()
        {
            try
            {
                IEnumerable<Exercise> filteredExercises = _exercises.AsEnumerable();

                // Filtro per testo di ricerca
                if(!string.IsNullOrWhiteSpace(SearchText))
                {
                    string searchLower = SearchText.ToLower();
                    filteredExercises = filteredExercises.Where(e =>
                        e.Name.ToLower().Contains(searchLower) ||
                        e.Description.ToLower().Contains(searchLower) ||
                        e.MuscleGroup.ToLower().Contains(searchLower));
                }

                // Filtro per gruppo muscolare
                if(SelectedMuscleGroup != "Tutti")
                {
                    filteredExercises = filteredExercises.Where(e =>
                        e.MuscleGroup == SelectedMuscleGroup);
                }

                // Filtro per esercizi senza attrezzatura
                if(ShowOnlyWithoutEquipment)
                {
                    filteredExercises = filteredExercises.Where(e =>
                        e.RequiredEquipmentJson == "[]" || string.IsNullOrEmpty(e.RequiredEquipmentJson));
                }

                FilteredExercises = [.. filteredExercises.OrderBy(e => e.Name)];
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'applicazione dei filtri: {ex.Message}");
            }
        }

        private async Task SelectExercise(Exercise exercise)
        {
            if(exercise == null)
            {
                return;
            }

            if (IsSelectionMode)
            {
                // Torna indietro passando l'esercizio selezionato
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                {
                    { "Exercise", exercise }
                });
            }
            else
            {
                try
                {
                    Dictionary<string, object> parameters = new()
                    {
                        { "Exercise", exercise }
                    };
                    await _navigationService.NavigateToAsync("exercisedetail", parameters);
                }
                catch(Exception ex)
                {
                    await _dialogService.ShowAlertAsync("Errore", $"Errore nella navigazione: {ex.Message}");
                }
            }
        }

        private async Task RefreshExercises() => await LoadExercises();

        private async Task CreateExercise()
        {
            await _navigationService.NavigateToAsync("exerciseform", new Dictionary<string, object> { { "IsEdit", false } });
        }

        #endregion

        #region Navigation

#if ANDROID || IOS || MACCATALYST || WINDOWS
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("IsSelectionMode", out var isSelectionModeObj))
            {
                if (isSelectionModeObj is bool b)
                    IsSelectionMode = b;
                else if (isSelectionModeObj is string s && bool.TryParse(s, out var parsed))
                    IsSelectionMode = parsed;
            }
        }
#endif

        #endregion
    }
}