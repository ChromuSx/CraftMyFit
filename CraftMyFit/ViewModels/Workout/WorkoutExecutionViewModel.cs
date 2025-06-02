using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    [QueryProperty(nameof(WorkoutPlan), "WorkoutPlan")]
    public class WorkoutExecutionViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IPreferenceService _preferenceService;

        private WorkoutPlan? _workoutPlan;
        private WorkoutDay? _currentWorkoutDay;
        private WorkoutExercise? _currentExercise;
        private int _currentExerciseIndex;
        private int _currentSet = 1;
        private int _completedSets;
        private int _completedReps;
        private float _usedWeight;
        private TimeSpan _exerciseTime;
        private TimeSpan _totalWorkoutTime;
        private TimeSpan _restTime;
        private bool _isResting;
        private bool _isWorkoutActive = true;
        private bool _isPaused;
        private DateTime _workoutStartTime;
        private DateTime _exerciseStartTime;

        private ObservableCollection<WorkoutExercise> _exercises = [];
        private ObservableCollection<ExerciseLog> _exerciseLogs = [];

        // Timer per il cronometro
        private Timer? _workoutTimer;
        private Timer? _restTimer;

        public WorkoutExecutionViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            IPreferenceService preferenceService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;
            _preferenceService = preferenceService;

            Title = "Allenamento";

            // Inizializza i comandi
            StartWorkoutCommand = new Command(async () => await StartWorkout());
            PauseWorkoutCommand = new Command(PauseWorkout);
            ResumeWorkoutCommand = new Command(ResumeWorkout);
            CompleteSetCommand = new Command(async () => await CompleteSet());
            SkipExerciseCommand = new Command(async () => await SkipExercise());
            PreviousExerciseCommand = new Command(() => PreviousExercise());
            NextExerciseCommand = new Command(() => NextExercise());
            StartRestTimerCommand = new Command(StartRestTimer);
            SkipRestCommand = new Command(SkipRest);
            CompleteWorkoutCommand = new Command(async () => await CompleteWorkout());
            CancelWorkoutCommand = new Command(async () => await CancelWorkout());
            EditExerciseDataCommand = new Command(async () => await EditExerciseData());

            _workoutStartTime = DateTime.Now;
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

        public WorkoutDay? CurrentWorkoutDay
        {
            get => _currentWorkoutDay;
            set => SetProperty(ref _currentWorkoutDay, value);
        }

        public WorkoutExercise? CurrentExercise
        {
            get => _currentExercise;
            set => SetProperty(ref _currentExercise, value);
        }

        public int CurrentExerciseIndex
        {
            get => _currentExerciseIndex;
            set => SetProperty(ref _currentExerciseIndex, value);
        }

        public int CurrentSet
        {
            get => _currentSet;
            set => SetProperty(ref _currentSet, value);
        }

        public int CompletedSets
        {
            get => _completedSets;
            set => SetProperty(ref _completedSets, value);
        }

        public int CompletedReps
        {
            get => _completedReps;
            set => SetProperty(ref _completedReps, value);
        }

        public float UsedWeight
        {
            get => _usedWeight;
            set => SetProperty(ref _usedWeight, value);
        }

        public TimeSpan ExerciseTime
        {
            get => _exerciseTime;
            set => SetProperty(ref _exerciseTime, value);
        }

        public TimeSpan TotalWorkoutTime
        {
            get => _totalWorkoutTime;
            set => SetProperty(ref _totalWorkoutTime, value);
        }

        public TimeSpan RestTime
        {
            get => _restTime;
            set => SetProperty(ref _restTime, value);
        }

        public bool IsResting
        {
            get => _isResting;
            set => SetProperty(ref _isResting, value);
        }

        public bool IsWorkoutActive
        {
            get => _isWorkoutActive;
            set => SetProperty(ref _isWorkoutActive, value);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public ObservableCollection<WorkoutExercise> Exercises
        {
            get => _exercises;
            set => SetProperty(ref _exercises, value);
        }

        public ObservableCollection<ExerciseLog> ExerciseLogs
        {
            get => _exerciseLogs;
            set => SetProperty(ref _exerciseLogs, value);
        }

        // Proprietà calcolate
        public string WorkoutTitle => WorkoutPlan?.Title ?? "Allenamento";
        public string CurrentExerciseName => CurrentExercise?.Exercise?.Name ?? "Nessun esercizio";
        public string ProgressText => $"Esercizio {CurrentExerciseIndex + 1} di {Exercises.Count}";
        public string SetProgressText => $"Serie {CurrentSet} di {CurrentExercise?.Sets ?? 0}";
        public string ExerciseTimeText => ExerciseTime.ToString(@"mm\:ss");
        public string TotalWorkoutTimeText => TotalWorkoutTime.ToString(@"hh\:mm\:ss");
        public string RestTimeText => RestTime.ToString(@"mm\:ss");
        public bool CanGoToPreviousExercise => CurrentExerciseIndex > 0;
        public bool CanGoToNextExercise => CurrentExerciseIndex < Exercises.Count - 1;
        public bool CanCompleteSet => IsWorkoutActive && !IsPaused && !IsResting;
        public float WorkoutProgress => Exercises.Count > 0 ? (float)(CurrentExerciseIndex + 1) / Exercises.Count : 0f;

        #endregion

        #region Commands

        public ICommand StartWorkoutCommand { get; }
        public ICommand PauseWorkoutCommand { get; }
        public ICommand ResumeWorkoutCommand { get; }
        public ICommand CompleteSetCommand { get; }
        public ICommand SkipExerciseCommand { get; }
        public ICommand PreviousExerciseCommand { get; }
        public ICommand NextExerciseCommand { get; }
        public ICommand StartRestTimerCommand { get; }
        public ICommand SkipRestCommand { get; }
        public ICommand CompleteWorkoutCommand { get; }
        public ICommand CancelWorkoutCommand { get; }
        public ICommand EditExerciseDataCommand { get; }

        #endregion

        #region Private Methods

        private void OnWorkoutPlanChanged()
        {
            if(WorkoutPlan == null)
            {
                return;
            }

            // Per ora, selezioniamo il primo giorno di allenamento
            CurrentWorkoutDay = WorkoutPlan.WorkoutDays?.FirstOrDefault();

            if(CurrentWorkoutDay?.Exercises != null)
            {
                Exercises = [.. CurrentWorkoutDay.Exercises];
                CurrentExercise = Exercises.FirstOrDefault();
                CurrentExerciseIndex = 0;
            }

            Title = $"Allenamento: {WorkoutPlan.Title}";
            InitializeCurrentExercise();
        }

        private void InitializeCurrentExercise()
        {
            if(CurrentExercise == null)
            {
                return;
            }

            CurrentSet = 1;
            CompletedSets = 0;
            CompletedReps = CurrentExercise.Reps;
            UsedWeight = CurrentExercise.Weight;
            _exerciseStartTime = DateTime.Now;
        }

        private async Task StartWorkout()
        {
            _workoutStartTime = DateTime.Now;
            _exerciseStartTime = DateTime.Now;
            IsWorkoutActive = true;
            IsPaused = false;

            StartWorkoutTimer();

            await _dialogService.ShowAlertAsync("Allenamento Iniziato", "Buon allenamento! 💪");
        }

        private void PauseWorkout()
        {
            IsPaused = true;
            _workoutTimer?.Dispose();
            _restTimer?.Dispose();
        }

        private void ResumeWorkout()
        {
            IsPaused = false;
            StartWorkoutTimer();

            if(IsResting)
            {
                StartRestTimer();
            }
        }

        private async Task CompleteSet()
        {
            if(CurrentExercise == null)
            {
                return;
            }

            // Registra il set completato
            ExerciseLog exerciseLog = new()
            {
                StartTime = _exerciseStartTime,
                EndTime = DateTime.Now,
                SetsCompleted = 1,
                RepsCompleted = CompletedReps,
                WeightUsed = UsedWeight,
                ExerciseId = CurrentExercise.ExerciseId,
                Exercise = CurrentExercise.Exercise
            };

            ExerciseLogs.Add(exerciseLog);
            CompletedSets++;

            if(CompletedSets >= CurrentExercise.Sets)
            {
                // Esercizio completato
                await _dialogService.ShowAlertAsync("Esercizio Completato!", $"{CurrentExercise.Exercise?.Name} completato!");

                if(CurrentExerciseIndex < Exercises.Count - 1)
                {
                    NextExercise();
                }
                else
                {
                    await CompleteWorkout();
                }
            }
            else
            {
                // Inizia il riposo tra le serie
                CurrentSet++;
                StartRestTimer();
            }
        }

        private async Task SkipExercise()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "Salta Esercizio",
                $"Sei sicuro di voler saltare {CurrentExercise?.Exercise?.Name}?",
                "Salta",
                "Annulla");

            if(confirmed)
            {
                if(CurrentExerciseIndex < Exercises.Count - 1)
                {
                    NextExercise();
                }
                else
                {
                    await CompleteWorkout();
                }
            }
        }

        private void PreviousExercise()
        {
            if(CurrentExerciseIndex > 0)
            {
                CurrentExerciseIndex--;
                CurrentExercise = Exercises[CurrentExerciseIndex];
                InitializeCurrentExercise();
                StopRest();
            }
        }

        private void NextExercise()
        {
            if(CurrentExerciseIndex < Exercises.Count - 1)
            {
                CurrentExerciseIndex++;
                CurrentExercise = Exercises[CurrentExerciseIndex];
                InitializeCurrentExercise();
                StopRest();
            }
        }

        private void StartRestTimer()
        {
            if(CurrentExercise == null)
            {
                return;
            }

            IsResting = true;
            RestTime = CurrentExercise.RestTime;

            _restTimer = new Timer(OnRestTimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void OnRestTimerTick(object? state) => MainThread.BeginInvokeOnMainThread(() =>
                                                                {
                                                                    if(RestTime.TotalSeconds > 0)
                                                                    {
                                                                        RestTime = RestTime.Subtract(TimeSpan.FromSeconds(1));
                                                                    }
                                                                    else
                                                                    {
                                                                        StopRest();
                                                                    }
                                                                });

        private void StopRest()
        {
            IsResting = false;
            _restTimer?.Dispose();
            RestTime = TimeSpan.Zero;
        }

        private void SkipRest() => StopRest();

        private void StartWorkoutTimer() => _workoutTimer = new Timer(OnWorkoutTimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        private void OnWorkoutTimerTick(object? state) => MainThread.BeginInvokeOnMainThread(() =>
                                                                   {
                                                                       if(!IsPaused)
                                                                       {
                                                                           TotalWorkoutTime = DateTime.Now - _workoutStartTime;
                                                                           ExerciseTime = DateTime.Now - _exerciseStartTime;
                                                                       }
                                                                   });

        private async Task CompleteWorkout()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "Completa Allenamento",
                "Sei sicuro di voler completare l'allenamento?",
                "Completa",
                "Continua");

            if(confirmed)
            {
                IsWorkoutActive = false;
                _workoutTimer?.Dispose();
                _restTimer?.Dispose();

                // TODO: Salvare la sessione di allenamento nel database
                SaveWorkoutSession();

                await _dialogService.ShowAlertAsync(
                    "Allenamento Completato! 🎉",
                    $"Ottimo lavoro! Allenamento durato {TotalWorkoutTimeText}");

                await _navigationService.GoBackAsync();
            }
        }

        private async Task CancelWorkout()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "Annulla Allenamento",
                "Sei sicuro di voler annullare l'allenamento? I progressi andranno persi.",
                "Annulla Allenamento",
                "Continua");

            if(confirmed)
            {
                IsWorkoutActive = false;
                _workoutTimer?.Dispose();
                _restTimer?.Dispose();

                await _navigationService.GoBackAsync();
            }
        }

        private async Task EditExerciseData()
        {
            // Mostra un dialog per modificare reps e peso
            var repsText = await _dialogService.ShowPromptAsync(
                "Modifica Ripetizioni",
                "Inserisci il numero di ripetizioni:",
                CompletedReps.ToString());

            if(int.TryParse(repsText, out int newReps) && newReps > 0)
            {
                CompletedReps = newReps;
            }

            var weightText = await _dialogService.ShowPromptAsync(
                "Modifica Peso",
                "Inserisci il peso utilizzato (kg):",
                UsedWeight.ToString("F1"));

            if(float.TryParse(weightText, out float newWeight) && newWeight >= 0)
            {
                UsedWeight = newWeight;
            }
        }

        private void SaveWorkoutSession()
        {
            try
            {
                // TODO: Implementare il salvataggio della sessione di allenamento
                // Qui salveresti WorkoutSession e ExerciseLog nel database

                // Aggiorna le statistiche dell'utente
                var totalWorkouts = _preferenceService.GetInt("total_workouts_completed", 0);
                _preferenceService.SetInt("total_workouts_completed", totalWorkouts + 1);

                // TODO: Aggiornare la streak e controllare gli achievement
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio sessione: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods

        public void OnAppearing()
        {
            // Se l'allenamento non è ancora iniziato, chiedi se iniziare
            if(!IsWorkoutActive && !IsPaused)
            {
                // L'allenamento verrà iniziato manualmente dall'utente
            }
        }

        public void OnDisappearing()
        {
            // Pausa l'allenamento quando la pagina scompare
            if(IsWorkoutActive && !IsPaused)
            {
                PauseWorkout();
            }
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            _workoutTimer?.Dispose();
            _restTimer?.Dispose();
        }

        #endregion
    }

    // Modello temporaneo per i log degli esercizi (dovrebbe essere nel Models)
    public class ExerciseLog
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SetsCompleted { get; set; }
        public int RepsCompleted { get; set; }
        public float WeightUsed { get; set; }
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
    }
}