using SmartDogDoor.Services;
using SmartDogDoor.View;
namespace SmartDogDoor.ViewModel;

[QueryProperty("Pet", "Pet")]
public partial class PetDetailsViewModel : BaseViewModel
{

    PetService petService;//Object of PetSerivce for getting info from database
    private ObservableCollection<PetActivity> _activities = new();//Data Collection of data from Database
    //[NotifyPropertyChangedFor(nameof(Activities))]
    public ObservableCollection<PetActivity> Activities
    {
        get
        {
            return _activities;
        }
        set
        {
            _activities = value;
            OnPropertyChanged(nameof(Activities));
        }
    }

    //For passed pet from Pet Info Page
    private Pet _pet;//For passed pet from Pet Info Page
    public Pet Pet
    {
        get => _pet;
        set
        {
            SetProperty(ref _pet, value);
            PetImageFile = _pet.Image;
            PetName = _pet.Name;
            PetNameSaved = _pet.Name;
            PetImageSaved = _pet.Image;
        }
    }

    private String selectedPetImage;

    //For keeping track of user selected pet image
    private String _petImageFile;
    public String PetImageFile
    {
        get
        {
            return _petImageFile;
        }
        set
        {
            _petImageFile = value;
            OnPropertyChanged(nameof(PetImageFile));
        }
    }

    //for holding most current saved pet name
    private String _petImageSaved;
    public String PetImageSaved
    {
        get
        {
            return _petImageSaved;
        }
        set
        {
            _petImageSaved = value;
            OnPropertyChanged(nameof(PetImageSaved));
        }
    }

    //for keeping track of user selected pet name
    private String _petName;
    public String PetName
    {
        get
        {
            return _petName;
        }
        set
        {
            _petName = value;
            OnPropertyChanged(nameof(PetName));
        }
    }

    //for holding most current saved pet name
    private String _petNameSaved;
    public String PetNameSaved
    {
        get
        {
            return _petNameSaved;
        }
        set
        {
            _petNameSaved = value;
            OnPropertyChanged(nameof(PetNameSaved));
        }
    }

    IConnectivity connectivity;

    public PetDetailsViewModel(PetService petService, IConnectivity connectivity)
    {
        this.petService = petService;
        this.connectivity = connectivity;
    }


    //Get Details from pet Information Page
    [RelayCommand]
    async Task GetActivitiesAsync()
    {
        if (IsBusy) return;
        Console.Write("Run GetPetActivity");
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }
            IsBusy = true;
            var activities = await petService.GetPetActivities();// get activity

            if (Activities.Count != 0)
            {
                Activities.Clear();
            }

            foreach (var activity in activities)// add activity for selected pet
            {
                if(activity.Id == Pet.Id)
                    Activities.Insert(0, activity);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get pets: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task deletePetAsync()
    {
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }

            IsBusy = true;

            await petService.deleteAllPetInformation(Pet.Id);// delete pet
            PetNameSaved = "Pet Deleted!";// change title to pet deleted
            PetName = "";
            PetImageFile = "";
        } 
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to save changes: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // chnage pet name
   async Task changePetNameAsync()
   {
        //call pet service function chnagePetName() to change name of pet in pet information database table entry with passed petID
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }
            Console.Write($"\nPet.Id: {Pet.Id}, PetName: {PetName}");
            await petService.ChangePetName(Pet.Id,PetName);
            PetNameSaved = PetName;
            
            for (int i = 0; i < Activities.Count; ++i)
            {
                //Update names in list (this forces the property chnaged event)
                Console.Write($"\nName Change");
                var activity = Activities[i];
                activity.Name = PetName;
                Activities[i] = activity;
            }
            Pet.Name = PetName;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to change range: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // save pet changes in form
    [RelayCommand]
    async Task savePetChanges()
    {
        //save changes made to pet information by user
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet) // Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }

            IsBusy = true;

            if (PetName != Pet.Name)// Change Pet Name
            {
                Console.Write($"\nPet.Name: {Pet.Name}, PetName: {PetName}");
                await changePetNameAsync();
            }

            // Change Pet Image
            if (PetImageFile != "" && PetImageFile != PetImageSaved)// Change Image
            {
                await petService.deletePetImage(Pet.Image);//delete current pet image
                var imageFilename = await petService.addPetImageDatabase(selectedPetImage, PetImageFile, /*Pet.Name*/PetImageFile);
                var image = await petService.changePetImage(Pet.Id, imageFilename);
                Pet.Image = image;
                PetImageSaved = image;
            }
            else if (PetImageFile == "" && PetImageSaved != "")// Change image if no previous image was registered with pet
            {
             
                var imageFilename = await petService.addPetImageDatabase(selectedPetImage, PetImageFile, /*Pet.Name*/PetImageFile);
                var image = await petService.changePetImage(Pet.Id, imageFilename);
                Pet.Image = image;
                PetImageSaved = image;
            }
           

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to save changes to pet information: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // User Picks Image From System
    [RelayCommand]
    async Task<FileResult> PickImage(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))// only allow jpg and png to be selected
                {
                    selectedPetImage = result.FullPath;// save path to selected image
                    PetImageFile = result.FileName;// save selected image filename
                }
            }

            return result;
        }
        catch (Exception ex)
        {

            return null; // The user canceled or something went wrong
        }
    }

    // On page appearing
    [RelayCommand]
    async Task Appearing()
    {
        try
        {
            await GetActivitiesAsync();// get pet actvities for selected pet
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

}
