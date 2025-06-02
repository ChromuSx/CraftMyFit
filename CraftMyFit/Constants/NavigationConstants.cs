namespace CraftMyFit.Constants
{
    public static class NavigationConstants
    {
        // Main Pages
        public const string Dashboard = "//dashboard";
        public const string WorkoutPlans = "//workoutplans";
        public const string Exercises = "//exercises";
        public const string Progress = "//progress";
        public const string Settings = "//settings";

        // Workout Pages
        public const string CreateWorkoutPlan = "createworkoutplan";
        public const string EditWorkoutPlan = "editworkoutplan";
        public const string WorkoutPlanDetail = "workoutplandetail";
        public const string WorkoutExecution = "workoutexecution";

        // Exercise Pages
        public const string ExerciseDetail = "exercisedetail";
        public const string ExerciseForm = "exerciseform";

        // Progress Pages
        public const string ProgressPhotos = "progressphotos";
        public const string AddProgressPhoto = "addprogressphoto";
        public const string BodyMeasurements = "bodymeasurements";

        // Modal Pages
        public const string ExerciseTimer = "exercisetimer";
        public const string PhotoGallery = "photogallery";

        // Route Parameters
        public static class Parameters
        {
            public const string WorkoutPlanId = "workoutPlanId";
            public const string ExerciseId = "exerciseId";
            public const string WorkoutDayId = "workoutDayId";
            public const string UserId = "userId";
            public const string SessionId = "sessionId";
            public const string Exercise = "exercise";
            public const string WorkoutPlan = "workoutPlan";
            public const string Photo = "photo";
            public const string IsEdit = "isEdit";
        }

        // Shell Routes
        public static class Shell
        {
            public const string TabBar = "//";
            public const string Dashboard = "dashboard";
            public const string WorkoutPlans = "workoutplans";
            public const string Exercises = "exercises";
            public const string Progress = "progress";
            public const string Settings = "settings";
        }
    }
}