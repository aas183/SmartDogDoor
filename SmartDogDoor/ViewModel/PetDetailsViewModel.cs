
namespace SmartDogDoor.ViewModel;

[QueryProperty("Pet", "Pet")]
public partial class PetDetailsViewModel : BaseViewModel
{
    public PetDetailsViewModel()
    {
        
       
    }

    [ObservableProperty]
    Pet pet;
}
