using CraftMyFit.Constants;
using CraftMyFit.Services.Interfaces;
using CraftMyFit.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CraftMyFit.ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IPreferenceService _preferenceService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        // Proprietà delle impostazioni
        private string _userName;
        private string _preferredWeightUnit;
        private string _preferredLengthUnit;
        private bool _enableNotifications;
        private bool _playWorkoutSounds;
        private int _defaultRestTime;
        private string _appTheme;

        public SettingsViewModel(
            IPreferenceService preferenceService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _preferenceService = preferenceService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            Title = "Impostazioni";

            // Inizializza le opzioni disponibili
            InitializeOptions();

            // Carica le impostazioni correnti
            LoadSettings();

            // Inizializza i comandi
            SaveSettingsCommand = new Command(async () => await SaveSettings());
            ResetSettingsCommand = new Command(async () => await ResetSettings());
            ExportDataCommand = new Command(async () => await ExportData());
            ImportDataCommand = new Command(async () => await ImportData());
            AboutCommand = new Command(async () => await ShowAbout());
            ContactSupportCommand = new Command(async () => await ContactSupport());
        }

        #region Properties

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string PreferredWeightUnit
        {
            get => _preferredWeightUnit;
            set => SetProperty(ref _preferredWeightUnit, value);
        }

        public string PreferredLengthUnit
        {
            get => _preferredLengthUnit;
            set => SetProperty(ref _preferredLengthUnit, value);
        }

        public bool EnableNotifications
        {
            get => _enableNotifications;
            set => SetProperty(ref _enableNotifications, value);
        }

        public bool PlayWorkoutSounds
        {
            get => _playWorkoutSounds;
            set => SetProperty(ref _playWorkoutSounds, value);
        }

        public int DefaultRestTime
        {
            get => _defaultRestTime;
            set => SetProperty(ref _defaultRestTime, value);
        }

        public string AppTheme
        {
            get => _appTheme;
            set => SetProperty(ref _appTheme, value);
        }

        // Opzioni disponibili
        public ObservableCollection<string> WeightUnits { get; set; } = [];
        public ObservableCollection<string> LengthUnits { get; set; } = [];
        public ObservableCollection<string> ThemeOptions { get; set; } = [];
        public ObservableCollection<int> RestTimeOptions { get; set; } = [];

        // Informazioni app
        public string AppVersion => AppConstants.AppVersion;
        public string AppName => AppConstants.AppName;

        #endregion

        #region Commands

        public ICommand SaveSettingsCommand { get; }
        public ICommand ResetSettingsCommand { get; }
        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand ContactSupportCommand { get; }

        #endregion

        #region Private Methods

        private void InitializeOptions()
        {
            // Unità di peso
            WeightUnits.Add("kg");
            WeightUnits.Add("lbs");

            // Unità di lunghezza
            LengthUnits.Add("cm");
            LengthUnits.Add("in");

            // Opzioni tema
            ThemeOptions.Add("Chiaro");
            ThemeOptions.Add("Scuro");
            ThemeOptions.Add("Sistema");

            // Opzioni tempo di riposo (in secondi)
            RestTimeOptions.Add(30);
            RestTimeOptions.Add(45);
            RestTimeOptions.Add(60);
            RestTimeOptions.Add(90);
            RestTimeOptions.Add(120);
            RestTimeOptions.Add(180);
        }

        private void LoadSettings()
        {
            UserName = _preferenceService.GetString(PreferenceKeys.UserName, "Utente");
            PreferredWeightUnit = _preferenceService.GetString(PreferenceKeys.PreferredWeightUnit, "kg");
            PreferredLengthUnit = _preferenceService.GetString(PreferenceKeys.PreferredLengthUnit, "cm");
            EnableNotifications = _preferenceService.GetBool(PreferenceKeys.EnableNotifications, true);
            PlayWorkoutSounds = _preferenceService.GetBool(PreferenceKeys.PlaySounds, true);
            DefaultRestTime = _preferenceService.GetInt(PreferenceKeys.DefaultRestTime, AppConstants.DefaultRestTimeSeconds);
            AppTheme = _preferenceService.GetString(PreferenceKeys.Theme, "Sistema");
        }

        private async Task SaveSettings()
        {
            try
            {
                // Salva tutte le impostazioni
                _preferenceService.SetString(PreferenceKeys.UserName, UserName);
                _preferenceService.SetString(PreferenceKeys.PreferredWeightUnit, PreferredWeightUnit);
                _preferenceService.SetString(PreferenceKeys.PreferredLengthUnit, PreferredLengthUnit);
                _preferenceService.SetBool(PreferenceKeys.EnableNotifications, EnableNotifications);
                _preferenceService.SetBool(PreferenceKeys.PlaySounds, PlayWorkoutSounds);
                _preferenceService.SetInt(PreferenceKeys.DefaultRestTime, DefaultRestTime);
                _preferenceService.SetString(PreferenceKeys.Theme, AppTheme);

                await _dialogService.ShowAlertAsync("Successo", "Impostazioni salvate con successo");
            }
            catch(Exception ex)
            {
                await _dialogService.ShowAlertAsync("Errore", $"Errore nel salvataggio delle impostazioni: {ex.Message}");
            }
        }

        private async Task ResetSettings()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "Reset Impostazioni",
                "Sei sicuro di voler ripristinare tutte le impostazioni ai valori predefiniti?",
                "Reset",
                "Annulla");

            if(confirmed)
            {
                try
                {
                    // Resetta le impostazioni ai valori predefiniti
                    _preferenceService.Remove(PreferenceKeys.UserName);
                    _preferenceService.Remove(PreferenceKeys.PreferredWeightUnit);
                    _preferenceService.Remove(PreferenceKeys.PreferredLengthUnit);
                    _preferenceService.Remove(PreferenceKeys.EnableNotifications);
                    _preferenceService.Remove(PreferenceKeys.PlaySounds);
                    _preferenceService.Remove(PreferenceKeys.DefaultRestTime);
                    _preferenceService.Remove(PreferenceKeys.Theme);

                    // Ricarica le impostazioni
                    LoadSettings();

                    await _dialogService.ShowAlertAsync("Successo", "Impostazioni ripristinate ai valori predefiniti");
                }
                catch(Exception ex)
                {
                    await _dialogService.ShowAlertAsync("Errore", $"Errore nel reset delle impostazioni: {ex.Message}");
                }
            }
        }

        private async Task ExportData() =>
            // TODO: Implementare l'esportazione dei dati
            await _dialogService.ShowAlertAsync("In Arrivo", "La funzionalità di esportazione sarà disponibile presto!");

        private async Task ImportData() =>
            // TODO: Implementare l'importazione dei dati
            await _dialogService.ShowAlertAsync("In Arrivo", "La funzionalità di importazione sarà disponibile presto!");

        private async Task ShowAbout()
        {
            string aboutMessage = $"{AppName} v{AppVersion}\n\n" +
                             "La tua app personale per il fitness che ti aiuta a tracciare allenamenti, " +
                             "misurazioni corporee e progressi fotografici.\n\n" +
                             "Sviluppato con ❤️ per aiutarti a raggiungere i tuoi obiettivi di fitness.";

            await _dialogService.ShowAlertAsync("Informazioni", aboutMessage);
        }

        private async Task ContactSupport()
        {
            string[] options = new string[] { "Email", "GitHub", "Sito Web" };

            var choice = await _dialogService.ShowActionSheetAsync(
                "Contatta il Supporto",
                "Annulla",
                null,
                options);

            if(choice is not null and not (object)"Annulla")
            {
                switch(choice)
                {
                    case "Email":
                        // TODO: Aprire il client email
                        await _dialogService.ShowAlertAsync("Email", "support@craftmyfit.com");
                        break;
                    case "GitHub":
                        // TODO: Aprire il browser su GitHub
                        await _dialogService.ShowAlertAsync("GitHub", "https://github.com/craftmyfit");
                        break;
                    case "Sito Web":
                        // TODO: Aprire il browser sul sito web
                        await _dialogService.ShowAlertAsync("Sito Web", "https://craftmyfit.com");
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void OnAppearing() =>
            // Ricarica le impostazioni quando la pagina appare
            LoadSettings();

        #endregion
    }
}