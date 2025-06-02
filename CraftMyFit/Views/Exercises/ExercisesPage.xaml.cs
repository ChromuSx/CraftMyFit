using CraftMyFit.ViewModels.Exercises;

namespace CraftMyFit.Views.Exercises;

public partial class ExercisesPage : ContentPage
{
    public ExercisesPage(ExercisesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}