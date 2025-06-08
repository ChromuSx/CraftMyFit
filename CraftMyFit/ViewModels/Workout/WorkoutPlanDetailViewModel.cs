using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    [QueryProperty(nameof(WorkoutPlan), "WorkoutPlan")]
    [QueryProperty(nameof(RefreshRequest), "Refresh")]
    public class WorkoutPlanDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private WorkoutPlan? _workoutPlan;
        private ObservableCollection<WorkoutDay> _workoutDays = [];
        private WorkoutDay? _selectedWorkoutDay;
        private bool _isLoading;
        private bool _refreshRequest;

        public WorkoutPlanDetailViewModel(
            IWorkoutPlanRepository workoutPlanRepository,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Piano di Allenamento";

            // Inizializza i comandi
            LoadDetailsCommand = new Command(async () => await LoadDetails());
            StartWorkoutCommand = new Command(async () => await StartWorkout());
            EditPlanCommand = new Command(async () => await EditPlan());
            DuplicatePlanCommand = new Command(async () => await DuplicatePlan());
            DeletePlanCommand = new Command(async () => await DeletePlan());
            SelectWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await SelectWorkoutDay(day));
            SharePlanCommand = new Command(async () => await SharePlan());
            ViewExerciseCommand = new Command<WorkoutExercise>(async (exercise) => await ViewExercise(exercise));
            GoBackCommand = new Command(async () => await GoBack());

            SubscribeToWorkoutDaysCollectionChanged();
        }

        #region Properties

        public WorkoutPlan? WorkoutPlan
        {
            get => _workoutPlan;
            set
            {
                if (SetProperty(ref _workoutPlan, value))
                {
                    // Clear existing workout days
                    WorkoutDays.Clear();

                    // Add new workout days if available
                    if (value?.WorkoutDays != null)
                    {
                        foreach (var day in value.WorkoutDays.OrderBy(d => d.OrderIndex))
                        {
                            WorkoutDays.Add(day);
                        }
                    }

                    OnPropertyChanged(nameof(PlanTitle));
                    OnPropertyChanged(nameof(PlanDescription));
                    OnPropertyChanged(nameof(HasDescription));
                    OnPropertyChanged(nameof(CreatedDateText));
                    OnPropertyChanged(nameof(ModifiedDateText));
                    OnPropertyChanged(nameof(HasWorkoutDays));
                    OnPropertyChanged(nameof(WorkoutDaysCount));
                    OnPropertyChanged(nameof(WorkoutDaysCountText));
                    OnPropertyChanged(nameof(TotalExercisesCount));
                    OnPropertyChanged(nameof(TotalExercisesText));
                    OnPropertyChanged(nameof(WorkoutDaysListText));

                    System.Diagnostics.Debug.WriteLine($"WorkoutPlan updated: {value?.Title} with {WorkoutDays.Count} days");
                    foreach (var day in WorkoutDays)
                    {
                        System.Diagnostics.Debug.WriteLine($"Day {day.DayOfWeek}: {day.Exercises?.Count ?? 0} exercises");
                    }
                }
            }
        }

        public bool RefreshRequest
        {
            get => _refreshRequest;
            set
            {
                if (SetProperty(ref _refreshRequest, value) && value)
                {
                    _ = LoadDetails();
                }
            }
        }

        public ObservableCollection<WorkoutDay> WorkoutDays
        {
            get => _workoutDays;
            private set
            {
                if (_workoutDays != null)
                    _workoutDays.CollectionChanged -= WorkoutDays_CollectionChanged;
                if (SetProperty(ref _workoutDays, value))
                {
                    if (_workoutDays != null)
                        _workoutDays.CollectionChanged += WorkoutDays_CollectionChanged;
                    OnPropertyChanged(nameof(HasWorkoutDays));
                    OnPropertyChanged(nameof(WorkoutDaysCount));
                    OnPropertyChanged(nameof(WorkoutDaysCountText));
                    OnPropertyChanged(nameof(TotalExercisesCount));
                    OnPropertyChanged(nameof(TotalExercisesText));
                    OnPropertyChanged(nameof(WorkoutDaysListText));
                }
            }
        }

        public WorkoutDay? SelectedWorkoutDay
        {
            get => _selectedWorkoutDay;
            set => SetProperty(ref _selectedWorkoutDay, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Proprietà calcolate
        public string PlanTitle => WorkoutPlan?.Title ?? "Piano di Allenamento";
        public string PlanDescription => WorkoutPlan?.Description ?? "Nessuna descrizione";
        public bool HasDescription => !string.IsNullOrWhiteSpace(WorkoutPlan?.Description);
        public string CreatedDateText => WorkoutPlan?.CreatedDate.ToString("dd/MM/yyyy") ?? "";
        public string ModifiedDateText => WorkoutPlan?.ModifiedDate.ToString("dd/MM/yyyy") ?? "";
        public bool HasWorkoutDays => WorkoutDays.Any();
        public int WorkoutDaysCount => WorkoutDays.Count;
        public string WorkoutDaysCountText => WorkoutDaysCount == 1 ? "1 giorno" : $"{WorkoutDaysCount} giorni";

        public int TotalExercisesCount => WorkoutDays.Sum(wd => wd.Exercises?.Count ?? 0);
        public string TotalExercisesText => TotalExercisesCount == 1 ? "1 esercizio" : $"{TotalExercisesCount} esercizi";

        public string WorkoutDaysListText => WorkoutDays.Any() ?
            string.Join(", ", WorkoutDays.Select(wd => GetDayName(wd.DayOfWeek))) :
            "Nessun giorno selezionato";

        #endregion

        #region Commands

        public ICommand LoadDetailsCommand { get; }
        public ICommand StartWorkoutCommand { get; }
        public ICommand EditPlanCommand { get; }
        public ICommand DuplicatePlanCommand { get; }
        public ICommand DeletePlanCommand { get; }
        public ICommand SelectWorkoutDayCommand { get; }
        public ICommand SharePlanCommand { get; }
        public ICommand ViewExerciseCommand { get; }
        public ICommand GoBackCommand { get; }

        #endregion

        #region Private Methods

        private async void OnWorkoutPlanChanged()
        {
            if (WorkoutPlan != null)
            {
                Title = WorkoutPlan.Title;
                await LoadDetails();
            }
        }

        private async Task LoadDetails()
        {
            if (WorkoutPlan == null || IsLoading)
            {
                return;
            }

            try
            {
                IsLoading = true;

                // Carica i dettagli completi del piano di allenamento
                WorkoutPlan fullPlan = await _workoutPlanRepository.GetWorkoutPlanWithDetailsAsync(WorkoutPlan.Id);
                if (fullPlan != null)
                {
                    WorkoutPlan = fullPlan; // Update the entire plan reference

                    System.Diagnostics.Debug.WriteLine($"Loaded plan: {fullPlan.Title} with {WorkoutDays.Count} days and {TotalExercisesCount} exercises");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load workout plan details");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento dei dettagli: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                RefreshRequest = false; // Reset refresh flag
            }
        }

        private async Task StartWorkout()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                if (!HasWorkoutDays)
                {
                    await _dialogService.ShowAlertAsync("Piano Vuoto", "Questo piano non contiene giorni di allenamento.");
                    return;
                }

                // Se c'è più di un giorno, chiedi quale scegliere
                WorkoutDay? selectedDay = null;

                if (WorkoutDays.Count == 1)
                {
                    selectedDay = WorkoutDays.First();
                }
                else
                {
                    string[] dayNames = WorkoutDays.Select(wd => $"{GetDayName(wd.DayOfWeek)} - {wd.Title}").ToArray();
                    string? selectedDayName = await _dialogService.ShowActionSheetAsync(
                        "Seleziona Allenamento",
                        "Annulla",
                        null,
                        dayNames);

                    if (selectedDayName is not null and not "Annulla")
                    {
                        int index = Array.IndexOf(dayNames, selectedDayName);
                        if (index >= 0 && index < WorkoutDays.Count)
                        {
                            selectedDay = WorkoutDays[index];
                        }
                    }
                }

                if (selectedDay != null)
                {
                    Dictionary<string, object> parameters = new()
                    {
                        { "WorkoutPlan", WorkoutPlan },
                        { "WorkoutDay", selectedDay }
                    };

                    await _navigationService.NavigateToAsync("workoutexecution", parameters);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'avvio dell'allenamento: {ex.Message}");
            }
        }

        private async Task EditPlan()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "WorkoutPlan", WorkoutPlan },
                    { "IsEdit", true }
                };

                await _navigationService.NavigateToAsync("editworkoutplan", parameters);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'apertura dell'editor: {ex.Message}");
            }
        }

        private async Task DuplicatePlan()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Duplica Piano",
                    $"Vuoi creare una copia di '{WorkoutPlan.Title}'?",
                    "Duplica",
                    "Annulla");

                if (confirmed)
                {
                    // Crea una copia del piano
                    WorkoutPlan duplicatedPlan = new()
                    {
                        Title = $"Copia di {WorkoutPlan.Title}",
                        Description = WorkoutPlan.Description,
                        WorkoutDaysJson = WorkoutPlan.WorkoutDaysJson,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        UserId = WorkoutPlan.UserId,
                        WorkoutDays = [] // Inizializza lista vuota
                    };

                    _ = await _workoutPlanRepository.AddAsync(duplicatedPlan);
                    await _dialogService.ShowAlertAsync("Successo", "Piano duplicato con successo!");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella duplicazione: {ex.Message}");
            }
        }

        private async Task DeletePlan()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Elimina Piano",
                    $"Sei sicuro di voler eliminare '{WorkoutPlan.Title}'? Questa azione non può essere annullata.",
                    "Elimina",
                    "Annulla");

                if (confirmed)
                {
                    await _workoutPlanRepository.DeleteAsync(WorkoutPlan.Id);
                    await _dialogService.ShowAlertAsync("Eliminato", "Piano eliminato con successo");
                    await _navigationService.GoBackAsync();
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'eliminazione: {ex.Message}");
            }
        }

        private async Task SelectWorkoutDay(WorkoutDay? workoutDay)
        {
            if (workoutDay == null)
            {
                return;
            }

            SelectedWorkoutDay = workoutDay;

            // Mostra dettagli del giorno di allenamento
            int exerciseCount = workoutDay.Exercises?.Count ?? 0;
            string message = exerciseCount == 0 ?
                "Questo giorno non contiene esercizi." :
                $"Questo giorno contiene {exerciseCount} esercizio/i.";

            await _dialogService.ShowAlertAsync(workoutDay.Title, message);
        }

        private async Task SharePlan()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                string shareText = $"Piano di Allenamento: {WorkoutPlan.Title}\n";

                if (HasDescription)
                {
                    shareText += $"Descrizione: {WorkoutPlan.Description}\n";
                }

                shareText += $"Giorni: {WorkoutDaysListText}\n";
                shareText += $"Esercizi totali: {TotalExercisesCount}\n\n";
                shareText += "Condiviso da CraftMyFit";

                // TODO: Implementare la condivisione nativa
                await _dialogService.ShowAlertAsync("Condivisione", shareText);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella condivisione: {ex.Message}");
            }
        }

        private async Task ViewExercise(WorkoutExercise? workoutExercise)
        {
            if (workoutExercise?.Exercise == null)
            {
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "Exercise", workoutExercise.Exercise }
                };

                await _navigationService.NavigateToAsync("exercisedetail", parameters);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella visualizzazione dell'esercizio: {ex.Message}");
            }
        }

        private async Task GoBack() => await _navigationService.GoBackAsync();

        private string GetDayName(DayOfWeek dayOfWeek) => dayOfWeek switch
        {
            DayOfWeek.Monday => "Lun",
            DayOfWeek.Tuesday => "Mar",
            DayOfWeek.Wednesday => "Mer",
            DayOfWeek.Thursday => "Gio",
            DayOfWeek.Friday => "Ven",
            DayOfWeek.Saturday => "Sab",
            DayOfWeek.Sunday => "Dom",
            _ => dayOfWeek.ToString()
        };

        private void SubscribeToWorkoutDaysCollectionChanged()
        {
            if (_workoutDays != null)
            {
                _workoutDays.CollectionChanged -= WorkoutDays_CollectionChanged;
                _workoutDays.CollectionChanged += WorkoutDays_CollectionChanged;
            }
        }

        private void WorkoutDays_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasWorkoutDays));
            OnPropertyChanged(nameof(WorkoutDaysCount));
            OnPropertyChanged(nameof(WorkoutDaysCountText));
            OnPropertyChanged(nameof(TotalExercisesCount));
            OnPropertyChanged(nameof(TotalExercisesText));
            OnPropertyChanged(nameof(WorkoutDaysListText));
        }

        #endregion

        #region Public Methods

        public async Task OnWorkoutPlanUpdated() =>
            // Chiamato quando il piano viene aggiornato
            await LoadDetails();

        public void OnAppearing()
        {
            // Call LoadDetails whenever the page appears to ensure data is up to date
            if (WorkoutPlan != null)
            {
                _ = LoadDetails();
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("WorkoutPlan"))
            {
                var plan = query["WorkoutPlan"] as WorkoutPlan;
                WorkoutPlan = plan;
            }

            if (query.ContainsKey("Refresh"))
            {
                if (query["Refresh"] is bool shouldRefresh && shouldRefresh)
                {
                    RefreshRequest = true;
                }
            }
        }

        #endregion
    }
}