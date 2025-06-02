using CraftMyFit.ViewModels.Progress;

namespace CraftMyFit.Views.Progress;

public partial class AddProgressPhotoPage : ContentPage
{
    public AddProgressPhotoPage(AddProgressPhotoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is AddProgressPhotoViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if(BindingContext is AddProgressPhotoViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}