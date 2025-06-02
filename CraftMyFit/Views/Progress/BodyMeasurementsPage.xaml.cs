using CraftMyFit.ViewModels.Progress;

namespace CraftMyFit.Views.Progress;

public partial class BodyMeasurementsPage : ContentPage
{
    public BodyMeasurementsPage(BodyMeasurementsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is BodyMeasurementsViewModel viewModel)
        {
            viewModel.OnAppearing();
        }
    }
}