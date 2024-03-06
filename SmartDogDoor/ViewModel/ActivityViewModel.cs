using SmartDogDoor.Services;
using System.Diagnostics;

namespace SmartDogDoor.ViewModel;

public partial class ActivityViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<PetActivity> Activities { get; } = new();
    public ObservableCollection<PetActivity> FilteredActivities { get; } = new();
    public ObservableCollection<Pet> Pets { get; } = new();
    private Pet _selectedPet;
    public Pet SelectedPet
    {
        get
        {
            return _selectedPet;
        }
        set
        {
            SetProperty(ref _selectedPet, value);
            filterActivity();
        }
    }
    private int _selectedFilterIndex = 0;
    public int SelectedFilterIndex
    {
        get
        {
            return _selectedFilterIndex;
        }
        set
        {
            _selectedFilterIndex = value;
            filterActivity();
        }
    }

    public ActivityViewModel(PetService petService)
    {
        Title = "Activity";
        this.petService = petService;
        //GetPetsAsync();
    }


    [RelayCommand]
    async Task GetActivitiesAsync ()
    {
        if(IsBusy) return;

        try
        {
            IsBusy = true;
            var activities = await petService.GetPetActivities();
            
            if(Activities.Count != 0)
            {
                Activities.Clear();
                FilteredActivities.Clear();
            }
            /*
            var selectedPetId;
            if (SelectedFilterIndex == 1)
            {
                selectedPetId = _selectedPet.Id;
            }
            */

            foreach (var activity in activities)
            {
                Activities.Insert(0,activity);
                if (SelectedFilterIndex == 0)
                {
                    FilteredActivities.Insert(0, activity);
                }
                else
                {
                    if (_selectedPet != null && activity.Id == _selectedPet.Id)
                    {
                        FilteredActivities.Insert(0, activity);
                    }
                }
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

    //Get Details from pet Information Page
    [RelayCommand]
    async Task GetPetsAsync()
    {
        //If data pull is already occurring quit
        if (IsBusy) return;


        try
        {
            IsBusy = true;
            var pets = await petService.GetPets();//Get pets

            if (Pets.Count != 0)//clear pet list if full
                Pets.Clear();

            foreach (var pet in pets)//update pet list from service call
            {
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
    void GetPetsLocal()
    {
        try
        {
            var pets = petService.GetPetsLocal();//Get pets

            if(Pets.Count != pets.Count)
            { 
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
            else
            {
                for(int i = 0; i < pets.Count; ++i)//update pet list from service call
                {
                    if (pets[i].Name != Pets[i].Name)
                    {
                        Pets[i].Name = pets[i].Name;
                    }
                }
            }
        }
        catch (Exception ex)//if error
        {
            Debug.WriteLine(ex);
        }
    }

    //Get Details from pet Information Page
    //[RelayCommand]
    public void filterActivity()
    {
        if(SelectedFilterIndex == 0)
        {
            FilteredActivities.Clear();
            foreach (var activity in Activities)
            {
                FilteredActivities.Add(activity);
            }
        }
        else
        {
            if (_selectedPet != null)
            {
                var selectedPetId = _selectedPet.Id;
                FilteredActivities.Clear();
                foreach (var activity in Activities)
                {
                    if (activity.Id == selectedPetId)
                    {
                        FilteredActivities.Add(activity);
                    }

                }
            }
            else
            {
                FilteredActivities.Clear();
            }
        }
        //call pet service, getPetActivities(), to update Activities observable collection

        //check time_pet bool
        //If 1 time filter activated
            //sort entries in observable collection by most recent time first
        //If 0 pet name selected;
            //look through observable collection and delete entries that do not have the passed petID
            //sort entries by most recent activity first
    }

    [RelayCommand]
    async Task Appearing()
    {
        try
        {

            //await GetPetsLocal();
            GetPetsLocal();
            await GetActivitiesAsync();
            //await GetPetsAsync();
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }
}
