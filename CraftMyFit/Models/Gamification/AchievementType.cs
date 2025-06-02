// AchievementType.cs - Tipi di achievement completi
namespace CraftMyFit.Models.Gamification
{
    /// <summary>
    /// Tipi di achievement disponibili nell'app
    /// </summary>
    public enum AchievementType
    {
        // Achievement legati agli allenamenti
        WorkoutsCompleted,      // Numero totale di allenamenti completati  
        ConsecutiveDays,        // Giorni consecutivi di allenamento
        WorkoutStreak,          // Streak più lunga di allenamenti
        TotalWorkoutTime,       // Tempo totale di allenamento in minuti
        WorkoutFrequency,       // Frequenza di allenamenti settimanali

        // Achievement legati ai progressi fisici
        WeightLost,             // Peso perso in kg
        WeightGained,           // Peso guadagnato in kg (massa muscolare)
        BodyFatReduced,         // Percentuale di grasso corporeo ridotta
        MuscleGained,           // Massa muscolare guadagnata
        MeasurementReduced,     // Riduzione circonferenze corporee

        // Achievement legati alle foto di progresso
        PhotosUploaded,         // Numero di foto di progresso caricate
        PhotoComparison,        // Confronti fotografici completati
        TransformationShared,   // Trasformazioni condivise

        // Achievement legati alla costanza
        WeeklyGoalMet,          // Obiettivi settimanali raggiunti
        MonthlyGoalMet,         // Obiettivi mensili raggiunti
        YearlyGoalMet,          // Obiettivi annuali raggiunti
        PerfectWeek,            // Settimana perfetta (tutti gli allenamenti)
        PerfectMonth,           // Mese perfetto

        // Achievement legati agli esercizi specifici
        PushUpsMaster,          // Padronanza nelle flessioni
        SquatChampion,          // Campione degli squat
        PlankWarrior,           // Guerriero del plank
        CardioKing,             // Re del cardio
        StrengthGuru,           // Guru della forza

        // Achievement sociali e condivisione
        FirstShare,             // Prima condivisione
        SocialInfluencer,       // Influencer social (condivisioni multiple)
        CommunityMember,        // Membro attivo della community
        Motivator,              // Motivatore (aiuta altri utenti)

        // Achievement speciali e milestone
        FirstWorkout,           // Primo allenamento completato
        FirstMonth,             // Primo mese di utilizzo app
        FirstYear,              // Primo anno di utilizzo app
        CustomGoalReached,      // Obiettivo personalizzato raggiunto
        PersonalRecord,         // Record personale battuto

        // Achievement stagionali/eventi
        NewYearChallenge,       // Sfida di Capodanno
        SummerBody,             // Corpo estivo
        HalloweenHorror,        // Sfida Halloween
        ChristmasChallenge,     // Sfida natalizia
        SpringCleaning,         // Pulizie di primavera

        // Achievement tecnici/app
        ProfileCompleted,       // Profilo completato al 100%
        AllFeatures,            // Utilizzate tutte le funzionalità
        DataBackup,             // Backup dati completato
        WearableConnected,      // Dispositivo wearable connesso
        AppRated              // App valutata nello store
    }

    /// <summary>
    /// Estensioni per AchievementType
    /// </summary>
    public static class AchievementTypeExtensions
    {
        /// <summary>
        /// Ottiene la descrizione localizzata del tipo di achievement
        /// </summary>
        public static string GetDescription(this AchievementType type) => type switch
        {
            AchievementType.WorkoutsCompleted => "Allenamenti Completati",
            AchievementType.ConsecutiveDays => "Giorni Consecutivi",
            AchievementType.WorkoutStreak => "Streak di Allenamenti",
            AchievementType.TotalWorkoutTime => "Tempo Totale Allenamento",
            AchievementType.WorkoutFrequency => "Frequenza Allenamenti",
            AchievementType.WeightLost => "Peso Perso",
            AchievementType.WeightGained => "Peso Guadagnato",
            AchievementType.BodyFatReduced => "Grasso Corporeo Ridotto",
            AchievementType.MuscleGained => "Massa Muscolare Guadagnata",
            AchievementType.MeasurementReduced => "Circonferenze Ridotte",
            AchievementType.PhotosUploaded => "Foto Caricate",
            AchievementType.PhotoComparison => "Confronti Fotografici",
            AchievementType.TransformationShared => "Trasformazioni Condivise",
            AchievementType.WeeklyGoalMet => "Obiettivi Settimanali",
            AchievementType.MonthlyGoalMet => "Obiettivi Mensili",
            AchievementType.YearlyGoalMet => "Obiettivi Annuali",
            AchievementType.PerfectWeek => "Settimana Perfetta",
            AchievementType.PerfectMonth => "Mese Perfetto",
            AchievementType.PushUpsMaster => "Maestro delle Flessioni",
            AchievementType.SquatChampion => "Campione degli Squat",
            AchievementType.PlankWarrior => "Guerriero del Plank",
            AchievementType.CardioKing => "Re del Cardio",
            AchievementType.StrengthGuru => "Guru della Forza",
            AchievementType.FirstShare => "Prima Condivisione",
            AchievementType.SocialInfluencer => "Influencer Social",
            AchievementType.CommunityMember => "Membro Community",
            AchievementType.Motivator => "Motivatore",
            AchievementType.FirstWorkout => "Primo Allenamento",
            AchievementType.FirstMonth => "Primo Mese",
            AchievementType.FirstYear => "Primo Anno",
            AchievementType.CustomGoalReached => "Obiettivo Personalizzato",
            AchievementType.PersonalRecord => "Record Personale",
            AchievementType.NewYearChallenge => "Sfida di Capodanno",
            AchievementType.SummerBody => "Corpo Estivo",
            AchievementType.HalloweenHorror => "Sfida Halloween",
            AchievementType.ChristmasChallenge => "Sfida Natalizia",
            AchievementType.SpringCleaning => "Pulizie di Primavera",
            AchievementType.ProfileCompleted => "Profilo Completato",
            AchievementType.AllFeatures => "Tutte le Funzionalità",
            AchievementType.DataBackup => "Backup Dati",
            AchievementType.WearableConnected => "Wearable Connesso",
            AchievementType.AppRated => "App Valutata",
            _ => type.ToString()
        };

