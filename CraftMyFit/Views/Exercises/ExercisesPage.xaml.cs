using CraftMyFit.ViewModels.Exercises;
using Microsoft.Maui.Controls;

namespace CraftMyFit.Views.Exercises;

public partial class ExercisesPage : ContentPage, IQueryAttributable
{
    public ExercisesPage(ExercisesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is ExercisesViewModel vm)
        {
            vm.ApplyQueryAttributes(query);
        }
    }
}