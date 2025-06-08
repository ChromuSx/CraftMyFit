using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    [QueryProperty(nameof(WorkoutPlan), "WorkoutPlan")]
    [QueryProperty(nameof(IsEdit), "IsEdit")]
    public class EditWorkoutPlanViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private WorkoutPlan? _originalWorkoutPlan;
        private string _planTitle = string.Empty;
        private string _planDescription = string.Empty;
        private ObservableCollection<DayOfWeek> _selectedDays = [];
        private ObservableCollection<WorkoutDay> _workoutDays = [];
        private WorkoutDay? _currentWorkoutDay;
        private bool _isEdit;
        private bool _isSaving;
        private bool _hasChanges;
        private int? _pendingAddExerciseDayId;

        public EditWorkoutPlanViewModel(
            IWorkoutPlanRepository workoutPlanRepository,
            IExerciseRepository exerciseRepository,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _exerciseRepository = exerciseRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Modifica Piano";

            // Inizializza le collezioni
            InitializeAvailableDays();

            // Sottoscrivi l'evento CollectionChanged
            _workoutDays.CollectionChanged += WorkoutDays_CollectionChanged;

            // Inizializza i comandi
            SaveChangesCommand = new Command(async () => await SaveChanges(), CanSaveChanges);
            CancelCommand = new Command(async () => await Cancel());
            AddWorkoutDayCommand = new Command(async () => await AddWorkoutDay());
            RemoveWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await RemoveWorkoutDay(day));
            EditWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await EditWorkoutDay(day));
            AddExerciseToWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await AddExerciseToWorkoutDay(day));
            RemoveExerciseCommand = new Command<(WorkoutDay day, WorkoutExercise exercise)>(async (param) => await RemoveExercise(param.day, param.exercise));
            ResetChangesCommand = new Command(async () => await ResetChanges());
        }

        #region Properties

        public WorkoutPlan? WorkoutPlan
        {
            get => _originalWorkoutPlan;
            set
            {
                if (SetProperty(ref _originalWorkoutPlan, value))
                {
                    OnWorkoutPlanChanged();
                }
            }
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public string PlanTitle
        {
            get => _planTitle;
            set
            {
                if (SetProperty(ref _planTitle, value))
                {
                    CheckForChanges();
                    ((Command)SaveChangesCommand).ChangeCanExecute();
                }
            }
        }

        public string PlanDescription
        {
            get => _planDescription;
            set
            {
                if (SetProperty(ref _planDescription, value))
                {
                    CheckForChanges();
                }
            }
        }

        public ObservableCollection<DayOfWeek> SelectedDays
        {
            get => _selectedDays;
            set => SetProperty(ref _selectedDays, value);
        }

        public ObservableCollection<WorkoutDay> WorkoutDays
        {
            get => _workoutDays;
            set
            {
                if (_workoutDays != null)
                    _workoutDays.CollectionChanged -= WorkoutDays_CollectionChanged;
                if (SetProperty(ref _workoutDays, value))
                {
                    if (_workoutDays != null)
                        _workoutDays.CollectionChanged += WorkoutDays_CollectionChanged;
                    OnPropertyChanged(nameof(HasWorkoutDays));
                }
            }
        }

        public WorkoutDay? CurrentWorkoutDay
        {
            get => _currentWorkoutDay;
            set => SetProperty(ref _currentWorkoutDay, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set => SetProperty(ref _hasChanges, value);
        }

        // Proprietà per i giorni della settimana
        public ObservableCollection<DaySelectionItem> AvailableDays { get; set; } = [];

        // Proprietà calcolate
        public bool HasWorkoutDays => WorkoutDays.Any();
        public string SaveButtonText => IsSaving ? "Salvataggio..." : "Salva Modifiche";
        public string WorkoutDaysCountText => WorkoutDays.Count == 1 ? "1 giorno" : $"{WorkoutDays.Count} giorni";
        public string PageTitle => IsEdit ? "Modifica Piano" : "Nuovo Piano";

        #endregion

        #region Commands

        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddWorkoutDayCommand { get; }
        public ICommand RemoveWorkoutDayCommand { get; }
        public ICommand EditWorkoutDayCommand { get; }
        public ICommand AddExerciseToWorkoutDayCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand ResetChangesCommand { get; }

        #endregion

        #region Private Methods

        private void InitializeAvailableDays()
        {
            DaySelectionItem[] days = new[]
            {
                new DaySelectionItem { Day = DayOfWeek.Monday, DisplayName = "Lunedì", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Tuesday, DisplayName = "Martedì", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Wednesday, DisplayName = "Mercoledì", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Thursday, DisplayName = "Giovedì", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Friday, DisplayName = "Venerdì", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Saturday, DisplayName = "Sabato", IsSelected = false },
                new DaySelectionItem { Day = DayOfWeek.Sunday, DisplayName = "Domenica", IsSelected = false }
            };

            foreach (DaySelectionItem day in days)
            {
                day.PropertyChanged += OnDaySelectionChanged;
                AvailableDays.Add(day);
            }
        }

        private async void OnWorkoutPlanChanged()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            try
            {
                // Carica i dettagli completi del piano
                WorkoutPlan fullPlan = await _workoutPlanRepository.GetWorkoutPlanWithDetailsAsync(WorkoutPlan.Id);
                if (fullPlan != null)
                {
                    // Inizializza i campi con i dati esistenti
                    PlanTitle = fullPlan.Title;
                    PlanDescription = fullPlan.Description ?? string.Empty;

                    // Inizializza i giorni selezionati
                    List<DayOfWeek> workoutDays = fullPlan.WorkoutDaysEnum;
                    foreach (DaySelectionItem day in AvailableDays)
                    {
                        day.IsSelected = workoutDays.Contains(day.Day);
                    }

                    // Inizializza i workout days - correzione dell'operatore ??
                    WorkoutDays = fullPlan.WorkoutDays != null ? [.. fullPlan.WorkoutDays.OrderBy(wd => wd.OrderIndex)] : [];

                    Title = $"Modifica: {fullPlan.Title}";
                    HasChanges = false;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento del piano: {ex.Message}");
            }
        }

        private void OnDaySelectionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DaySelectionItem.IsSelected) && sender is DaySelectionItem dayItem)
            {
                if (dayItem.IsSelected)
                {
                    if (!SelectedDays.Contains(dayItem.Day))
                    {
                        SelectedDays.Add(dayItem.Day);
                        CreateWorkoutDayForDay(dayItem.Day);
                    }
                }
                else
                {
                    _ = SelectedDays.Remove(dayItem.Day);
                    RemoveWorkoutDayForDay(dayItem.Day);
                }

                CheckForChanges();
                ((Command)SaveChangesCommand).ChangeCanExecute();
            }
        }

        private void CreateWorkoutDayForDay(DayOfWeek day)
        {
            // Controlla se esiste già un workout day per questo giorno
            WorkoutDay? existingDay = WorkoutDays.FirstOrDefault(wd => wd.DayOfWeek == day);
            if (existingDay != null)
            {
                return;
            }

            WorkoutDay workoutDay = new()
            {
                DayOfWeek = day,
                Title = GetDefaultWorkoutDayTitle(day),
                OrderIndex = (int)day,
                Exercises = [],
                WorkoutPlanId = WorkoutPlan?.Id ?? 0,
                WorkoutPlan = WorkoutPlan!
            };

            WorkoutDays.Add(workoutDay);
        }

        private void RemoveWorkoutDayForDay(DayOfWeek day)
        {
            WorkoutDay? workoutDay = WorkoutDays.FirstOrDefault(wd => wd.DayOfWeek == day);
            if (workoutDay != null)
            {
                _ = WorkoutDays.Remove(workoutDay);
            }
        }

        private string GetDefaultWorkoutDayTitle(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Allenamento Lunedì",
                DayOfWeek.Tuesday => "Allenamento Martedì",
                DayOfWeek.Wednesday => "Allenamento Mercoledì",
                DayOfWeek.Thursday => "Allenamento Giovedì",
                DayOfWeek.Friday => "Allenamento Venerdì",
                DayOfWeek.Saturday => "Allenamento Sabato",
                DayOfWeek.Sunday => "Allenamento Domenica",
                _ => "Allenamento"
            };
        }

        private void CheckForChanges()
        {
            if (WorkoutPlan == null)
            {
                return;
            }

            bool hasChanges = PlanTitle != WorkoutPlan.Title ||
                           PlanDescription != (WorkoutPlan.Description ?? string.Empty) ||
                           !SelectedDays.OrderBy(d => d).SequenceEqual(WorkoutPlan.WorkoutDaysEnum.OrderBy(d => d)) ||
                           WorkoutDays.Count != (WorkoutPlan.WorkoutDays?.Count ?? 0);

            HasChanges = hasChanges;
        }

        private bool CanSaveChanges()
        {
            return !string.IsNullOrWhiteSpace(PlanTitle) &&
                   HasChanges &&
                   WorkoutDays.Any() &&
                   !IsSaving;
        }

        private async Task SaveChanges()
        {
            if (!CanSaveChanges() || WorkoutPlan == null)
            {
                return;
            }

            try
            {
                IsSaving = true;

                // Aggiorna il piano esistente
                WorkoutPlan.Title = PlanTitle.Trim();
                WorkoutPlan.Description = PlanDescription?.Trim() ?? string.Empty;
                WorkoutPlan.ModifiedDate = DateTime.Now;
                WorkoutPlan.WorkoutDaysEnum = SelectedDays.ToList();
                WorkoutPlan.WorkoutDays = WorkoutDays.ToList();

                // Salva nel database
                await _workoutPlanRepository.UpdateAsync(WorkoutPlan);

                // TODO: Aggiornare anche i WorkoutDay e WorkoutExercise separatamente

                await _dialogService.ShowAlertAsync("Successo", "Piano di allenamento aggiornato con successo!");

                HasChanges = false;
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio delle modifiche: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task Cancel()
        {
            if (HasChanges)
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Annulla Modifiche",
                    "Sei sicuro di voler annullare? Le modifiche andranno perse.",
                    "Annulla Modifiche",
                    "Continua");

                if (!confirmed)
                {
                    return;
                }
            }

            await _navigationService.GoBackAsync();
        }

        private async Task AddWorkoutDay()
        {
            DaySelectionItem[] availableDays = AvailableDays.Where(d => !d.IsSelected).ToArray();

            if (!availableDays.Any())
            {
                await _dialogService.ShowAlertAsync("Tutti i Giorni Selezionati", "Hai già selezionato tutti i giorni della settimana.");
                return;
            }

            string[] dayNames = availableDays.Select(d => d.DisplayName).ToArray();
            string? selectedDayName = await _dialogService.ShowActionSheetAsync(
                "Seleziona Giorno",
                "Annulla",
                null,
                dayNames);

            if (selectedDayName is not null and not "Annulla")
            {
                DaySelectionItem? dayItem = availableDays.FirstOrDefault(d => d.DisplayName == selectedDayName);
                if (dayItem != null)
                {
                    dayItem.IsSelected = true;
                }
            }
        }

        private async Task RemoveWorkoutDay(WorkoutDay? workoutDay)
        {
            if (workoutDay == null)
            {
                return;
            }

            int exerciseCount = workoutDay.Exercises?.Count ?? 0;
            string message = exerciseCount > 0 ?
                $"Questo giorno contiene {exerciseCount} esercizio/i. Vuoi rimuoverlo comunque?" :
                $"Sei sicuro di voler rimuovere {workoutDay.Title}?";

            bool confirmed = await _dialogService.ShowConfirmAsync(
                "Rimuovi Giorno",
                message,
                "Rimuovi",
                "Annulla");

            if (confirmed)
            {
                DaySelectionItem? dayItem = AvailableDays.FirstOrDefault(d => d.Day == workoutDay.DayOfWeek);
                if (dayItem != null)
                {
                    dayItem.IsSelected = false;
                }
            }
        }

        private async Task EditWorkoutDay(WorkoutDay? workoutDay)
        {
            if (workoutDay == null)
            {
                return;
            }

            string? newTitle = await _dialogService.ShowPromptAsync(
                "Modifica Titolo",
                "Inserisci il nuovo titolo per questo giorno:",
                workoutDay.Title);

            if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != workoutDay.Title)
            {
                workoutDay.Title = newTitle.Trim();
                CheckForChanges();
            }
        }

        private async Task AddExerciseToWorkoutDay(WorkoutDay? workoutDay)
        {
            if (workoutDay == null)
            {
                return;
            }

            _pendingAddExerciseDayId = workoutDay.Id;
            await Shell.Current.GoToAsync($"exercisespage?IsSelectionMode=true");
        }

        private async Task RemoveExercise(WorkoutDay day, WorkoutExercise exercise)
        {
            bool confirmed = await _dialogService.ShowConfirmAsync(
                "Rimuovi Esercizio",
                $"Sei sicuro di voler rimuovere {exercise.Exercise?.Name} da {day.Title}?",
                "Rimuovi",
                "Annulla");

            if (confirmed)
            {
                _ = (day.Exercises?.Remove(exercise));
                CheckForChanges();
            }
        }

        private async Task ResetChanges()
        {
            if (!HasChanges)
            {
                return;
            }

            bool confirmed = await _dialogService.ShowConfirmAsync(
                "Ripristina Modifiche",
                "Sei sicuro di voler annullare tutte le modifiche e ripristinare i valori originali?",
                "Ripristina",
                "Annulla");

            if (confirmed)
            {
                // Ripristina i valori originali - rimuovi await
                OnWorkoutPlanChanged();
            }
        }

        private void WorkoutDays_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasWorkoutDays));
        }

        #endregion

        #region Public Methods

        public void OnAppearing()
        {
            // Aggiorna il check dei cambiamenti quando la pagina appare
            CheckForChanges();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Exercise", out object? exerciseObj) && exerciseObj is Exercise selectedExercise && _pendingAddExerciseDayId.HasValue)
            {
                WorkoutDay? workoutDay = WorkoutDays.FirstOrDefault(wd => wd.Id == _pendingAddExerciseDayId.Value);
                if (workoutDay != null && selectedExercise != null)
                {
                    WorkoutExercise workoutExercise = new()
                    {
                        ExerciseId = selectedExercise.Id,
                        Exercise = selectedExercise,
                        Sets = 3,
                        Reps = 10,
                        Weight = 0,
                        RestTime = TimeSpan.FromSeconds(60),
                        OrderIndex = workoutDay.Exercises.Count
                    };
                    workoutDay.Exercises.Add(workoutExercise);
                    CheckForChanges();
                }

                _pendingAddExerciseDayId = null;
            }
        }

        #endregion
    }
}