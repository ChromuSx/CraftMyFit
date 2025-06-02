using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class CreateWorkoutPlanPage : ContentPage
{
    public CreateWorkoutPlanPage(CreateWorkoutPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}