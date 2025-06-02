using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class EditWorkoutPlanPage : ContentPage
{
    public EditWorkoutPlanPage(EditWorkoutPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is EditWorkoutPlanViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}