using CraftMyFit.Views.Exercises;
using CraftMyFit.Views.Progress;
using CraftMyFit.Views.Workout;

namespace CraftMyFit;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        // Registra route per navigazione
        Routing.RegisterRoute("createworkoutplan", typeof(CreateWorkoutPlanPage));
        Routing.RegisterRoute("editworkoutplan", typeof(EditWorkoutPlanPage));
        Routing.RegisterRoute("workoutplandetail", typeof(WorkoutPlanDetailPage));
        Routing.RegisterRoute("workoutexecution", typeof(WorkoutExecutionPage));
        Routing.RegisterRoute("exercisedetail", typeof(ExerciseDetailPage));
        Routing.RegisterRoute("exerciseform", typeof(ExerciseFormPage));
        Routing.RegisterRoute("addprogressphoto", typeof(AddProgressPhotoPage));
        Routing.RegisterRoute("bodymeasurements", typeof(BodyMeasurementsPage));
        Routing.RegisterRoute("exercisespage", typeof(ExercisesPage));
    }
}