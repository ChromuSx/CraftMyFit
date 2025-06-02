using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models;
using CraftMyFit.Models.Gamification;
using CraftMyFit.Models.Progress;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IBodyMeasurementRepository? _bodyMeasurementRepository;
        private readonly IProgressPhotoRepository? _progressPhotoRepository;
        private readonly IAchievementRepository? _achievementRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        // Properties per i dati del dashboard
        private User? _currentUser;
        private WorkoutPlan? _nextWorkout;
        private BodyMeasurement? _latestMeasurement;
        private ProgressPhoto? _latestPhoto;
        private int _totalWorkouts;
        private int _totalPoints;
        private int _currentStreak;
        private ObservableCollection<Achievement> _recentAchievements = [];
        private ObservableCollection<WorkoutPlan> _activeWorkoutPlans = [];

        public DashboardViewModel(
            IWorkoutPlanRepository workoutPlanRepository,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService,
            IBodyMeasurementRepository? bodyMeasurementRepository = null,
            IProgressPhotoRepository? progressPhotoRepository = null,
            IAchievementRepository? achievementRepository = null)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _bodyMeasurementRepository = bodyMeasurementRepository;
            _progressPhotoRepository = progressPhotoRepository;
            _achievementRepository = achievementRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Dashboard";

            // Inizializza i comandi
            LoadDashboardDataCommand = new Command(async () => await LoadDashboardData());
            NavigateToWorkoutsCommand = new Command(async () => await NavigateToWorkouts());
            NavigateToProgressCommand = new Command(async () => await NavigateToProgress());
            NavigateToExercisesCommand = new Command(async () => await NavigateToExercises());
            StartQuickWorkoutCommand = new Command(async () => await StartQuickWorkout());
            ViewAchievementsCommand = new Command(async () => await ViewAchievements());
            RefreshCommand = new Command(async () => await RefreshDashboard());

            // Carica i dati iniziali
            _ = Task.Run(LoadDashboardData);
        }

        #region Properties

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public WorkoutPlan? NextWorkout
        {
            get => _nextWorkout;
            set => SetProperty(ref _nextWorkout, value);
        }

        public BodyMeasurement? LatestMeasurement
        {
            get => _latestMeasurement;
            set => SetProperty(ref _latestMeasurement, value);
        }

        public ProgressPhoto? LatestPhoto
        {
            get => _latestPhoto;
            set => SetProperty(ref _latestPhoto, value);
        }

        public int TotalWorkouts
        {
            get => _totalWorkouts;
            set => SetProperty(ref _totalWorkouts, value);
        }

        public int TotalPoints
        {
            get => _totalPoints;
            set => SetProperty(ref _totalPoints, value);
        }

        public int CurrentStreak
        {
            get => _currentStreak;
            set => SetProperty(ref _currentStreak, value);
        }

        public ObservableCollection<Achievement> RecentAchievements
        {
            get => _recentAchievements;
            set => SetProperty(ref _recentAchievements, value);
        }

        public ObservableCollection<WorkoutPlan> ActiveWorkoutPlans
        {
            get => _activeWorkoutPlans;
            set => SetProperty(ref _activeWorkoutPlans, value);
        }

        // Proprietà calcolate per il binding
        public string WelcomeMessage => CurrentUser != null ? $"Ciao, {CurrentUser.DisplayName}!" : "Benvenuto!";

        public string ProgressSummary
        {
            get
            {
                if(LatestMeasurement != null && CurrentUser?.StartingWeight != null)
                {
                    float weightChange = LatestMeasurement.Weight - CurrentUser.StartingWeight.Value;
                    string sign = weightChange >= 0 ? "+" : "";
                    return $"Peso: {LatestMeasurement.Weight:F1} kg ({sign}{weightChange:F1} kg)";
                }

                return LatestMeasurement != null ? $"Peso: {LatestMeasurement.Weight:F1} kg" : "Nessuna misurazione";
            }
        }

        public string NextWorkoutInfo => NextWorkout != null ? $"Prossimo: {NextWorkout.Title}" : "Nessun allenamento programmato";

        public bool HasRecentAchievements => RecentAchievements.Any();
        public bool HasActiveWorkouts => ActiveWorkoutPlans.Any();

        #endregion

        #region Commands

        public ICommand LoadDashboardDataCommand { get; }
        public ICommand NavigateToWorkoutsCommand { get; }
        public ICommand NavigateToProgressCommand { get; }
        public ICommand NavigateToExercisesCommand { get; }
        public ICommand StartQuickWorkoutCommand { get; }
        public ICommand ViewAchievementsCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Private Methods

        private async Task LoadDashboardData()
        {
            if(IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Carica l'utente corrente
                LoadCurrentUser();

                if(CurrentUser == null)
                {
                    return;
                }

                // Carica i dati in parallelo per migliorare le performance
                List<Task> tasks = new()
                {
                    LoadWorkoutData(),
                    LoadProgressData(),
                    LoadAchievementData()
                };

                await Task.WhenAll(tasks);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento dei dati: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadCurrentUser() =>
            // Per ora creiamo un utente fittizio
            // In un'implementazione completa, questo verrebbe dal database o dalle preferenze
            CurrentUser = new User
            {
                Id = 1,
                Name = _preferenceService.GetString("user_name", "Utente"),
                RegistrationDate = DateTime.Now.AddDays(-30),
                StartingWeight = 75.0f
            };

        private async Task LoadWorkoutData()
        {
            if(CurrentUser == null)
            {
                return;
            }

            try
            {
                // Carica i piani di allenamento attivi
                List<WorkoutPlan> workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(CurrentUser.Id);
                ActiveWorkoutPlans = [.. workoutPlans.Take(3)];

                // Trova il prossimo allenamento (per ora il primo piano)
                NextWorkout = workoutPlans.FirstOrDefault();

                // TODO: Calcolare il numero totale di allenamenti completati
                TotalWorkouts = _preferenceService.GetInt("total_workouts_completed", 0);

                // TODO: Calcolare la streak corrente
                CurrentStreak = _preferenceService.GetInt("current_workout_streak", 0);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dei dati di allenamento: {ex.Message}");
            }
        }

        private async Task LoadProgressData()
        {
            if(CurrentUser == null)
            {
                return;
            }

            try
            {
                // Carica l'ultima misurazione corporea
                if(_bodyMeasurementRepository != null)
                {
                    LatestMeasurement = await _bodyMeasurementRepository.GetLatestByUserIdAsync(CurrentUser.Id);
                }

                // Carica l'ultima foto di progresso
                if(_progressPhotoRepository != null)
                {
                    LatestPhoto = await _progressPhotoRepository.GetLatestByUserIdAsync(CurrentUser.Id);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dei dati di progresso: {ex.Message}");
            }
        }

        private async Task LoadAchievementData()
        {
            if(CurrentUser == null || _achievementRepository == null)
            {
                return;
            }

            try
            {
                // Carica i riconoscimenti recenti
                var recentAchievements = await _achievementRepository.GetRecentlyUnlockedAsync(CurrentUser.Id, 7);
                RecentAchievements = new ObservableCollection<Achievement>(recentAchievements.Take(3));

                // Carica i punti totali
                TotalPoints = await _achievementRepository.GetTotalPointsByUserIdAsync(CurrentUser.Id);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel caricamento dei dati dei riconoscimenti: {ex.Message}");
            }
        }

        private async Task NavigateToWorkouts() => await _navigationService.NavigateToAsync("//workoutplans");

        private async Task NavigateToProgress() => await _navigationService.NavigateToAsync("//progress");

        private async Task NavigateToExercises() => await _navigationService.NavigateToAsync("//exercises");

        private async Task StartQuickWorkout()
        {
            if(NextWorkout == null)
            {
                await _dialogService.ShowAlertAsync("Nessun Allenamento", "Crea prima un piano di allenamento per iniziare.");
                return;
            }

            try
            {
                Dictionary<string, object> parameters = new()
                {
                    { "WorkoutPlan", NextWorkout }
                };

                await _navigationService.NavigateToAsync("workoutexecution", parameters);
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'avvio dell'allenamento: {ex.Message}");
            }
        }

        private async Task ViewAchievements() =>
            // TODO: Navigare alla pagina dei riconoscimenti quando sarà implementata
            await _dialogService.ShowAlertAsync("In Arrivo", "La sezione riconoscimenti sarà disponibile presto!");

        private async Task RefreshDashboard() => await LoadDashboardData();

        #endregion
    }
}