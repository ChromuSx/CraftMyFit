using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    public class WorkoutPlansViewModel : BaseViewModel
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private ObservableCollection<WorkoutPlan> _workoutPlans = [];
        private ObservableCollection<WorkoutPlan> _filteredWorkoutPlans = [];
        private string _searchText = string.Empty;
        private bool _showActiveOnly = false;
        private WorkoutPlan? _selectedWorkoutPlan;

        public WorkoutPlansViewModel(
            IWorkoutPlanRepository workoutPlanRepository,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Piani di Allenamento";

            // Inizializza i comandi
            LoadWorkoutPlansCommand = new Command(async () => await LoadWorkoutPlans());
            CreateWorkoutPlanCommand = new Command(async () => await CreateWorkoutPlan());
            SelectWorkoutPlanCommand = new Command<WorkoutPlan>(async (plan) => await SelectWorkoutPlan(plan));
            EditWorkoutPlanCommand = new Command<WorkoutPlan>(async (plan) => await EditWorkoutPlan(plan));
            DeleteWorkoutPlanCommand = new Command<WorkoutPlan>(async (plan) => await DeleteWorkoutPlan(plan));
            DuplicateWorkoutPlanCommand = new Command<WorkoutPlan>(async (plan) => await DuplicateWorkoutPlan(plan));
            StartWorkoutCommand = new Command<WorkoutPlan>(async (plan) => await StartWorkout(plan));
            SearchCommand = new Command<string>(async (searchText) => await SearchWorkoutPlans(searchText));
            ToggleActiveFilterCommand = new Command(async () => await ToggleActiveFilter());
            RefreshCommand = new Command(async () => await RefreshWorkoutPlans());

            // Carica i dati iniziali
            _ = Task.Run(LoadWorkoutPlans);
        }

        #region Properties

        public ObservableCollection<WorkoutPlan> FilteredWorkoutPlans
        {
            get => _filteredWorkoutPlans;
            set => SetProperty(ref _filteredWorkoutPlans, value);
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

        public bool ShowActiveOnly
        {
            get => _showActiveOnly;
            set
            {
                if(SetProperty(ref _showActiveOnly, value))
                {
                    ToggleActiveFilterCommand.Execute(null);
                }
            }
        }

        public WorkoutPlan? SelectedWorkoutPlan
        {
            get => _selectedWorkoutPlan;
            set => SetProperty(ref _selectedWorkoutPlan, value);
        }

        public bool HasWorkoutPlans => FilteredWorkoutPlans.Any();
        public string EmptyStateMessage => ShowActiveOnly ? "Nessun piano attivo trovato" : "Nessun piano di allenamento";

        #endregion

        #region Commands

        public ICommand LoadWorkoutPlansCommand { get; }
        public ICommand CreateWorkoutPlanCommand { get; }
        public ICommand SelectWorkoutPlanCommand { get; }
        public ICommand EditWorkoutPlanCommand { get; }
        public ICommand DeleteWorkoutPlanCommand { get; }
        public ICommand DuplicateWorkoutPlanCommand { get; }
        public ICommand StartWorkoutCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleActiveFilterCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Private Methods

        private async Task LoadWorkoutPlans()
        {
            if(IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Ottieni l'ID utente corrente (per ora hardcoded)
                var currentUserId = _preferenceService.GetInt("current_user_id", 1);

                var workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(currentUserId);
                _workoutPlans = new ObservableCollection<WorkoutPlan>(workoutPlans);

                await ApplyFilters();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento dei piani: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateWorkoutPlan()
        {
            try
            {
                await _navigationService.NavigateToAsync("createworkoutplan");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella navigazione: {ex.Message}");
            }
        }

        private async Task SelectWorkoutPlan(WorkoutPlan? workoutPlan)
        {
            if(workoutPlan == null)
            {
                return;
            }

            try
            {
                SelectedWorkoutPlan = workoutPlan;

                Dictionary<string, object> parameters = new()
                {
                    { "WorkoutPlan", workoutPlan }
                };

                await _navigationService.NavigateToAsync("workoutplandetail", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella navigazione: {ex.Message}");
            }
        }

        private async Task EditWorkoutPlan(WorkoutPlan? workoutPlan)
        {
            if(workoutPlan == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "WorkoutPlan", workoutPlan },
                    { "IsEdit", true }
                };

                await _navigationService.NavigateToAsync("editworkoutplan", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella navigazione: {ex.Message}");
            }
        }

        private async Task DeleteWorkoutPlan(WorkoutPlan? workoutPlan)
        {
            if(workoutPlan == null)
            {
                return;
            }

            try
            {
                var confirmed = await _dialogService.ShowConfirmAsync(
                    "Elimina Piano",
                    $"Sei sicuro di voler eliminare il piano '{workoutPlan.Title}'?",
                    "Elimina",
                    "Annulla");

                if(confirmed)
                {
                    await _workoutPlanRepository.DeleteAsync(workoutPlan.Id);
                    await LoadWorkoutPlans();
                    await _dialogService.ShowAlertAsync("Successo", "Piano eliminato con successo");
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'eliminazione del piano: {ex.Message}");
            }
        }

        private async Task DuplicateWorkoutPlan(WorkoutPlan? workoutPlan)
        {
            if(workoutPlan == null)
            {
                return;
            }

            try
            {
                // Carica il piano completo con i dettagli
                WorkoutPlan fullPlan = await _workoutPlanRepository.GetWorkoutPlanWithDetailsAsync(workoutPlan.Id);
                if(fullPlan == null)
                {
                    return;
                }

                // Crea una copia del piano
                WorkoutPlan duplicatedPlan = new()
                {
                    Title = $"Copia di {fullPlan.Title}",
                    Description = fullPlan.Description,
                    WorkoutDaysJson = fullPlan.WorkoutDaysJson,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    UserId = fullPlan.UserId,
                    User = new Models.User { Id = fullPlan.UserId, Name = "User" }, // Placeholder
                    WorkoutDays = new List<WorkoutDay>() // Inizializza lista vuota
                };

                int newPlanId = await _workoutPlanRepository.AddAsync(duplicatedPlan);

                await LoadWorkoutPlans();
                await _dialogService.ShowAlertAsync("Successo", "Piano duplicato con successo");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella duplicazione del piano: {ex.Message}");
            }
        }

        private async Task StartWorkout(WorkoutPlan? workoutPlan)
        {
            if(workoutPlan == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "WorkoutPlan", workoutPlan }
                };

                await _navigationService.NavigateToAsync("workoutexecution", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'avvio dell'allenamento: {ex.Message}");
            }
        }

        private async Task SearchWorkoutPlans(string searchText) => await ApplyFilters();

        private async Task ToggleActiveFilter() => await ApplyFilters();

        private async Task ApplyFilters()
        {
            try
            {
                IEnumerable<WorkoutPlan> filteredPlans = _workoutPlans.AsEnumerable();

                // Filtro per testo di ricerca
                if(!string.IsNullOrWhiteSpace(SearchText))
                {
                    string searchLower = SearchText.ToLower();
                    filteredPlans = filteredPlans.Where(p =>
                        p.Title.ToLower().Contains(searchLower) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchLower)));
                }

                // Filtro per piani attivi (modificati di recente)
                if(ShowActiveOnly)
                {
                    DateTime cutoffDate = DateTime.Now.AddDays(-30); // Considera attivi i piani modificati negli ultimi 30 giorni
                    filteredPlans = filteredPlans.Where(p => p.ModifiedDate >= cutoffDate);
                }

                FilteredWorkoutPlans = [.. filteredPlans.OrderByDescending(p => p.ModifiedDate)];
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'applicazione dei filtri: {ex.Message}");
            }
        }

        private async Task RefreshWorkoutPlans() => await LoadWorkoutPlans();

        #endregion

        #region Public Methods

        public async Task OnWorkoutPlanCreated() =>
            // Chiamato quando viene creato un nuovo piano
            await LoadWorkoutPlans();

        public async Task OnWorkoutPlanUpdated() =>
            // Chiamato quando viene aggiornato un piano
            await LoadWorkoutPlans();

        #endregion
    }
}