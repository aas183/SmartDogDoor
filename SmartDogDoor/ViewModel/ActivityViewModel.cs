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
            var activities = await petService.GetPetActvities();

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
    async Task filterActivity(string time_pet, string petID)
    {

    }
    */
}
