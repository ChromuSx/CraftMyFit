namespace CraftMyFit.Constants
{
    public static class AppConstants
    {
        // App Info
        public const string AppName = "CraftMyFit";
        public const string AppVersion = "1.0.0";
        public const string AppDescription = "La tua app personale per il fitness";

        // Database
        public const string DatabaseFilename = "craftmyfit.db";
        public const int DatabaseVersion = 1;

        // File Paths
        public const string ImagesFolder = "Images";
        public const string ExerciseImagesFolder = "Images/exercises";
        public const string BadgeImagesFolder = "Images/badges";
        public const string ProgressPhotosFolder = "ProgressPhotos";
        public const string BackupsFolder = "Backups";

        // Defaults
        public const int DefaultRestTimeSeconds = 60;
        public const int DefaultSets = 3;
        public const int DefaultReps = 10;
        public const float DefaultWeight = 0f;

        // Limits
        public const int MaxSets = 20;
        public const int MinSets = 1;
        public const int MaxReps = 100;
        public const int MinReps = 1;
        public const float MaxWeight = 500f;
        public const float MinWeight = 0f;
        public const int MaxRestTimeSeconds = 600; // 10 minuti
        public const int MinRestTimeSeconds = 10;

        // Units
        public static class Units
        {
            public const string Kilograms = "kg";
            public const string Pounds = "lbs";
            public const string Centimeters = "cm";
            public const string Inches = "in";
            public const string Seconds = "s";
            public const string Minutes = "min";
        }

        // Achievement Thresholds
        public static class Achievements
        {
            public const int FirstWorkout = 1;
            public const int FiveWorkouts = 5;
            public const int TenWorkouts = 10;
            public const int TwentyFiveWorkouts = 25;
            public const int FiftyWorkouts = 50;
            public const int HundredWorkouts = 100;

            public const int FirstProgressPhoto = 1;
            public const int FiveProgressPhotos = 5;
            public const int TenProgressPhotos = 10;

            public const int OneDayStreak = 1;
            public const int SevenDayStreak = 7;
            public const int ThirtyDayStreak = 30;
            public const int HundredDayStreak = 100;
        }

        // Image Settings
        public const int MaxImageWidth = 1920;
        public const int MaxImageHeight = 1080;
        public const int ThumbnailWidth = 150;
        public const int ThumbnailHeight = 150;
        public const int ImageQuality = 85; // JPEG quality percentage

        // Animation Durations (in milliseconds)
        public const uint ShortAnimation = 200;
        public const uint MediumAnimation = 400;
        public const uint LongAnimation = 600;

        // Colors (for potential theming)
        public static class Colors
        {
            public const string Primary = "#512BD4";
            public const string Secondary = "#DFD8F7";
            public const string Success = "#28A745";
            public const string Warning = "#FFC107";
            public const string Danger = "#DC3545";
            public const string Info = "#17A2B8";
        }

        // Muscle Groups
        public static readonly string[] MuscleGroups =
        {
            "Pettorali",
            "Schiena",
            "Spalle",
            "Bicipiti",
            "Tricipiti",
            "Addominali",
            "Gambe",
            "Glutei",
            "Polpacci",
            "Avambracci",
            "Full Body"
        };

        // Equipment Types
        public static readonly string[] EquipmentTypes =
        {
            "Manubri",
            "Bilanciere",
            "Kettlebell",
            "Bande elastiche",
            "Panca",
            "Pull-up bar",
            "Tappetino",
            "Palla medica",
            "TRX",
            "Macchina per cavi"
        };

        // Default Workout Plans Templates
        public static class WorkoutTemplates
        {
            public const string BeginnerFullBody = "Beginner Full Body";
            public const string IntermediatePPL = "Intermediate Push/Pull/Legs";
            public const string AdvancedBodybuilding = "Advanced Bodybuilding Split";
            public const string HomeWorkout = "Home Workout (No Equipment)";
            public const string HIIT = "High Intensity Interval Training";
        }
    }
}