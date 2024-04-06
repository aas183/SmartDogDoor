using SmartDogDoor.Services;
using SmartDogDoor.View;
using System.Diagnostics;
using System.Threading;

namespace SmartDogDoor.ViewModel;

public partial class ActivityViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<PetActivity> Activities { get; } = new();
    public ObservableCollection<PetActivity> FilteredActivities { get; } = new();
    //public ObservableCollection<Pet> Pets { get; } = new();

    private ObservableCollection<Pet> _pets = new();//Data Collection of data from Database
    //[NotifyPropertyChangedFor(nameof(Activities))]
    public ObservableCollection<Pet> Pets
    {
        get
        {
            return _pets;
        }
        set
        {
            _pets = value;
            OnPropertyChanged(nameof(Pets));
        }
    }

    // for keeping track of selected pet for activity filter
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

    // keep track of selected index for most Recent or pet filter
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

    bool firstAppear = false; // keep track of firt appearing of page

    public ActivityViewModel(PetService petService)
    {
        Title = "Activity";
        this.petService = petService;
    }

    // Get Pet Activity
    [RelayCommand]
    async Task GetActivitiesAsync ()
    {
        //while(true)
        //{ 
            try
            {
                IsBusy = true;

                var activities = await petService.GetPetActivities();// get activity
                
                // clear activities if new activity
                if(Activities.Count != 0)
                {
                    Activities.Clear();
                    FilteredActivities.Clear();
                }

                // insert new activities in list and activities in filtered list
                foreach (var activity in activities)
                {
                    Activities.Insert(0,activity);// insert activity
                    if (SelectedFilterIndex == 0)// Most Recent
                    {
                        FilteredActivities.Insert(0, activity);
                    }
                    else// Pet Filter
                    {
                        if (_selectedPet != null && activity.Id == _selectedPet.Id)// pet is selected and Id matches
                        {
                            FilteredActivities.Insert(0, activity);// inset activity into filtered activity
                        }
                    }
                }
               
            }
            catch (Exception ex) // catch errors
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to get pet activity: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
    }

    // Get actvities every 30 seconds (only called on first appear of activity page
    void GetActivitiesContinuousAsync()
    {
        // Run on new thread continoulsy every 30 seconds will check for new activity
        new Thread(async() =>
        {
            Thread.CurrentThread.IsBackground = true;

            while(true) // run forever
            { 
                try
                {
                    IsBusy = true;
                    var activities = await petService.GetPetActivities();// get activity

                    // insert new activities in list and activities in filtered list
                    if (Activities.Count != activities.Count)// if new activity
                    {
                        Activities.Clear();
                        FilteredActivities.Clear();


                        foreach (var activity in activities)
                        {
                            Activities.Insert(0, activity);
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

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    await Shell.Current.DisplayAlert("Error!",
                        $"Unable to get pet activity: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
                Thread.Sleep(30000);
            }
        }).Start();

    }


    //Get Pets
    [RelayCommand]
    async Task GetPetsAsync()
    {
       
        //If data pull is already occurring quit
        if (IsBusy) return;


        try
        {
            IsBusy = true;
            var pets = await petService.GetPets();//Get pets

            if (Pets.Count != 0)//clear pet list if not empty
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

    //Get Pets locally saved in per service
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

    // Filter Activity
    //[RelayCommand]
    public void filterActivity()
    {
        if(SelectedFilterIndex == 0)// most recent selecte
        {
            FilteredActivities.Clear();
            foreach (var activity in Activities)
            {
                FilteredActivities.Add(activity);
            }
        }
        else// Pet Filter selected
        {
            if (_selectedPet != null)// if pet is selected
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
    }

    // Runs on appearing of page
    [RelayCommand]
    async Task Appearing()
    {
        try
        {
            await GetPetsAsync();
            if(!firstAppear)// first appearing of page
            {
                firstAppear = true;
                GetActivitiesContinuousAsync();
            }
            else
            {
                await GetActivitiesAsync();
            }
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }
}
