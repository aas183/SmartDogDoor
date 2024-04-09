using SmartDogDoor.Services;
using System.Diagnostics;

namespace SmartDogDoor.ViewModel;

public partial class PetViewModel : BaseViewModel
{
    PetService petService;//Object of PetSerivce for getting info from database
    public ObservableCollection<Pet> Pets { get; } = new();//Data Collection of data from Database

    private bool firstPageLoad = false; 

    // for keeping track of pet being added
    private bool _isAddPet = false;

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


    IConnectivity connectivity;

    //Constructor of ViewModel
    public PetViewModel(PetService petService, IConnectivity connectivity)
    {
        Title = "Pets";
        this.petService = petService;
        this.connectivity = connectivity;
    }


    // On click of pet frame got to individual pet pages
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
            if(connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }
            IsBusy = true;
            var pets = await petService.GetPets();//Get pets

            if (Pets.Count != 0)//clear pet list if full
                Pets.Clear();

            foreach (var pet in pets)// update pet list from service call
            {
                if(pet.Id == "0")
                {
                    await petService.deletePet("0");
                    continue;
                }

                //Get InOut Colors
                if (pet.InOut.ToLower() == "true")
                {
                    pet.InOut = "In";
                    pet.InOutColor = Color.FromRgba("#008450");
                }
                else
                {
                    pet.InOut = "Out";
                    pet.InOutColor = Color.FromRgba("#B81D13");
                }
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

        return;
    }

    // Add pet command to start the process of adding a pet
    [RelayCommand]
    async Task AddPetAsync()
    {
        //If data pull is already occurring quit
        if (IsBusy) return;


        bool isInternet = true;
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                isInternet = false;
                return;
            }
            AddPetDialog = "Place the Pet's Tag Against the Tag Reader on the Dog Door within\n60 seconds.";// prompt the user what to do

            // Set busy flags
            IsBusy = true;
            IsAddPet = true;

            // Add placeholder pet Id to be seen by door
            var petCount = Pets.Count;
            await petService.addPet(); // Add new Pet to Database

            // Keep track of 60 seconds
            DateTime startTime, endTime;
            startTime = DateTime.Now;
            endTime = startTime;
            var elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);
            var stopTime = 60.0;
            
            // Flags to quit
            bool IdAdded = false;
            bool AddCancel = false;

            //AddPetDialog = "Place the Pet's Tag Against the Tag Reader on the Dog Door within\n60 seconds.";

            // loop until new Id added or 60 seconds elapses
            while ((elapsedTime < stopTime) && !IdAdded && !AddCancel)
            {
                IdAdded = await petService.checkForNewId(petCount + 1);
                AddCancel = await petService.checkForPetIdOfZero();

                endTime = DateTime.Now;
                elapsedTime = (((TimeSpan)(endTime - startTime)).TotalSeconds);

                Debug.WriteLine($"Still Check = {elapsedTime}");
                AddPetDialog = $"Place the Pet's Tag Against the Tag Reader on the Dog Door within\n{(int)(stopTime-elapsedTime)} seconds.";
            }

            if (!AddCancel)
            {
                await petService.deletePet("0");
                AddPetDialog = "Pet Could Not Be Found.";
            }

            if (IdAdded) // if no tag found delete pet
            { 
                AddPetDialog = "Pet Added Successfully!";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
        finally
        {
            if (isInternet)
            {
                IsBusy = false;//set busy back to false
                await GetPetsAsync();
            }   
        }

        return;
    }

    // On cancel of adding pet
    [RelayCommand]
    async Task cancelPetAdd()
    {
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }
            if (!(await petService.checkForPetIdOfZero()))// check for id of 0 still exists
                await petService.deletePet("0");// delete pet entry with Id of 0
             IsAddPet = false;// set flag to no longer adding pet
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }

    }

    // On page appearing
    [RelayCommand]
    async Task Appearing()
    {
        try
        {
            await GetPetsAsync();
            /*
            if(!firstPageLoad)
            {
                await GetPetsAsync();
                firstPageLoad = true;
            }
            else
            {
                await GetPetsLocal();
            }
            */


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

}
