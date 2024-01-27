﻿using SmartDogDoor.Services;
namespace SmartDogDoor.ViewModel;

[QueryProperty("Pet", "Pet")]
public partial class PetDetailsViewModel : BaseViewModel
{

    PetService petService;//Object of PetSerivce for getting info from database
    public ObservableCollection<PetActivity> PetActivities { get; } = new();//Data Collection of data from Database
    public PetDetailsViewModel(PetService petService)
    {

        this.petService = petService;
    }

    [ObservableProperty]
    Pet pet;


    //Get Details from pet Information Page
    [RelayCommand]
    async Task GetPetsAsync()
    {
        //If data pull is already occurring quit
        if (IsBusy) return;


        try
        {
            IsBusy = true;
            var activities = await petService.GetPetActivities();//Get pets

            if (PetActivities.Count != 0)//clear pet list if full
                PetActivities.Clear();

            foreach (var activity in activities)//update pet list from service call
                PetActivities.Add(activity);
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

    /*
   async Task changePetNameAsync(string name, string petID)
   {
       //call pet service function chnagePetName() to change name of pet in pet information database table entry with passed petID
   }

   async Task changePetImageAsync(image, string petID)
   {
       //User will pick an image using XMAL file picker and pass it to this function
       
       //change image name to Profile_<petID>.<extension>

       //call pet services, addPetImage() to upload image to Azure Blob Storage and return image URL

       //call pet services, addPetImageDatabase(), with image URL and petID to put image URL in pet information database table entry with passed petID
       //returns URL

       //call pet services, deletePetImage() with returned URL from addPetImage() call, to delete old pet profile image if it exists
    }
   */
    public async Task<FileResult> PickImage(PickOptions options)
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
                    var image = ImageSource.FromStream(() => stream);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }
}
