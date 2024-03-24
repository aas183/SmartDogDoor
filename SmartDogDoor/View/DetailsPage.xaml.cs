using Plugin.LocalNotification;

namespace SmartDogDoor.View;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(PetDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    private void editPetInfo(object sender, EventArgs e)
    {
        Activity.IsVisible = false;
        Edit.IsVisible = true;
        deleteBtn.IsVisible = true;
    }

    private void saveInfo(object sender, EventArgs e)
    {
        Activity.IsVisible = true;
        Edit.IsVisible = false;
        deleteBtn.IsVisible = false;
    }

    private void cancelInfo(object sender, EventArgs e)
    {
        Activity.IsVisible = true;
        Edit.IsVisible = false;
        deleteBtn.IsVisible= false;
    }
}
