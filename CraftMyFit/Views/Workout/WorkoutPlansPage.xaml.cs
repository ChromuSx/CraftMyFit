using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class WorkoutPlansPage : ContentPage
{
    public WorkoutPlansPage(WorkoutPlansViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}