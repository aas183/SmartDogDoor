

namespace SmartDogDoor.View;


public partial class MainPage : ContentPage
{
   
    public MainPage(PetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void addPet(object sender, EventArgs e)
    {
        //AddPetBtn.IsVisible = false;
        //PetsView.IsVisible = false;
        //AddPetInteraction.IsVisible = true;

        //InsLabel.Text = "Place the New Pet's Tag Against the Tag Reader on the Dog Door within\n1 minute.";


    }

    private void cancelAddPet(object sender, EventArgs e)
    {
        //AddPetBtn.IsVisible = true;
        //PetsView.IsVisible = true;
        //AddPetInteraction.IsVisible = false;
    }
}
