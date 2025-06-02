using CraftMyFit.ViewModels.Exercises;

namespace CraftMyFit.Views.Exercises;

public partial class ExerciseDetailPage : ContentPage
{
    public ExerciseDetailPage(ExerciseDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}