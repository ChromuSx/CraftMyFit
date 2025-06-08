using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models;
using CraftMyFit.Models.Workout;

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
            List<Exercise> exercises = await _exerciseRepository.GetAllAsync();
            List<Exercise> basicExercises = exercises.Where(e =>
                e.RequiredEquipmentJson == "[]" || string.IsNullOrEmpty(e.RequiredEquipmentJson))
                .ToList();

            WorkoutPlan plan = new()
            {
                Title = "Principiante Full Body",
                Description = "Piano di allenamento per principianti che allena tutto il corpo",
                UserId = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                WorkoutDaysEnum = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday],
                WorkoutDaysJson = "[\"Monday\",\"Wednesday\",\"Friday\"]",
                WorkoutDays = []
            };

            // Crea i giorni di allenamento
            WorkoutDay workoutDay = new()
            {
                DayOfWeek = DayOfWeek.Monday,
                Title = "Full Body Workout",
                OrderIndex = 0,
                WorkoutPlan = plan,
                Exercises = []
            };

            // Aggiungi esercizi base
            Exercise? pushUpExercise = basicExercises.FirstOrDefault(e => e.Name.Contains("Push-up"));
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

            Exercise? squatExercise = basicExercises.FirstOrDefault(e => e.Name.Contains("Squat"));
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
            List<Exercise> exercises = await _exerciseRepository.GetAllAsync();
            List<Exercise> hiitExercises = exercises.Where(e =>
                e.MuscleGroup == "Full Body" ||
                e.Name.Contains("Burpee") ||
                e.Name.Contains("Jump") ||
                e.Name.Contains("Sprint"))
                .ToList();

            User user = new()
            {
                Id = userId,
                Name = "User"
            };

            WorkoutPlan plan = new()
            {
                Title = "HIIT Intenso",
                Description = "Allenamento ad alta intensità per bruciare calorie",
                UserId = userId,
                User = user,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                WorkoutDaysEnum = [DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday],
                WorkoutDaysJson = "[\"Tuesday\",\"Thursday\",\"Saturday\"]",
                WorkoutDays = []
            };

            WorkoutDay workoutDay = new()
            {
                DayOfWeek = DayOfWeek.Tuesday,
                Title = "HIIT Circuit",
                OrderIndex = 0,
                WorkoutPlan = plan,
                Exercises = []
            };

            // Aggiungi esercizi HIIT
            int orderIndex = 0;
            foreach(Exercise? exercise in hiitExercises.Take(5))
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
        public List<WorkoutTemplate> GetAvailableTemplates() => [
                new WorkoutTemplate
                {
                    Id = "beginner_fullbody",
                    Name = "Principiante Full Body",
                    Description = "Perfetto per chi inizia, allena tutto il corpo 3 volte a settimana",
                    Difficulty = WorkoutDifficulty.Beginner,
                    EstimatedDuration = TimeSpan.FromMinutes(45),
                    RequiredEquipment = [],
                    TargetMuscleGroups = ["Full Body"]
                },
                new WorkoutTemplate
                {
                    Id = "hiit_intense",
                    Name = "HIIT Intenso",
                    Description = "Allenamento ad alta intensità per bruciare calorie velocemente",
                    Difficulty = WorkoutDifficulty.Intermediate,
                    EstimatedDuration = TimeSpan.FromMinutes(30),
                    RequiredEquipment = [],
                    TargetMuscleGroups = ["Full Body", "Cardio"]
                },
                new WorkoutTemplate
                {
                    Id = "push_pull_legs",
                    Name = "Push/Pull/Legs",
                    Description = "Divisione muscolare classica per livello intermedio-avanzato",
                    Difficulty = WorkoutDifficulty.Intermediate,
                    EstimatedDuration = TimeSpan.FromMinutes(60),
                    RequiredEquipment = ["Manubri", "Bilanciere"],
                    TargetMuscleGroups = ["Pettorali", "Schiena", "Spalle", "Bicipiti", "Tricipiti", "Gambe"]
                },
                new WorkoutTemplate
                {
                    Id = "home_bodyweight",
                    Name = "Allenamento a Casa",
                    Description = "Allenamento completo senza attrezzature",
                    Difficulty = WorkoutDifficulty.Beginner,
                    EstimatedDuration = TimeSpan.FromMinutes(40),
                    RequiredEquipment = [],
                    TargetMuscleGroups = ["Full Body"]
                }
            ];
    }

    // Modelli per i template
    public class WorkoutTemplate
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkoutDifficulty Difficulty { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public List<string> RequiredEquipment { get; set; } = [];
        public List<string> TargetMuscleGroups { get; set; } = [];
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