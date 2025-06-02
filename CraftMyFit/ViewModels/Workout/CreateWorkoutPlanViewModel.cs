using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Workout
{
    [QueryProperty(nameof(PreselectedExercise), "PreselectedExercise")]
    public class CreateWorkoutPlanViewModel : BaseViewModel
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private string _planTitle = string.Empty;
        private string _planDescription = string.Empty;
        private ObservableCollection<DayOfWeek> _selectedDays = [];
        private ObservableCollection<WorkoutDay> _workoutDays = [];
        private WorkoutDay? _currentWorkoutDay;
        private Exercise? _preselectedExercise;
        private bool _isSaving;

        public CreateWorkoutPlanViewModel(
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

            Title = "Nuovo Piano";

            // Inizializza le collezioni
            InitializeAvailableDays();

            // Inizializza i comandi
            SavePlanCommand = new Command(async () => await SavePlan(), CanSavePlan);
            CancelCommand = new Command(async () => await Cancel());
            AddWorkoutDayCommand = new Command(async () => await AddWorkoutDay());
            RemoveWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await RemoveWorkoutDay(day));
            EditWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await EditWorkoutDay(day));
            AddExerciseToWorkoutDayCommand = new Command<WorkoutDay>(async (day) => await AddExerciseToWorkoutDay(day));
            UseTemplateCommand = new Command(async () => await UseTemplate());
        }

        #region Properties

        public string PlanTitle
        {
            get => _planTitle;
            set
            {
                if(SetProperty(ref _planTitle, value))
                {
                    ((Command)SavePlanCommand).ChangeCanExecute();
                }
            }
        }

        public string PlanDescription
        {
            get => _planDescription;
            set => SetProperty(ref _planDescription, value);
        }

        public ObservableCollection<DayOfWeek> SelectedDays
        {
            get => _selectedDays;
            set => SetProperty(ref _selectedDays, value);
        }

        public ObservableCollection<WorkoutDay> WorkoutDays
        {
            get => _workoutDays;
            set => SetProperty(ref _workoutDays, value);
        }

        public WorkoutDay? CurrentWorkoutDay
        {
            get => _currentWorkoutDay;
            set => SetProperty(ref _currentWorkoutDay, value);
        }

        public Exercise? PreselectedExercise
        {
            get => _preselectedExercise;
            set
            {
                if(SetProperty(ref _preselectedExercise, value))
                {
                    OnPreselectedExerciseChanged();
                }
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        // Proprietà per i giorni della settimana
        public ObservableCollection<DaySelectionItem> AvailableDays { get; set; } = [];

        // Proprietà calcolate
        public bool HasWorkoutDays => WorkoutDays.Any();
        public string SaveButtonText => IsSaving ? "Salvataggio..." : "Salva Piano";
        public string WorkoutDaysCountText => WorkoutDays.Count == 1 ? "1 giorno" : $"{WorkoutDays.Count} giorni";

        #endregion

        #region Commands

        public ICommand SavePlanCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddWorkoutDayCommand { get; }
        public ICommand RemoveWorkoutDayCommand { get; }
        public ICommand EditWorkoutDayCommand { get; }
        public ICommand AddExerciseToWorkoutDayCommand { get; }
        public ICommand UseTemplateCommand { get; }

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

            foreach(DaySelectionItem? day in days)
            {
                day.PropertyChanged += OnDaySelectionChanged;
                AvailableDays.Add(day);
            }
        }

        private void OnDaySelectionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(DaySelectionItem.IsSelected) && sender is DaySelectionItem dayItem)
            {
                if(dayItem.IsSelected)
                {
                    if(!SelectedDays.Contains(dayItem.Day))
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

                ((Command)SavePlanCommand).ChangeCanExecute();
            }
        }

        private void CreateWorkoutDayForDay(DayOfWeek day)
        {
            WorkoutDay workoutDay = new()
            {
                DayOfWeek = day,
                Title = GetDefaultWorkoutDayTitle(day),
                OrderIndex = (int)day,
                Exercises = [],
                WorkoutPlanId = 0, // Sarà impostato quando si salva il piano
                WorkoutPlan = null! // Sarà impostato quando si salva il piano
            };

            WorkoutDays.Add(workoutDay);
        }

        private void RemoveWorkoutDayForDay(DayOfWeek day)
        {
            WorkoutDay? workoutDay = WorkoutDays.FirstOrDefault(wd => wd.DayOfWeek == day);
            if(workoutDay != null)
            {
                _ = WorkoutDays.Remove(workoutDay);
            }
        }

        private string GetDefaultWorkoutDayTitle(DayOfWeek day) => day switch
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

        private void OnPreselectedExerciseChanged()
        {
            if(PreselectedExercise != null)
            {
                // Se c'è un esercizio preselezionato, crea automaticamente un giorno di allenamento
                PlanTitle = $"Piano con {PreselectedExercise.Name}";

                // Seleziona alcuni giorni di default (es. Lunedì, Mercoledì, Venerdì)
                DayOfWeek[] defaultDays = new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday };
                foreach(DayOfWeek day in defaultDays)
                {
                    DaySelectionItem? dayItem = AvailableDays.FirstOrDefault(d => d.Day == day);
                    if(dayItem != null)
                    {
                        dayItem.IsSelected = true;
                    }
                }
            }
        }

        private bool CanSavePlan() => !string.IsNullOrWhiteSpace(PlanTitle) &&
                   WorkoutDays.Any() &&
                   !IsSaving;

        private async Task SavePlan()
        {
            if(!CanSavePlan())
            {
                return;
            }

            try
            {
                IsSaving = true;

                // Crea il piano di allenamento
                int currentUserId = _preferenceService.GetInt("current_user_id", 1);
                WorkoutPlan workoutPlan = new()
                {
                    Title = PlanTitle.Trim(),
                    Description = PlanDescription?.Trim() ?? string.Empty,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    UserId = currentUserId,
                    User = new Models.User { Id = currentUserId, Name = "User" }, // Placeholder
                    WorkoutDays = WorkoutDays.ToList(),
                    WorkoutDaysJson = System.Text.Json.JsonSerializer.Serialize(SelectedDays.ToList()) // Inizializza JSON
                };

                // Salva nel database
                int planId = await _workoutPlanRepository.AddAsync(workoutPlan);

                await _dialogService.ShowAlertAsync("Successo", "Piano di allenamento creato con successo!");

                // Torna alla schermata precedente
                await _navigationService.GoBackAsync();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nella creazione del piano: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task Cancel()
        {
            if(HasChanges())
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Annulla Creazione",
                    "Sei sicuro di voler annullare? Le modifiche andranno perse.",
                    "Annulla",
                    "Continua");

                if(!confirmed)
                {
                    return;
                }
            }

            await _navigationService.GoBackAsync();
        }

        private bool HasChanges() => !string.IsNullOrWhiteSpace(PlanTitle) ||
                   !string.IsNullOrWhiteSpace(PlanDescription) ||
                   WorkoutDays.Any();

        private async Task AddWorkoutDay()
        {
            // Mostra dialog per selezionare un giorno
            DaySelectionItem[] availableDays = AvailableDays.Where(d => !d.IsSelected).ToArray();

            if(!availableDays.Any())
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

            if(selectedDayName is not null and not "Annulla")
            {
                DaySelectionItem? dayItem = availableDays.FirstOrDefault(d => d.DisplayName == selectedDayName);
                if(dayItem != null)
                {
                    dayItem.IsSelected = true;
                }
            }
        }

        private async Task RemoveWorkoutDay(WorkoutDay? workoutDay)
        {
            if(workoutDay == null)
            {
                return;
            }

            bool confirmed = await _dialogService.ShowConfirmAsync(
                "Rimuovi Giorno",
                $"Sei sicuro di voler rimuovere {workoutDay.Title}?",
                "Rimuovi",
                "Annulla");

            if(confirmed)
            {
                DaySelectionItem? dayItem = AvailableDays.FirstOrDefault(d => d.Day == workoutDay.DayOfWeek);
                if(dayItem != null)
                {
                    dayItem.IsSelected = false;
                }
            }
        }

        private async Task EditWorkoutDay(WorkoutDay? workoutDay)
        {
            if(workoutDay == null)
            {
                return;
            }

            string? newTitle = await _dialogService.ShowPromptAsync(
                "Modifica Titolo",
                "Inserisci il nuovo titolo per questo giorno:",
                workoutDay.Title);

            if(!string.IsNullOrWhiteSpace(newTitle))
            {
                workoutDay.Title = newTitle;
            }
        }

        private async Task AddExerciseToWorkoutDay(WorkoutDay? workoutDay)
        {
            if(workoutDay == null)
            {
                return;
            }

            // TODO: Implementare la selezione degli esercizi
            await _dialogService.ShowAlertAsync("In Arrivo", "Selezione esercizi disponibile presto!");
        }

        private async Task UseTemplate()
        {
            string[] templates = new[]
            {
                "Principiante Full Body",
                "Push/Pull/Legs Intermedio",
                "Bodybuilding Avanzato",
                "Allenamento a Casa",
                "HIIT"
            };

            string? selectedTemplate = await _dialogService.ShowActionSheetAsync(
                "Seleziona Template",
                "Annulla",
                null,
                templates);

            if(selectedTemplate is not null and not "Annulla")
            {
                await ApplyTemplate(selectedTemplate);
            }
        }

        private async Task ApplyTemplate(string templateName)
        {
            try
            {
                // Reset delle selezioni correnti
                foreach(DaySelectionItem day in AvailableDays)
                {
                    day.IsSelected = false;
                }

                switch(templateName)
                {
                    case "Principiante Full Body":
                        PlanTitle = "Piano Principiante Full Body";
                        PlanDescription = "Allenamento completo per tutto il corpo, ideale per principianti";
                        AvailableDays.First(d => d.Day == DayOfWeek.Monday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Wednesday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Friday).IsSelected = true;
                        break;

                    case "Push/Pull/Legs Intermedio":
                        PlanTitle = "Piano Push/Pull/Legs";
                        PlanDescription = "Divisione muscolare push/pull/legs per livello intermedio";
                        AvailableDays.First(d => d.Day == DayOfWeek.Monday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Wednesday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Friday).IsSelected = true;
                        break;

                    case "Allenamento a Casa":
                        PlanTitle = "Piano Allenamento a Casa";
                        PlanDescription = "Allenamento completo senza attrezzature";
                        AvailableDays.First(d => d.Day == DayOfWeek.Tuesday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Thursday).IsSelected = true;
                        AvailableDays.First(d => d.Day == DayOfWeek.Saturday).IsSelected = true;
                        break;
                }

                await _dialogService.ShowAlertAsync("Template Applicato", $"Template '{templateName}' applicato con successo!");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'applicazione del template: {ex.Message}");
            }
        }

        #endregion

        #region Public Methods

        public void OnAppearing()
        {
            // Verifica se c'è un esercizio preselezionato
            if(PreselectedExercise != null)
            {
                // Aggiungi l'esercizio al primo giorno di allenamento
                WorkoutDay? firstWorkoutDay = WorkoutDays.FirstOrDefault();
                if(firstWorkoutDay != null && !firstWorkoutDay.Exercises.Any())
                {
                    WorkoutExercise workoutExercise = new()
                    {
                        ExerciseId = PreselectedExercise.Id,
                        Exercise = PreselectedExercise,
                        Sets = 3,
                        Reps = 10,
                        Weight = 0,
                        RestTime = TimeSpan.FromSeconds(60),
                        OrderIndex = 0
                    };

                    firstWorkoutDay.Exercises.Add(workoutExercise);
                }
            }
        }

        #endregion
    }

    // Classe helper per la selezione dei giorni
    public class DaySelectionItem : BaseViewModel
    {
        private bool _isSelected;

        public DayOfWeek Day { get; set; }
        public string DisplayName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}