using CraftMyFit.ViewModels.Exercises;

namespace CraftMyFit.Views.Exercises;

public partial class ExerciseFormPage : ContentPage
{
    public ExerciseFormPage(ExerciseFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is ExerciseFormViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}