using SmartDogDoor.Services;
using System.Diagnostics;

namespace SmartDogDoor.ViewModel;

public partial class LockViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<Lock> Locks { get; } = new();
    public LockViewModel(PetService petService)
    {
        Title = "Lock Restrictions";
        this.petService = petService;
    }


    [RelayCommand]
    async Task GetLocksAsync ()
    {
        if(IsBusy) return;

        try
        {
            IsBusy = true;
            var locks = await petService.GetLocks();

            if(Locks.Count != 0)
                Locks.Clear();

            foreach (var lock_ in locks)
                Locks.Add(lock_);
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
    async Task addRestriction(string timeStart, string timeStop, bool lockUnlock )
    {
        //call pet service addLock with the passed parameters
        //this will add entry in table

        //call pet service getLocks to update observable collection, which will update the page
 
    }
    */

    /*
    async Task deleteRestriction(Lock lock)
    {
        //Prompt the user "Are you sure you want to delete the locking restriction?" (yes/no)

        //call pet service deleteLock with the passed Lock object
        //this will delete entry in table

        //call pet service getLocks to update observable collection, which will update the page
    }
    */
}
