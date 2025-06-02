using CraftMyFit.Data.Interfaces;
using CraftMyFit.Helpers.Utils;
using CraftMyFit.Models.Workout;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Exercises
{
    [QueryProperty(nameof(Exercise), "Exercise")]
    [QueryProperty(nameof(IsEdit), "IsEdit")]
    public class ExerciseFormViewModel : BaseViewModel
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private Exercise? _originalExercise;
        private string _exerciseName = string.Empty;
        private string _exerciseDescription = string.Empty;
        private string _selectedMuscleGroup = string.Empty;
        private ObservableCollection<string> _selectedEquipment = [];
        private bool _isEdit;
        private bool _isSaving;
        private bool _hasChanges;

        public ExerciseFormViewModel(
            IExerciseRepository exerciseRepository,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _exerciseRepository = exerciseRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;

            Title = "Nuovo Esercizio";

            // Inizializza le collezioni
            InitializeCollections();

            // Inizializza i comandi
            SaveExerciseCommand = new Command(async () => await SaveExercise(), CanSaveExercise);
            CancelCommand = new Command(async () => await Cancel());
            AddEquipmentCommand = new Command(async () => await AddEquipment());
            RemoveEquipmentCommand = new Command<string>(async (equipment) => await RemoveEquipment(equipment));
            SelectMuscleGroupCommand = new Command(async () => await SelectMuscleGroup());
        }

        #region Properties

        public Exercise? Exercise
        {
            get => _originalExercise;
            set
            {
                if(SetProperty(ref _originalExercise, value))
                {
                    OnExerciseChanged();
                }
            }
        }

        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if(SetProperty(ref _isEdit, value))
                {
                    Title = _isEdit ? "Modifica Esercizio" : "Nuovo Esercizio";
                }
            }
        }

        public string ExerciseName
        {
            get => _exerciseName;
            set
            {
                if(SetProperty(ref _exerciseName, value))
                {
                    CheckForChanges();
                    ((Command)SaveExerciseCommand).ChangeCanExecute();
                }
            }
        }

        public string ExerciseDescription
        {
            get => _exerciseDescription;
            set
            {
                if(SetProperty(ref _exerciseDescription, value))
                {
                    CheckForChanges();
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
                    CheckForChanges();
                    ((Command)SaveExerciseCommand).ChangeCanExecute();
                }
            }
        }

        public ObservableCollection<string> SelectedEquipment
        {
            get => _selectedEquipment;
            set => SetProperty(ref _selectedEquipment, value);
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

        // Collezioni di opzioni disponibili
        public ObservableCollection<string> AvailableMuscleGroups { get; set; } = [];
        public ObservableCollection<string> AvailableEquipment { get; set; } = [];

        // Proprietà calcolate
        public string SaveButtonText => IsSaving ? "Salvataggio..." : (IsEdit ? "Salva Modifiche" : "Crea Esercizio");
        public string PageTitle => IsEdit ? "Modifica Esercizio" : "Nuovo Esercizio";
        public bool HasSelectedEquipment => SelectedEquipment.Any();
        public string EquipmentListText => HasSelectedEquipment ? string.Join(", ", SelectedEquipment) : "Nessuna attrezzatura richiesta";

        #endregion

        #region Commands

        public ICommand SaveExerciseCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddEquipmentCommand { get; }
        public ICommand RemoveEquipmentCommand { get; }
        public ICommand SelectMuscleGroupCommand { get; }

        #endregion

        #region Private Methods

        private void InitializeCollections()
        {
            // Gruppi muscolari
            string[] muscleGroups = new[]
            {
                "Pettorali", "Schiena", "Spalle", "Bicipiti", "Tricipiti",
                "Addominali", "Gambe", "Glutei", "Polpacci", "Avambracci", "Full Body"
            };

            foreach(string? group in muscleGroups)
            {
                AvailableMuscleGroups.Add(group);
            }

            // Attrezzature disponibili
            string[] equipment = new[]
            {
                "Manubri", "Bilanciere", "Kettlebell", "Bande elastiche",
                "Panca", "Pull-up bar", "Tappetino", "Palla medica", "TRX", "Macchina per cavi"
            };

            foreach(string? item in equipment)
            {
                AvailableEquipment.Add(item);
            }
        }

        private void OnExerciseChanged()
        {
            if(Exercise == null)
            {
                return;
            }

            // Popola i campi con i dati dell'esercizio esistente
            ExerciseName = Exercise.Name;
            ExerciseDescription = Exercise.Description;
            SelectedMuscleGroup = Exercise.MuscleGroup;
            SelectedEquipment = [.. Exercise.RequiredEquipment];

            Title = $"Modifica: {Exercise.Name}";
            HasChanges = false;
        }

        private void CheckForChanges()
        {
            if(Exercise == null && IsEdit)
            {
                return;
            }

            HasChanges = !IsEdit
                ? !string.IsNullOrWhiteSpace(ExerciseName) ||
                           !string.IsNullOrWhiteSpace(ExerciseDescription) ||
                           !string.IsNullOrWhiteSpace(SelectedMuscleGroup) ||
                           SelectedEquipment.Any()
                : ExerciseName != Exercise?.Name ||
                           ExerciseDescription != Exercise?.Description ||
                           SelectedMuscleGroup != Exercise?.MuscleGroup ||
                           !SelectedEquipment.SequenceEqual(Exercise?.RequiredEquipment ?? []);
        }

        private bool CanSaveExercise() => !string.IsNullOrWhiteSpace(ExerciseName) &&
                   !string.IsNullOrWhiteSpace(SelectedMuscleGroup) &&
                   !IsSaving;

        private async Task SaveExercise()
        {
            if(!CanSaveExercise())
            {
                return;
            }

            try
            {
                IsSaving = true;

                // Valida i dati
                ValidationResult nameValidation = ValidationHelper.ValidateWorkoutPlanTitle(ExerciseName); // Riusa la validazione del titolo
                ValidationResult descriptionValidation = ValidationHelper.ValidateDescription(ExerciseDescription, 1000);

                ValidationResult combinedValidation = ValidationHelper.Combine(nameValidation, descriptionValidation);
                if(!combinedValidation.IsValid)
                {
                    await _dialogService.ShowAlertAsync("Errore di Validazione", combinedValidation.GetErrorsAsString());
                    return;
                }

                // Verifica se il nome esiste già (per nuovi esercizi)
                if(!IsEdit)
                {
                    bool existingExercise = await _exerciseRepository.ExerciseExistsAsync(ExerciseName);
                    if(existingExercise)
                    {
                        await _dialogService.ShowAlertAsync("Esercizio Esistente", "Esiste già un esercizio con questo nome.");
                        return;
                    }
                }

                if(IsEdit && Exercise != null)
                {
                    // Aggiorna esercizio esistente
                    Exercise.Name = ExerciseName.Trim();
                    Exercise.Description = ExerciseDescription?.Trim() ?? string.Empty;
                    Exercise.MuscleGroup = SelectedMuscleGroup;
                    Exercise.RequiredEquipment = SelectedEquipment.ToList();

                    await _exerciseRepository.UpdateAsync(Exercise);
                    await _dialogService.ShowAlertAsync("Successo", "Esercizio aggiornato con successo!");
                }
                else
                {
                    Exercise newExercise = new()
                    {
                        Name = ExerciseName.Trim(),
                        Description = ExerciseDescription?.Trim() ?? string.Empty,
                        MuscleGroup = SelectedMuscleGroup,
                        RequiredEquipment = SelectedEquipment.ToList(),
                        RequiredEquipmentJson = System.Text.Json.JsonSerializer.Serialize(SelectedEquipment.ToList()), // Inizializza il JSON
                        ImagePath = string.Empty,
                        VideoPath = string.Empty
                    };

                    _ = await _exerciseRepository.AddAsync(newExercise);
                    await _dialogService.ShowAlertAsync("Successo", "Esercizio creato con successo!");
                }

                HasChanges = false;
                await _navigationService.GoBackAsync();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio dell'esercizio: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private async Task Cancel()
        {
            if(HasChanges)
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Annulla Modifiche",
                    "Sei sicuro di voler annullare? Le modifiche andranno perse.",
                    "Annulla Modifiche",
                    "Continua");

                if(!confirmed)
                {
                    return;
                }
            }

            await _navigationService.GoBackAsync();
        }

        private async Task SelectMuscleGroup()
        {
            string? selectedGroup = await _dialogService.ShowActionSheetAsync(
                "Seleziona Gruppo Muscolare",
                "Annulla",
                null,
                AvailableMuscleGroups.ToArray());

            if(selectedGroup is not null and not "Annulla")
            {
                SelectedMuscleGroup = selectedGroup;
            }
        }

        private async Task AddEquipment()
        {
            string[] availableItems = AvailableEquipment.Where(e => !SelectedEquipment.Contains(e)).ToArray();

            if(!availableItems.Any())
            {
                await _dialogService.ShowAlertAsync("Nessuna Attrezzatura", "Hai già selezionato tutta l'attrezzatura disponibile.");
                return;
            }

            string? selectedItem = await _dialogService.ShowActionSheetAsync(
                "Aggiungi Attrezzatura",
                "Annulla",
                null,
                availableItems);

            if(selectedItem is not null and not "Annulla")
            {
                SelectedEquipment.Add(selectedItem);
                CheckForChanges();
            }
        }

        private async Task RemoveEquipment(string? equipment)
        {
            if(string.IsNullOrEmpty(equipment))
            {
                return;
            }

            bool confirmed = await _dialogService.ShowConfirmAsync(
                "Rimuovi Attrezzatura",
                $"Vuoi rimuovere '{equipment}' dall'elenco?",
                "Rimuovi",
                "Annulla");

            if(confirmed)
            {
                _ = SelectedEquipment.Remove(equipment);
                CheckForChanges();
            }
        }

        #endregion

        #region Public Methods

        public void OnAppearing() =>
            // Aggiorna il check dei cambiamenti quando la pagina appare
            CheckForChanges();

        #endregion
    }
}