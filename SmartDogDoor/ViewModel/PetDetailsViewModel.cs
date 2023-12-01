
namespace SmartDogDoor.ViewModel;

[QueryProperty("Pet", "Pet")]
public partial class PetDetailsViewModel : BaseViewModel
{
    public PetDetailsViewModel()
    {
        
       
    }

    [ObservableProperty]
    Pet pet;

    /*
    async Task deletePetsAsync()
    {
        //Prompt user "Are you sure if you want to delete pet from system?" (Yes/No)

        //If no 
            //quit
        //If yes
            //call pet service to delete all entries for selected pet's id in pet information and pet activity table
    }
    */

    /*
   async Task changePetNameAsync(string name, string petID)
   {
       //call pet service and change name of pet in pet information databaser table entry with passed petID
   }

   async Task changePetNameAsync(image, string petID)
   {
       //User will pick an image using xaml file picker and pass it to this function
       
       //change image name to Profile_<petID>.<extension>

       //call pet service to upload image to Azure Blob Storage and return image URL

       //call service with image URL and petID to put image URL in pet information database table entry with passed petID

    }
   */
}
