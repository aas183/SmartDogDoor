using SmartDogDoor.Services;
namespace SmartDogDoor.ViewModel;

public partial class PetViewModel : BaseViewModel
{
    PetService petService;//Object of PetSerivce for getting info from database
    public ObservableCollection<Pet> Pets { get; } = new();//Data Collection of data from Database

    // for keeping track of pet being added
    private bool _isAddPet;

    public bool IsAddPet
    {
        get
        {
            return _isAddPet;
        }
        set
        {
            _isAddPet = value;
            OnPropertyChanged(nameof(IsAddPet));
        }
    }

    // for diaglog of pet add
    private string _addPetDialog;

    public string AddPetDialog
    {
        get
        {
            return _addPetDialog;
        }
        set
        {
            _addPetDialog = value;
            OnPropertyChanged(nameof(AddPetDialog));
        }
    }

    //Constructor of ViewModel
    public PetViewModel(PetService petService)
    {
        Title = "Pets";
        this.petService = petService;
        IsAddPet = false;
        //GetPetsAsync();
    }


    //Not Fully Implemented but function from going to pet info page to indivdual pet pages
    [RelayCommand]
    async Task GoToDetailsAsync(Pet pet)
    {
        if (pet is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(View.DetailsPage)}", true, 
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

            if (Pets.Count != 0)//clear pet list if full
                Pets.Clear();

            foreach (var pet in pets)//update pet list from service call
            { 
                if (pet.InOut == "In")
                    pet.InOutColor = Color.FromRgba("#008450");
                else
                    pet.InOutColor = Color.FromRgba("#B81D13");
                Pets.Add(pet);
            }   
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

    //Get Details from pet Information Page from local save in pet service
    [RelayCommand]
    async Task GetPetsLocal()
    {
        try
        {
            var pets = petService.GetPetsLocal();//Get pets

            if (Pets.Count != 0)//clear pet list if full
                Pets.Clear();

            foreach (var pet in pets)//update pet list from service call
            {
                if (pet.InOut == "In")
                    pet.InOutColor = Color.FromRgba("#008450");
                else
                    pet.InOutColor = Color.FromRgba("#B81D13");
                Pets.Add(pet);
            }
        }
        catch (Exception ex)//if error
        {
            Debug.WriteLine(ex);
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
    /*
    [RelayCommand]
    async Task AddPetAsync()
    {
        try
        {
            await petService.addPet(); // Add new Pet to Database

            DateTime startTime, endTime;
            startTime = DateTime.Now;
            endTime = startTime;
            var elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);
            bool IdAdded = false;

            // loop until new Id added or 60 seconds elapses
            while ((elapsedTime < 60.0) || IdAdded)
            {
                IdAdded = await petService.checkForPetIdOfZero();

                endTime = DateTime.Now;
                elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);

                Debug.WriteLine($"Still Check = {elapsedTime}");
            }

            if (!IdAdded) // if no tag found delete pet
            {
                await petService.deletePet("0");
            }
            else
            {
                await GetPetsAsync(); 
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }

        return;
    }
    */

    [RelayCommand]
    async Task AddPetAsync()
    {
        //If data pull is already occurring quit
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            IsAddPet = true;
            var petCount = Pets.Count;

            await petService.addPet(); // Add new Pet to Database

            DateTime startTime, endTime;
            startTime = DateTime.Now;
            endTime = startTime;
            var elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);
            bool IdAdded = false;
            bool AddCancel = false;

            // loop until new Id added or 60 seconds elapses
            while ((elapsedTime < 60.0) && !IdAdded)
            {
                IdAdded = await petService.checkForNewId(petCount + 1);
                AddCancel = await petService.checkForPetIdOfZero();

                endTime = DateTime.Now;
                elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);

                Debug.WriteLine($"Still Check = {elapsedTime}");
            }

            await petService.deletePet("0");

            if (IdAdded) // if no tag found delete pet
            {
                await GetPetsAsync(); // if found update pet view
            }
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
        finally
        {
            IsBusy = false;//set busy back to false
            IsAddPet = false;
        }

        return;
    }

    [RelayCommand]
    async Task cancelPetAdd()
    {
        await petService.deletePet("0");
        IsAddPet = false;
    }

    [RelayCommand]
    async Task Appearing()
    {
        try
        {

            //await GetPetsLocal();
            await GetPetsAsync();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

}
