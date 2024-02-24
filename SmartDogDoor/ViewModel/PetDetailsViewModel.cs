﻿using SmartDogDoor.Services;
using SmartDogDoor.View;
namespace SmartDogDoor.ViewModel;

[QueryProperty("Pet", "Pet")]
public partial class PetDetailsViewModel : BaseViewModel
{

    PetService petService;//Object of PetSerivce for getting info from database
    private ObservableCollection<PetActivity> _activities = new();//Data Collection of data from Databasej
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
        }
    }

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

    private ImageSource selectedPetImage;

    public PetDetailsViewModel(PetService petService)
    {

        this.petService = petService;
        GetActivitiesAsync();
    }


    //Get Details from pet Information Page
    [RelayCommand]
    async Task GetActivitiesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var activities = await petService.GetPetActivities();

            if (Activities.Count != 0)
            {
                Activities.Clear();
            }

            foreach (var activity in activities)
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
    /*
    async Task deletePetsAsync(string Id)
    {
        //Prompt user "Are you sure if you want to delete pet from system?" (Yes/No)

        //If no 
            //quit
        //If yes
            //call pet service, deletePetImages(), to delete all images associated Id
            //all pet services, deletePet(), to delete all entries for selected pet's id in pet information and pet activity table
    }
    */

    
   async Task changePetNameAsync()
   {
        //call pet service function chnagePetName() to change name of pet in pet information database table entry with passed petID
        try
        {
            Console.Write($"\nPet.Id: {Pet.Id}, PetName: {PetName}");
            await petService.ChangePetName(Pet.Id,PetName);
            PetNameSaved = PetName;

            
            for (int i = 0; i < Activities.Count; ++i)
            {
                Console.Write($"\nName Change");
                Activities[i].Name= PetName;
            }
            

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

    [RelayCommand]
    async Task savePetChanges()
    {
        //save changes made to pet information by user
        try
        {
            if(PetImageFile != "" &&  PetImageFile != Pet.Image) // Save Image
            {
                
            }
            if(PetName != Pet.Name) // Change Pet Name
            {
                Console.Write($"\nPet.Name: {Pet.Name}, PetName: {PetName}");
                await changePetNameAsync();
            }

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


    /*
   [RelayCommand]
   async Task changePetImageAsync(PickOptions options)
   {
        //User will pick an image using XMAL file picker and pass it to this function
        if(PetImageFile == "")
        {
            return;
        }


        //change image name to Profile_<petID>.<extension>
       // var activities = await petService.GetPetActivities();//Get pets


        //call pet services, addPetImage() to upload image to Azure Blob Storage and return image URL
        //var activities = await petService.GetPetActivities();//Get pets


        //call pet services, addPetImageDatabase(), with image URL and petID to put image URL in pet information database table entry with passed petID
        //returns URL

        //call pet services, deletePetImage() with returned URL from addPetImage() call, to delete old pet profile image if it exists
    }
    */
    /*
    //User Picks Image From System
    [RelayCommand]
    async Task<FileResult> PickImage(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    
                    PetImageFile = result.FileName;
                }
            }

            return result;
        }
        catch (Exception ex)
        {

            return null; // The user canceled or something went wrong
        }
    }
    */
    
    //User Picks Image From System
    [RelayCommand]
    async Task<FileResult> PickImage(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    selectedPetImage = ImageSource.FromStream(() => stream);
                    PetImageFile = result.FileName;
                }
            }

            return result;
        }
        catch (Exception ex)
        {

            return null; // The user canceled or something went wrong
        }
    }
    
}
