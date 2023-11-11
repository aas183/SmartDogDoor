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
}
