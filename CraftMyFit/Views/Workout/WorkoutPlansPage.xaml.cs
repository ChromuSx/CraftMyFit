using CraftMyFit.ViewModels.Workout;

namespace CraftMyFit.Views.Workout;

public partial class WorkoutPlansPage : ContentPage
{
    public WorkoutPlansPage(WorkoutPlansViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is WorkoutPlansViewModel viewModel)
        {
            // Richiama il refresh dei piani ogni volta che la pagina appare
            viewModel.RefreshCommand.Execute(null);
        }
    }
}