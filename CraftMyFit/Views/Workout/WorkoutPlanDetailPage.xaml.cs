using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class WorkoutPlanDetailPage : ContentPage
{
    public WorkoutPlanDetailPage(WorkoutPlanDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is WorkoutPlanDetailViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}