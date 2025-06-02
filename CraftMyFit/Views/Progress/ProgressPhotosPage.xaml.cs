using CraftMyFit.ViewModels.Progress;

namespace CraftMyFit.Views.Progress;

public partial class ProgressPhotosPage : ContentPage
{
    public ProgressPhotosPage(ProgressPhotosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}