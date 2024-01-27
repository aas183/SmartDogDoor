using SmartDogDoor.Services;
using System.Diagnostics;

namespace SmartDogDoor.ViewModel;

public partial class ActivityViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<PetActivity> Activities { get; } = new();
    public ActivityViewModel(PetService petService)
    {
        Title = "Activity";
        this.petService = petService;
    }


    [RelayCommand]
    async Task GetPetsAsync ()
    {
        if(IsBusy) return;

        try
        {
            IsBusy = true;
            var activities = await petService.GetPetActivities();
            
            if(Activities.Count != 0)
                Activities.Clear();

            foreach (var activity in activities)
                Activities.Add(activity);
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
    async Task filterActivity(bool time_pet, string petID)
    {
        //call pet service, getPetActivities(), to update Activities observable collection

        //check time_pet bool
        //If 1 time filter activated
            //sort entries in observable collection by most recent time first
        //If 0 pet name selected;
            //look through observable collection and delete entries that do not have the passed petID
            //sort entries by most recent activity first
    }
    */
}
