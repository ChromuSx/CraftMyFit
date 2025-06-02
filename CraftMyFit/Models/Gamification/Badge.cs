// Badge.cs - Modello per i badge di gamification
using CraftMyFit.Models.Gamification;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace CraftMyFit.Models.Gamification
{
    public class Badge
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        public required string Description { get; set; }
        public required string IconPath { get; set; }
        public required string Color { get; set; } // Hex color per il badge

        public BadgeCategory Category { get; set; }
        public BadgeRarity Rarity { get; set; }
        public int PointsRequired { get; set; }

        // Condizioni per ottenere il badge
        public BadgeConditionType ConditionType { get; set; }
        public int ConditionValue { get; set; }
        public required string ConditionDescription { get; set; }

        // Relazioni
        public int UserId { get; set; }
        public required User User { get; set; }

        public DateTime? EarnedDate { get; set; }  // null se non ancora ottenuto
        public bool IsVisible { get; set; } = true; // Se il badge deve essere mostrato anche se non ottenuto

        // Proprietà calcolate
        [Ignore]
        public bool IsEarned => EarnedDate.HasValue;

        [Ignore]
        public string DisplayName => IsEarned ? Name : $"??? {Name}";

        [Ignore]
        public string DisplayDescription => IsEarned ? Description : "Badge nascosto - completa le condizioni per sbloccarlo";
    }

    public enum BadgeCategory
    {
        Workout,      // Badge legati agli allenamenti
        Progress,     // Badge legati ai progressi fisici
        Consistency,  // Badge legati alla costanza
        Achievement,  // Badge per achievement specifici
        Social,       // Badge per condivisioni e social
        Special       // Badge speciali per eventi o ricorrenze
    }

    public enum BadgeRarity
    {
        Common,       // Badge comuni, facili da ottenere
        Uncommon,     // Badge non comuni
        Rare,         // Badge rari
        Epic,         // Badge epici
        Legendary     // Badge leggendari, molto difficili
    }

    public enum BadgeConditionType
    {
        WorkoutsCompleted,     // Numero di allenamenti completati
        ConsecutiveDays,       // Giorni consecutivi di allenamento
        WeightLost,           // Peso perso in kg
        PhotosUploaded,       // Foto di progresso caricate
        TimeSpentExercising,  // Tempo totale di esercizio in minuti
        SpecificExercise,     // Esercizio specifico completato N volte
        MilestoneReached,     // Traguardo specifico raggiunto
        SeasonalEvent         // Evento stagionale o speciale
    }
}

