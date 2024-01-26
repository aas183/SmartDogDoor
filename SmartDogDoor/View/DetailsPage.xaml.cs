namespace SmartDogDoor;

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
   
}
