using Microsoft.Maui.Storage;
using SmartDogDoor.Services;
using System;
using System.Diagnostics;

namespace SmartDogDoor.ViewModel;

public partial class LockViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<Lock> Locks { get; } = new();
    public LockViewModel(PetService petService)
    {
        Title = "Access";
        this.petService = petService;
        GetLocksAsync();
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

            int count = 1;
            foreach (var lockRule in locks)
            {
                // set rule number
                lockRule.ruleNumber = count;
                
                // parse time start information
                int index1 = lockRule.TimeStart.IndexOf("_");
                int index2 = lockRule.TimeStart.LastIndexOf("_");
                lockRule.TimeStartDay = lockRule.TimeStart.Substring(0, index1);
                lockRule.TimeStartHour = lockRule.TimeStart.Substring(index1, index2-index1);
                lockRule.TimeStartMinute = lockRule.TimeStart.Substring(index2, lockRule.TimeStart.Length - index2 -1);

                // parse time stop information
                index1 = lockRule.TimeStop.IndexOf("_");
                index2 = lockRule.TimeStop.LastIndexOf("_");
                lockRule.TimeStopDay = lockRule.TimeStop.Substring(0, index1);
                lockRule.TimeStopHour = lockRule.TimeStop.Substring(index1, index2 - index1);
                lockRule.TimeStopMinute = lockRule.TimeStop.Substring(index2, lockRule.TimeStop.Length - index2 - 1);

                // Add Rule to Lock list
                Locks.Add(lockRule);
                
                // Increase rule count
                count++;
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
    async Task addRestrictionAsync(string timeStart, string timeStop, bool lockUnlock )
    {
        //call pet service, addLock(), with the passed parameters
        //this will add entry in table

        //call pet service, getLocks(), to update observable collection, which will update the page_
 
    }
    */

    /*
    async Task deleteRestrictionAsync(Lock lock)
    {
        //Prompt the user "Are you sure you want to delete the locking restriction?" (yes/no)

        //call pet service, deleteLock(), with the passed Lock object
        //this will delete entry in table

        //call pet service, getLocks(), to update observable collection, which will update the page
    }
    */
}