        /// <summary>
        /// Ottiene l'icona associata al tipo di achievement
        /// </summary>
        public static string GetIcon(this AchievementType type) => type switch
        {
            AchievementType.WorkoutsCompleted => "💪",
            AchievementType.ConsecutiveDays => "🔥",
            AchievementType.WorkoutStreak => "⚡",
            AchievementType.TotalWorkoutTime => "⏱️",
            AchievementType.WorkoutFrequency => "📊",
            AchievementType.WeightLost => "⚖️",
            AchievementType.WeightGained => "🏋️",
            AchievementType.BodyFatReduced => "📉",
            AchievementType.MuscleGained => "💪",
            AchievementType.MeasurementReduced => "📏",
            AchievementType.PhotosUploaded => "📸",
            AchievementType.PhotoComparison => "🔄",
            AchievementType.TransformationShared => "📤",
            AchievementType.WeeklyGoalMet => "🎯",
            AchievementType.MonthlyGoalMet => "🏆",
            AchievementType.YearlyGoalMet => "👑",
            AchievementType.PerfectWeek => "⭐",
            AchievementType.PerfectMonth => "🌟",
            AchievementType.PushUpsMaster => "🤲",
            AchievementType.SquatChampion => "🦵",
            AchievementType.PlankWarrior => "🪨",
            AchievementType.CardioKing => "❤️",
            AchievementType.StrengthGuru => "🏋️‍♂️",
            AchievementType.FirstShare => "🤝",
            AchievementType.SocialInfluencer => "📱",
            AchievementType.CommunityMember => "👥",
            AchievementType.Motivator => "🎉",
            AchievementType.FirstWorkout => "🎊",
            AchievementType.FirstMonth => "📅",
            AchievementType.FirstYear => "🎂",
            AchievementType.CustomGoalReached => "🎖️",
            AchievementType.PersonalRecord => "🥇",
            AchievementType.NewYearChallenge => "🎆",
            AchievementType.SummerBody => "☀️",
            AchievementType.HalloweenHorror => "🎃",
            AchievementType.ChristmasChallenge => "🎄",
            AchievementType.SpringCleaning => "🌸",
            AchievementType.ProfileCompleted => "✅",
            AchievementType.AllFeatures => "🚀",
            AchievementType.DataBackup => "💾",
            AchievementType.WearableConnected => "⌚",
            AchievementType.AppRated => "⭐",
            _ => "🏅"
        };

        /// <summary>
        /// Ottiene il colore associato al tipo di achievement
        /// </summary>
        public static string GetColor(this AchievementType type) => type switch
        {
            // Allenamenti - Blu
            AchievementType.WorkoutsCompleted or
            AchievementType.ConsecutiveDays or
            AchievementType.WorkoutStreak => "#2196F3",

            // Progressi fisici - Verde
            AchievementType.WeightLost or
            AchievementType.BodyFatReduced or
            AchievementType.MeasurementReduced => "#4CAF50",

            // Forza/Muscoli - Rosso
            AchievementType.WeightGained or
            AchievementType.MuscleGained or
            AchievementType.StrengthGuru => "#F44336",

            // Foto/Social - Viola
            AchievementType.PhotosUploaded or
            AchievementType.TransformationShared or
            AchievementType.SocialInfluencer => "#9C27B0",

            // Obiettivi - Arancione
            AchievementType.WeeklyGoalMet or
            AchievementType.MonthlyGoalMet or
            AchievementType.CustomGoalReached => "#FF9800",

            // Speciali/Milestone - Oro
            AchievementType.FirstWorkout or
            AchievementType.FirstYear or
            AchievementType.PersonalRecord => "#FFD700",

            // Stagionali - Variegati
            AchievementType.NewYearChallenge => "#FF5722",
            AchievementType.SummerBody => "#FFC107",
            AchievementType.HalloweenHorror => "#FF5722",
            AchievementType.ChristmasChallenge => "#4CAF50",
            AchievementType.SpringCleaning => "#8BC34A",

            // Default
            _ => "#607D8B"
        };

        /// <summary>
        /// Verifica se l'achievement è stagionale
        /// </summary>
        public static bool IsSeasonal(this AchievementType type) => type switch
        {
            AchievementType.NewYearChallenge or
            AchievementType.SummerBody or
            AchievementType.HalloweenHorror or
            AchievementType.ChristmasChallenge or
            AchievementType.SpringCleaning => true,
            _ => false
        };

        /// <summary>
        /// Ottiene il periodo di validità per achievement stagionali
        /// </summary>
        public static (DateTime Start, DateTime End)? GetSeasonalPeriod(this AchievementType type, int year) => type switch
        {
            AchievementType.NewYearChallenge => (new DateTime(year, 1, 1), new DateTime(year, 1, 31)),
            AchievementType.SpringCleaning => (new DateTime(year, 3, 20), new DateTime(year, 6, 20)),
            AchievementType.SummerBody => (new DateTime(year, 6, 1), new DateTime(year, 8, 31)),
            AchievementType.HalloweenHorror => (new DateTime(year, 10, 1), new DateTime(year, 10, 31)),
            AchievementType.ChristmasChallenge => (new DateTime(year, 12, 1), new DateTime(year, 12, 31)),
            _ => null
        };
    }
}