// DatabaseVersionService.cs - Servizio per gestire le versioni del database
using CraftMyFit.Data;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Migrations
{
    public class DatabaseVersionService
    {
        private readonly CraftMyFitDbContext _context;
        private const int CurrentDatabaseVersion = 1;

        public DatabaseVersionService(CraftMyFitDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se è necessario aggiornare il database
        /// </summary>
        public async Task<bool> NeedsUpdateAsync()
        {
            try
            {
                var currentVersion = await GetCurrentVersionAsync();
                return currentVersion < CurrentDatabaseVersion;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo versione database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ottiene la versione corrente del database
        /// </summary>
        public async Task<int> GetCurrentVersionAsync()
        {
            try
            {
                // Verifica se la tabella delle versioni esiste
                var tableExists = await _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='DatabaseVersion'")
                    .FirstOrDefaultAsync();

                if(tableExists == 0)
                {
                    // Prima installazione
                    return 0;
                }

                // Ottieni l'ultima versione
                var version = await _context.Database.SqlQueryRaw<int>(
                    "SELECT Version FROM DatabaseVersion ORDER BY Id DESC LIMIT 1")
                    .FirstOrDefaultAsync();

                return version;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel recupero versione: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Esegue le migrazioni necessarie
        /// </summary>
        public async Task<bool> MigrateAsync()
        {
            try
            {
                var currentVersion = await GetCurrentVersionAsync();

                // Esegui le migrazioni in sequenza
                for(int version = currentVersion + 1; version <= CurrentDatabaseVersion; version++)
                {
                    bool success = await ExecuteMigrationAsync(version);
                    if(!success)
                    {
                        System.Diagnostics.Debug.WriteLine($"Migrazione fallita alla versione {version}");
                        return false;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nelle migrazioni: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Crea la struttura iniziale del database
        /// </summary>
        public async Task<bool> CreateInitialStructureAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();

                // Crea la tabella delle versioni se non esiste
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE IF NOT EXISTS DatabaseVersion (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Version INTEGER NOT NULL,
                        AppliedDate DATETIME NOT NULL,
                        Description TEXT
                    )");

                // Inserisci la versione iniziale
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO DatabaseVersion (Version, AppliedDate, Description) 
                    VALUES (1, datetime('now'), 'Initial database structure')");

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella creazione struttura iniziale: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ExecuteMigrationAsync(int version)
        {
            try
            {
                switch(version)
                {
                    case 1:
                        // Migrazione alla versione 1 - struttura iniziale
                        await CreateInitialStructureAsync();
                        break;

                    // Aggiungi qui le future migrazioni
                    // case 2:
                    //     await MigrateToVersion2Async();
                    //     break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"Migrazione non definita per la versione {version}");
                        return false;
                }

                // Registra la migrazione completata
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO DatabaseVersion (Version, AppliedDate, Description) 
                    VALUES ({0}, datetime('now'), {1})",
                    version, $"Migration to version {version}");

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella migrazione versione {version}: {ex.Message}");
                return false;
            }
        }
    }
}

// WorkoutTemplateService.cs - Servizio per gestire i template di allenamento
using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Workout;
using System.Text.Json;

namespace CraftMyFit.Services
{
    public class WorkoutTemplateService
    {
        private readonly IWorkoutPlanRepository _workoutPlanRepository;
        private readonly IExerciseRepository _exerciseRepository;

        public WorkoutTemplateService(
            IWorkoutPlanRepository workoutPlanRepository,
            IExerciseRepository exerciseRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
            _exerciseRepository = exerciseRepository;
        }

        /// <summary>
        /// Crea un piano di allenamento per principianti full body
        /// </summary>
        public async Task<WorkoutPlan> CreateBeginnerFullBodyPlanAsync(int userId)
        {
            var exercises = await _exerciseRepository.GetAllAsync();
            var basicExercises = exercises.Where(e =>
                e.RequiredEquipmentJson == "[]" || string.IsNullOrEmpty(e.RequiredEquipmentJson))
                .ToList();

            var plan = new WorkoutPlan
            {
                Title = "Principiante Full Body",
                Description = "Piano di allenamento per principianti che allena tutto il corpo",
                UserId = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                WorkoutDaysEnum = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                WorkoutDays = new List<WorkoutDay>()
            };

            // Crea i giorni di allenamento
            var workoutDay = new WorkoutDay
            {
                DayOfWeek = DayOfWeek.Monday,
                Title = "Full Body Workout",
                OrderIndex = 0,
                WorkoutPlan = plan,
                Exercises = new List<WorkoutExercise>()
            };

            // Aggiungi esercizi base
            var pushUpExercise = basicExercises.FirstOrDefault(e => e.Name.Contains("Push-up"));
            if(pushUpExercise != null)
            {
                workoutDay.Exercises.Add(new WorkoutExercise
                {
                    ExerciseId = pushUpExercise.Id,
                    Exercise = pushUpExercise,
                    Sets = 3,
                    Reps = 10,
                    Weight = 0,
                    RestTime = TimeSpan.FromSeconds(60),
                    OrderIndex = 0,
                    WorkoutDay = workoutDay
                });
            }

            var squatExercise = basicExercises.FirstOrDefault(e => e.Name.Contains("Squat"));
            if(squatExercise != null)
            {
                workoutDay.Exercises.Add(new WorkoutExercise
                {
                    ExerciseId = squatExercise.Id,
                    Exercise = squatExercise,
                    Sets = 3,
                    Reps = 15,
                    Weight = 0,
                    RestTime = TimeSpan.FromSeconds(60),
                    OrderIndex = 1,
                    WorkoutDay = workoutDay
                });
            }

            plan.WorkoutDays.Add(workoutDay);
            return plan;
        }

        /// <summary>
        /// Crea un piano HIIT ad alta intensità
        /// </summary>
        public async Task<WorkoutPlan> CreateHIITPlanAsync(int userId)
        {
            var exercises = await _exerciseRepository.GetAllAsync();
            var hiitExercises = exercises.Where(e =>
                e.MuscleGroup == "Full Body" ||
                e.Name.Contains("Burpee") ||
                e.Name.Contains("Jump") ||
                e.Name.Contains("Sprint"))
                .ToList();

            var plan = new WorkoutPlan
            {
                Title = "HIIT Intenso",
                Description = "Allenamento ad alta intensità per bruciare calorie",
                UserId = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                WorkoutDaysEnum = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday },
                WorkoutDays = new List<WorkoutDay>()
            };

            var workoutDay = new WorkoutDay
            {
                DayOfWeek = DayOfWeek.Tuesday,
                Title = "HIIT Circuit",
                OrderIndex = 0,
                WorkoutPlan = plan,
                Exercises = new List<WorkoutExercise>()
            };

            // Aggiungi esercizi HIIT
            int orderIndex = 0;
            foreach(var exercise in hiitExercises.Take(5))
            {
                workoutDay.Exercises.Add(new WorkoutExercise
                {
                    ExerciseId = exercise.Id,
                    Exercise = exercise,
                    Sets = 4,
                    Reps = 0, // HIIT basato sul tempo
                    Weight = 0,
                    Duration = TimeSpan.FromSeconds(45),
                    RestTime = TimeSpan.FromSeconds(15),
                    OrderIndex = orderIndex++,
                    WorkoutDay = workoutDay
                });
            }

            plan.WorkoutDays.Add(workoutDay);
            return plan;
        }

        /// <summary>
        /// Ottiene tutti i template disponibili
        /// </summary>
        public List<WorkoutTemplate> GetAvailableTemplates()
        {
            return new List<WorkoutTemplate>
            {
                new WorkoutTemplate
                {
                    Id = "beginner_fullbody",
                    Name = "Principiante Full Body",
                    Description = "Perfetto per chi inizia, allena tutto il corpo 3 volte a settimana",
                    Difficulty = WorkoutDifficulty.Beginner,
                    EstimatedDuration = TimeSpan.FromMinutes(45),
                    RequiredEquipment = new List<string>(),
                    TargetMuscleGroups = new List<string> { "Full Body" }
                },
                new WorkoutTemplate
                {
                    Id = "hiit_intense",
                    Name = "HIIT Intenso",
                    Description = "Allenamento ad alta intensità per bruciare calorie velocemente",
                    Difficulty = WorkoutDifficulty.Intermediate,
                    EstimatedDuration = TimeSpan.FromMinutes(30),
                    RequiredEquipment = new List<string>(),
                    TargetMuscleGroups = new List<string> { "Full Body", "Cardio" }
                },
                new WorkoutTemplate
                {
                    Id = "push_pull_legs",
                    Name = "Push/Pull/Legs",
                    Description = "Divisione muscolare classica per livello intermedio-avanzato",
                    Difficulty = WorkoutDifficulty.Intermediate,
                    EstimatedDuration = TimeSpan.FromMinutes(60),
                    RequiredEquipment = new List<string> { "Manubri", "Bilanciere" },
                    TargetMuscleGroups = new List<string> { "Pettorali", "Schiena", "Spalle", "Bicipiti", "Tricipiti", "Gambe" }
                },
                new WorkoutTemplate
                {
                    Id = "home_bodyweight",
                    Name = "Allenamento a Casa",
                    Description = "Allenamento completo senza attrezzature",
                    Difficulty = WorkoutDifficulty.Beginner,
                    EstimatedDuration = TimeSpan.FromMinutes(40),
                    RequiredEquipment = new List<string>(),
                    TargetMuscleGroups = new List<string> { "Full Body" }
                }
            };
        }
    }

    // Modelli per i template
    public class WorkoutTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkoutDifficulty Difficulty { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public List<string> RequiredEquipment { get; set; } = new();
        public List<string> TargetMuscleGroups { get; set; } = new();
        public string ImagePath { get; set; } = string.Empty;
    }

    public enum WorkoutDifficulty
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}

// BadgeService.cs - Servizio per gestire i badge
using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Gamification;

namespace CraftMyFit.Services
{
    public class BadgeService
    {
        private readonly IAchievementRepository _achievementRepository;

        public BadgeService(IAchievementRepository achievementRepository)
        {
            _achievementRepository = achievementRepository;
        }

        /// <summary>
        /// Verifica e sblocca badge in base ai progressi
        /// </summary>
        public async Task<List<Badge>> CheckAndUnlockBadgesAsync(int userId, BadgeConditionType conditionType, int currentValue)
        {
            var unlockedBadges = new List<Badge>();

            try
            {
                var availableBadges = GetAvailableBadges(userId, conditionType);

                foreach(var badge in availableBadges.Where(b => !b.IsEarned && currentValue >= b.ConditionValue))
                {
                    // Sblocca il badge
                    badge.EarnedDate = DateTime.Now;
                    unlockedBadges.Add(badge);

                    System.Diagnostics.Debug.WriteLine($"Badge sbloccato: {badge.Name}");
                }

                return unlockedBadges;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo badge: {ex.Message}");
                return unlockedBadges;
            }
        }

        /// <summary>
        /// Ottiene tutti i badge disponibili per una condizione specifica
        /// </summary>
        private List<Badge> GetAvailableBadges(int userId, BadgeConditionType conditionType)
        {
            // In un'implementazione reale, questi dati verrebbero dal database
            // Per ora creiamo una lista statica di badge disponibili

            var badges = new List<Badge>();

            switch(conditionType)
            {
                case BadgeConditionType.WorkoutsCompleted:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Primo Passo", "Completa il tuo primo allenamento", "#4CAF50", BadgeRarity.Common, 1),
                        CreateBadge(userId, "In Movimento", "Completa 5 allenamenti", "#2196F3", BadgeRarity.Common, 5),
                        CreateBadge(userId, "Determinato", "Completa 10 allenamenti", "#FF9800", BadgeRarity.Uncommon, 10),
                        CreateBadge(userId, "Atleta", "Completa 25 allenamenti", "#9C27B0", BadgeRarity.Rare, 25),
                        CreateBadge(userId, "Campione", "Completa 50 allenamenti", "#F44336", BadgeRarity.Epic, 50),
                        CreateBadge(userId, "Leggenda", "Completa 100 allenamenti", "#FFD700", BadgeRarity.Legendary, 100)
                    });
                    break;

                case BadgeConditionType.ConsecutiveDays:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Costante", "Allenati per 3 giorni consecutivi", "#4CAF50", BadgeRarity.Common, 3),
                        CreateBadge(userId, "Disciplinato", "Allenati per 7 giorni consecutivi", "#2196F3", BadgeRarity.Uncommon, 7),
                        CreateBadge(userId, "Implacabile", "Allenati per 14 giorni consecutivi", "#FF9800", BadgeRarity.Rare, 14),
                        CreateBadge(userId, "Inarrestabile", "Allenati per 30 giorni consecutivi", "#9C27B0", BadgeRarity.Epic, 30)
                    });
                    break;

                case BadgeConditionType.PhotosUploaded:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Prima Foto", "Carica la tua prima foto di progresso", "#4CAF50", BadgeRarity.Common, 1),
                        CreateBadge(userId, "Documentarista", "Carica 5 foto di progresso", "#2196F3", BadgeRarity.Common, 5),
                        CreateBadge(userId, "Cronista", "Carica 10 foto di progresso", "#FF9800", BadgeRarity.Uncommon, 10)
                    });
                    break;
            }

            return badges;
        }

        private Badge CreateBadge(int userId, string name, string description, string color, BadgeRarity rarity, int conditionValue)
        {
            return new Badge
            {
                Name = name,
                Description = description,
                IconPath = $"badge_{name.ToLower().Replace(" ", "_")}.png",
                Color = color,
                Category = BadgeCategory.Achievement,
                Rarity = rarity,
                ConditionType = BadgeConditionType.WorkoutsCompleted,
                ConditionValue = conditionValue,
                ConditionDescription = description,
                UserId = userId,
                User = new User { Id = userId, Name = "User" }, // Placeholder
                IsVisible = true
            };
        }
    }
}