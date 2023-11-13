

namespace SmartDogDoor.View;


public partial class MainPage : ContentPage
{
   
    public MainPage(PetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
