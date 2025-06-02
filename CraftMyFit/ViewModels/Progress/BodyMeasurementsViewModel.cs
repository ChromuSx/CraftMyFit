using CraftMyFit.Data.Interfaces;
using CraftMyFit.Helpers.Utils;
using CraftMyFit.Models.Progress;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Progress
{
    public class BodyMeasurementsViewModel : BaseViewModel
    {
        private readonly IBodyMeasurementRepository _bodyMeasurementRepository;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPreferenceService _preferenceService;

        private ObservableCollection<BodyMeasurement> _measurements = [];
        private BodyMeasurement? _latestMeasurement;
        private BodyMeasurement? _newMeasurement;
        private bool _isAddingMeasurement;
        private DateTime _selectedDate = DateTime.Today;
        private string _weightUnit = "kg";
        private string _lengthUnit = "cm";

        public BodyMeasurementsViewModel(
            IBodyMeasurementRepository bodyMeasurementRepository,
            INavigationService navigationService,
            IDialogService dialogService,
            IPreferenceService preferenceService)
        {
            _bodyMeasurementRepository = bodyMeasurementRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _preferenceService = preferenceService;

            Title = "Misurazioni Corporee";

            // Inizializza la nuova misurazione
            InitializeNewMeasurement();

            // Carica le unità di misura preferite
            LoadPreferences();

            // Inizializza i comandi
            LoadMeasurementsCommand = new Command(async () => await LoadMeasurements());
            AddMeasurementCommand = new Command(AddMeasurement);
            SaveMeasurementCommand = new Command(async () => await SaveMeasurement());
            CancelAddMeasurementCommand = new Command(CancelAddMeasurement);
            DeleteMeasurementCommand = new Command<BodyMeasurement>(async (measurement) => await DeleteMeasurement(measurement));
            ViewHistoryCommand = new Command(async () => await ViewHistory());
            ExportDataCommand = new Command(async () => await ExportData());
            RefreshCommand = new Command(async () => await RefreshMeasurements());

            // Carica i dati iniziali
            _ = Task.Run(LoadMeasurements);
        }

        #region Properties

        public ObservableCollection<BodyMeasurement> Measurements
        {
            get => _measurements;
            set => SetProperty(ref _measurements, value);
        }

        public BodyMeasurement? LatestMeasurement
        {
            get => _latestMeasurement;
            set => SetProperty(ref _latestMeasurement, value);
        }

        public BodyMeasurement NewMeasurement
        {
            get => _newMeasurement ?? InitializeAndReturnNewMeasurement();
            set => SetProperty(ref _newMeasurement, value);
        }

        public bool IsAddingMeasurement
        {
            get => _isAddingMeasurement;
            set => SetProperty(ref _isAddingMeasurement, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string WeightUnit
        {
            get => _weightUnit;
            set => SetProperty(ref _weightUnit, value);
        }

        public string LengthUnit
        {
            get => _lengthUnit;
            set => SetProperty(ref _lengthUnit, value);
        }

        // Proprietà calcolate
        public bool HasMeasurements => Measurements.Any();
        public string EmptyStateMessage => "Nessuna misurazione registrata";
        public int MeasurementCount => Measurements.Count;

        public string LatestWeightText => LatestMeasurement != null ?
            MeasurementUtils.FormatWeight(LatestMeasurement.Weight, WeightUnit) : "N/A";

        public string BMIText
        {
            get
            {
                if(LatestMeasurement == null)
                {
                    return "N/A";
                }

                // Per calcolare il BMI serve l'altezza, che potrebbe essere nell'utente
                // Per ora mostriamo solo se disponibile
                return "Calcolo BMI disponibile presto";
            }
        }

        public string WeightChangeText
        {
            get
            {
                if(Measurements.Count < 2)
                {
                    return "N/A";
                }

                BodyMeasurement latest = Measurements.First();
                BodyMeasurement previous = Measurements.Skip(1).First();
                float change = latest.Weight - previous.Weight;
                string sign = change >= 0 ? "+" : "";

                return $"{sign}{MeasurementUtils.FormatWeight(change, WeightUnit)}";
            }
        }

        #endregion

        #region Commands

        public ICommand LoadMeasurementsCommand { get; }
        public ICommand AddMeasurementCommand { get; }
        public ICommand SaveMeasurementCommand { get; }
        public ICommand CancelAddMeasurementCommand { get; }
        public ICommand DeleteMeasurementCommand { get; }
        public ICommand ViewHistoryCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Private Methods
        private void InitializeNewMeasurement()
        {
            int currentUserId = _preferenceService.GetInt("current_user_id", 1);
            NewMeasurement = new BodyMeasurement
            {
                Date = DateTime.Now,
                UserId = currentUserId,
                User = new Models.User { Id = currentUserId, Name = "User" }, // Placeholder
                Notes = string.Empty, // Inizializza Notes come stringa vuota
                Weight = 0 // Aggiungi inizializzazione del peso
            };
        }

        private void LoadPreferences()
        {
            WeightUnit = _preferenceService.GetString("preferred_weight_unit", "kg");
            LengthUnit = _preferenceService.GetString("preferred_length_unit", "cm");
        }

        private async Task LoadMeasurements()
        {
            if(IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                int currentUserId = _preferenceService.GetInt("current_user_id", 1);
                List<BodyMeasurement> measurements = await _bodyMeasurementRepository.GetByUserIdAsync(currentUserId);

                Measurements = [.. measurements];
                LatestMeasurement = measurements.FirstOrDefault();
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel caricamento delle misurazioni: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddMeasurement()
        {
            IsAddingMeasurement = true;
            InitializeNewMeasurement();
        }

        private async Task SaveMeasurement()
        {
            try
            {
                if(NewMeasurement.Weight <= 0)
                {
                    await _dialogService.ShowAlertAsync("Errore", "Inserisci un peso valido");
                    return;
                }

                // Valida il peso
                if(!MeasurementUtils.IsValidWeight(NewMeasurement.Weight, WeightUnit))
                {
                    await _dialogService.ShowAlertAsync("Errore", "Il peso inserito non è valido");
                    return;
                }

                // Controlla se esiste già una misurazione per questa data
                int currentUserId = _preferenceService.GetInt("current_user_id", 1);
                BodyMeasurement? existingMeasurement = await _bodyMeasurementRepository.GetByUserIdAndDateAsync(currentUserId, SelectedDate);

                if(existingMeasurement != null)
                {
                    bool overwrite = await _dialogService.ShowConfirmAsync(
                        "Misurazione Esistente",
                        "Esiste già una misurazione per questa data. Vuoi sovrascriverla?",
                        "Sovrascrivi",
                        "Annulla");

                    if(!overwrite)
                    {
                        return;
                    }

                    // Aggiorna la misurazione esistente
                    existingMeasurement.Weight = NewMeasurement.Weight;
                    existingMeasurement.BodyFatPercentage = NewMeasurement.BodyFatPercentage;
                    existingMeasurement.MuscleMass = NewMeasurement.MuscleMass;
                    existingMeasurement.Chest = NewMeasurement.Chest;
                    existingMeasurement.Waist = NewMeasurement.Waist;
                    existingMeasurement.Hips = NewMeasurement.Hips;
                    existingMeasurement.LeftArm = NewMeasurement.LeftArm;
                    existingMeasurement.RightArm = NewMeasurement.RightArm;
                    existingMeasurement.LeftThigh = NewMeasurement.LeftThigh;
                    existingMeasurement.RightThigh = NewMeasurement.RightThigh;
                    existingMeasurement.LeftCalf = NewMeasurement.LeftCalf;
                    existingMeasurement.RightCalf = NewMeasurement.RightCalf;
                    existingMeasurement.Notes = NewMeasurement.Notes;

                    await _bodyMeasurementRepository.UpdateAsync(existingMeasurement);
                }
                else
                {
                    // Crea una nuova misurazione
                    NewMeasurement.Date = SelectedDate;
                    _ = await _bodyMeasurementRepository.AddAsync(NewMeasurement);
                }

                IsAddingMeasurement = false;
                await LoadMeasurements();
                await _dialogService.ShowAlertAsync("Successo", "Misurazione salvata con successo");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio della misurazione: {ex.Message}");
            }
        }

        private void CancelAddMeasurement()
        {
            IsAddingMeasurement = false;
            InitializeNewMeasurement();
        }

        private async Task DeleteMeasurement(BodyMeasurement? measurement)
        {
            if(measurement == null)
            {
                return;
            }

            try
            {
                bool confirmed = await _dialogService.ShowConfirmAsync(
                    "Elimina Misurazione",
                    $"Sei sicuro di voler eliminare la misurazione del {measurement.Date:dd/MM/yyyy}?",
                    "Elimina",
                    "Annulla");

                if(confirmed)
                {
                    await _bodyMeasurementRepository.DeleteAsync(measurement.Id);
                    await LoadMeasurements();
                    await _dialogService.ShowAlertAsync("Successo", "Misurazione eliminata");
                }
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'eliminazione della misurazione: {ex.Message}");
            }
        }

        private async Task ViewHistory() =>
            // TODO: Implementare la visualizzazione dello storico con grafici
            await _dialogService.ShowAlertAsync("In Arrivo", "Visualizzazione storico disponibile presto!");

        private async Task ExportData()
        {
            try
            {
                if(!HasMeasurements)
                {
                    await _dialogService.ShowAlertAsync("Nessun Dato", "Nessuna misurazione da esportare");
                    return;
                }

                // TODO: Implementare l'esportazione in CSV
                await _dialogService.ShowAlertAsync("In Arrivo", "Esportazione dati disponibile presto!");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nell'esportazione: {ex.Message}");
            }
        }

        private async Task RefreshMeasurements() => await LoadMeasurements();

        #endregion

        #region Public Methods

        public string FormatMeasurement(float? value, string unit) => !value.HasValue || value.Value <= 0
                ? "N/A"
                : unit is "kg" or "cm" ?
                $"{value.Value:F1} {unit}" :
                $"{value.Value:F1} {unit}";

        public void OnAppearing() =>
            // Ricarica le preferenze quando la pagina appare
            LoadPreferences();

        private BodyMeasurement InitializeAndReturnNewMeasurement()
        {
            InitializeNewMeasurement();
            return _newMeasurement!;
        }
        #endregion
    }
}