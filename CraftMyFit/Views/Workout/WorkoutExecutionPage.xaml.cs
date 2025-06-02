using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class WorkoutExecutionPage : ContentPage
{
    public WorkoutExecutionPage(WorkoutExecutionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is WorkoutExecutionViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if(BindingContext is WorkoutExecutionViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}