using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    [QueryProperty(nameof(WorkoutPlan), "WorkoutPlan")]
    public class WorkoutPlanDetailViewModel : BaseViewModel
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private WorkoutPlan? _workoutPlan;
        private ObservableCollection<WorkoutDay> _workoutDays = [];
        private WorkoutDay? _selectedWorkoutDay;
        private bool _isLoading;

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
        }

        #region Properties

        public WorkoutPlan? WorkoutPlan
        {
            get => _workoutPlan;
            set
            {
                if(SetProperty(ref _workoutPlan, value))
                {
                    OnWorkoutPlanChanged();
                }
            }
        }

        public ObservableCollection<WorkoutDay> WorkoutDays
        {
            get => _workoutDays;
            set => SetProperty(ref _workoutDays, value);
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
            if(WorkoutPlan == null)
            {
                return;
            }

            Title = WorkoutPlan.Title;
            await LoadDetails();
        }

        private async Task LoadDetails()
        {
            if(WorkoutPlan == null || IsLoading)
            {
                return;
            }

            try
            {
                IsLoading = true;

                // Carica i dettagli completi del piano di allenamento
                WorkoutPlan fullPlan = await _workoutPlanRepository.GetWorkoutPlanWithDetailsAsync(WorkoutPlan.Id);
                if(fullPlan != null)
                {
                    WorkoutPlan = fullPlan;
                    // Correzione dell'operatore ??
                    WorkoutDays = fullPlan.WorkoutDays != null ? [.. fullPlan.WorkoutDays.OrderBy(wd => wd.OrderIndex)] : [];
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento dei dettagli: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private async Task StartWorkout()
        {
            if(WorkoutPlan == null)
            {
                return;
            }

            try
            {
                if(!HasWorkoutDays)
                {
                    await _dialogService.ShowAlertAsync("Piano Vuoto", "Questo piano non contiene giorni di allenamento.");
                    return;
                }

                // Se c'è più di un giorno, chiedi quale scegliere
                WorkoutDay? selectedDay = null;

                if(WorkoutDays.Count == 1)
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

                    if(selectedDayName is not null and not "Annulla")
                    {
                        int index = Array.IndexOf(dayNames, selectedDayName);
                        if(index >= 0 && index < WorkoutDays.Count)
                        {
                            selectedDay = WorkoutDays[index];
                        }
                    }
                }

                if(selectedDay != null)
                {
                    Dictionary<string, object> parameters = new()
                    {
                        { "WorkoutPlan", WorkoutPlan },
                        { "WorkoutDay", selectedDay }
                    };

                    await _navigationService.NavigateToAsync("workoutexecution", parameters);
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'avvio dell'allenamento: {ex.Message}");
            }
        }

        private async Task EditPlan()
        {
            if(WorkoutPlan == null)
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
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'apertura dell'editor: {ex.Message}");
            }
        }

        private async Task DuplicatePlan()
        {
            if(WorkoutPlan == null)
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

                if(confirmed)
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
                        User = new Models.User { Id = WorkoutPlan.UserId, Name = "User" }, // Placeholder
                        WorkoutDays = [] // Inizializza lista vuota
                    };

                    _ = await _workoutPlanRepository.AddAsync(duplicatedPlan);
                    await _dialogService.ShowAlertAsync("Successo", "Piano duplicato con successo!");
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella duplicazione: {ex.Message}");
            }
        }

        private async Task DeletePlan()
        {
            if(WorkoutPlan == null)
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

                if(confirmed)
                {
                    await _workoutPlanRepository.DeleteAsync(WorkoutPlan.Id);
                    await _dialogService.ShowAlertAsync("Eliminato", "Piano eliminato con successo");
                    await _navigationService.GoBackAsync();
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'eliminazione: {ex.Message}");
            }
        }

        private async Task SelectWorkoutDay(WorkoutDay? workoutDay)
        {
            if(workoutDay == null)
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
            if(WorkoutPlan == null)
            {
                return;
            }

            try
            {
                string shareText = $"Piano di Allenamento: {WorkoutPlan.Title}\n";

                if(HasDescription)
                {
                    shareText += $"Descrizione: {WorkoutPlan.Description}\n";
                }

                shareText += $"Giorni: {WorkoutDaysListText}\n";
                shareText += $"Esercizi totali: {TotalExercisesCount}\n\n";
                shareText += "Condiviso da CraftMyFit";

                // TODO: Implementare la condivisione nativa
                await _dialogService.ShowAlertAsync("Condivisione", shareText);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella condivisione: {ex.Message}");
            }
        }

        private async Task ViewExercise(WorkoutExercise? workoutExercise)
        {
            if(workoutExercise?.Exercise == null)
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
            catch(Exception ex)
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

        #endregion

        #region Public Methods

        public async Task OnWorkoutPlanUpdated() =>
            // Chiamato quando il piano viene aggiornato
            await LoadDetails();

        public void OnAppearing()
        {
            // Ricarica i dettagli quando la pagina appare
            if(WorkoutPlan != null)
            {
                _ = Task.Run(LoadDetails);
            }
        }

        #endregion
    }
}