using SmartDogDoor.Services;
namespace SmartDogDoor.ViewModel;

public partial class PetViewModel : BaseViewModel
{
    PetService petService;//Object of PetSerivce for getting info from database
    public ObservableCollection<Pet> Pets { get; } = new();//Data Collection of data from Database
    
    //Constructor of ViewModel
    public PetViewModel(PetService petService)
    {
        Title = "Pets";
        this.petService = petService;
        //GetPetsAsync();
    }


    //Not Fully Implemented but function from going to pet info page to indivdual pet pages
    [RelayCommand]
    async Task GoToDetailsAsync(Pet pet)
    {
        if (pet is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
            new Dictionary<string, object>
            {
                {"Pet", pet }
            });
    }

    //Get Details from pet Information Page
    [RelayCommand]
    async Task GetPetsAsync ()
    {
        //If data pull is already occurring quit
        if(IsBusy) return;


        try
        {
            IsBusy = true;
            var pets = await petService.GetPets();//Get pets

            if(Pets.Count != 0)//clear pet list if full
                Pets.Clear();

            foreach (var pet in pets)//update pet list from service call
                Pets.Add(pet);
        }
        catch (Exception ex)//if error
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get pets: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;//set busy back to false
        }
    }

    /*
    async Task addPetsAsync()
    {
        //Display a alert box saying do you want to add pet? Options are Yes or no

        //If no 
            //quit 
        //If yes 
            //call pet service, addPet(), to add row to database that with generic pet name ie. Pet_3
            //and no image and no id and InOut of in

        //After prompt user to hold new RFID tag to door

        //Wait for 1 minute and doing so continously check database to see if new row has id populated from dog door end

        //If it is populated 
            //tell the user the pet was successfully added to system update view
        //If not 
            //tell user pet addition was unsuccessful then call pet service, deletePet(), to delete pet entry
    }
    */

}
