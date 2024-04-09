using Microsoft.Maui.Storage;
using SmartDogDoor.Services;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace SmartDogDoor.ViewModel;

public partial class LockViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<Lock> Locks { get; } = new();

    // for keeping track of pet being added
    private bool _isEditLocks = false;

    public bool IsEditLocks
    {
        get
        {
            return _isEditLocks;
        }
        set
        {
            _isEditLocks = value;
            OnPropertyChanged(nameof(IsEditLocks));
        }
    }

    //Bool to keep track of edit or add function used (true edit, false add)
    private bool _isEditAdd;
    public bool IsEditAdd
    {
        get
        {
            return _isEditAdd;
        }
        set
        {
            _isEditAdd = value;
            OnPropertyChanged(nameof(IsEditAdd));
            //filterActivity();
        }
    }

    //Bool to keep track of always locked (true always locked enabled)
    private bool _isNotAlwaysLocked;
    public bool IsNotAlwaysLocked
    {
        get
        {
            return _isNotAlwaysLocked;
        }
        set
        {
            _isNotAlwaysLocked = value;
            OnPropertyChanged(nameof(IsNotAlwaysLocked));
        }
    }

    // for keeping track of selected start day index when editng/adding a lock rule
    private int _selectedRuleStartDayIndex = 0;
    public int SelectedRuleStartDayIndex
    {
        get
        {
            return _selectedRuleStartDayIndex;
        }
        set
        {
            _selectedRuleStartDayIndex = value;
            OnPropertyChanged(nameof(SelectedRuleStartDayIndex));
        }
    }

    // for keeping track of selected start hour index when editng/adding a lock rule
    private String _selectedRuleStartHour;
    public String SelectedRuleStartHour
    {
        get
        {
            return _selectedRuleStartHour;
        }
        set
        {
            _selectedRuleStartHour = value;
            OnPropertyChanged(nameof(SelectedRuleStartHour));
        }
    }

    // for keeping track of selected start minute index when editng/adding a lock rule
    private String _selectedRuleStartMinute;
    public String SelectedRuleStartMinute
    {
        get
        {
            return _selectedRuleStartMinute;
        }
        set
        {
            _selectedRuleStartMinute = value;
            OnPropertyChanged(nameof(SelectedRuleStartMinute));
        }
    }

    // for keeping track of selected start AM/PM index when editng/adding a lock rule
    private int _selectedRuleStartAMPMIndex = 0;
    public int SelectedRuleStartAMPMIndex
    {
        get
        {
            return _selectedRuleStartAMPMIndex;
        }
        set
        {
            _selectedRuleStartAMPMIndex = value;
            OnPropertyChanged(nameof(SelectedRuleStartAMPMIndex));
        }
    }

    // for keeping track of selected stop day index when editng/adding a lock rule
    private int _selectedRuleStopDayIndex = 0;
    public int SelectedRuleStopDayIndex
    {
        get
        {
            return _selectedRuleStopDayIndex;
        }
        set
        {
            _selectedRuleStopDayIndex = value;
            OnPropertyChanged(nameof(SelectedRuleStopDayIndex));
        }
    }


    // for keeping track of selected stop hour index when editng/adding a lock rule
    private String _selectedRuleStopHour;
    public String SelectedRuleStopHour
    {
        get
        {
            return _selectedRuleStopHour;
        }
        set
        {
            _selectedRuleStopHour = value;
            OnPropertyChanged(nameof(SelectedRuleStopHour));
        }
    }


    // for keeping track of selected stop minute index when editng/adding a lock rule
    private String _selectedRuleStopMinute;
    public String SelectedRuleStopMinute
    {
        get
        {
            return _selectedRuleStopMinute;
        }
        set
        {
            _selectedRuleStopMinute = value;
            OnPropertyChanged(nameof(SelectedRuleStopMinute));
        }
    }


    // for keeping track of selected stop AM/PM index when editng/adding a lock rule
    private int _selectedRuleStopAMPMIndex = 0;
    public int SelectedRuleStopAMPMIndex
    {
        get
        {
            return _selectedRuleStopAMPMIndex;
        }
        set
        {
            _selectedRuleStopAMPMIndex = value;
            OnPropertyChanged(nameof(SelectedRuleStopAMPMIndex));
        }
    }

    private int selectedRuleId;// currently selected rule id

    IConnectivity connectivity;

    public LockViewModel(PetService petService, IConnectivity connectivity)
    {
        Title = "Access";
        this.petService = petService;
        this.connectivity = connectivity;
    }

    // Get locking rules from database
    [RelayCommand]
    async Task GetLocksAsync ()
    {
        if(IsBusy) return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }
            IsBusy = true;
            var locks = await petService.GetLocks();// get locking rules

            if(Locks.Count != 0)// clear locking rule list if populated
                Locks.Clear();

            IsNotAlwaysLocked = true; // Check for always locked condition

            int count = 1;
            foreach (var lockRule in locks)// parse each locking rule
            {
                if (lockRule.Id == -1)
                {
                    IsNotAlwaysLocked = false;
                    Title = "Access (Always Locked Enabled)";
                }
                else
                {
                    // set rule number
                    lockRule.ruleNumber = count;

                    //Convert Information to display format
                    // Convert Hour
                    String hour = lockRule.TimeStartHour;
                    String am_pm = "";
                    ConvertMilitaryToHour(ref hour, ref am_pm);
                    lockRule.TimeStartHour = hour;
                    lockRule.TimeStartAM_PM = am_pm;
                    
                    //Convert Day
                    String day = lockRule.TimeStartDay;
                    ConvertNumberToDay(ref day);
                    lockRule.TimeStartDay = day;

                    if(lockRule.TimeStartMinute.Length == 1)
                    {
                        lockRule.TimeStartDisplay = $"{lockRule.TimeStartDay} {lockRule.TimeStartHour}:0{lockRule.TimeStartMinute} {lockRule.TimeStartAM_PM}";
                    }
                    else
                    {
                        lockRule.TimeStartDisplay = $"{lockRule.TimeStartDay} {lockRule.TimeStartHour}:{lockRule.TimeStartMinute} {lockRule.TimeStartAM_PM}";
                    }

                    //Convert Information to display format
                    // Convert Hour
                    hour = lockRule.TimeStopHour;
                    am_pm = "";
                    ConvertMilitaryToHour(ref hour, ref am_pm);
                    lockRule.TimeStopHour = hour;
                    lockRule.TimeStopAM_PM = am_pm;
                    
                    //Convert Day
                    day = lockRule.TimeStopDay;
                    ConvertNumberToDay(ref day);
                    lockRule.TimeStopDay = day;
                    Debug.WriteLine("\n\n Day");
                    Debug.WriteLine(lockRule.TimeStopDay);

                    if (lockRule.TimeStopMinute.Length == 1)
                    {
                        lockRule.TimeStopDisplay = $"{lockRule.TimeStopDay} {lockRule.TimeStopHour}:0{lockRule.TimeStopMinute} {lockRule.TimeStopAM_PM}";
                    }
                    else
                    {
                        lockRule.TimeStopDisplay = $"{lockRule.TimeStopDay} {lockRule.TimeStopHour}:{lockRule.TimeStopMinute} {lockRule.TimeStopAM_PM}";
                    }
                    // Add Rule to Lock list
                    Locks.Add(lockRule);
                
                    // Increase rule count
                    count++;
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

    // Edit Selected Rule
    [RelayCommand]
    async Task EditRule(Lock restriction)
    {
        if(IsNotAlwaysLocked && !IsEditLocks)// if not always locked enabled and lock rule is not being edited
        {
            // Set Start Rule
            selectedRuleId = restriction.Id;
            SelectedRuleStartDayIndex = ConvertDayToNumber(restriction.TimeStartDay);
            SelectedRuleStartHour = restriction.TimeStartHour;
            SelectedRuleStartMinute = restriction.TimeStartMinute;
            if(restriction.TimeStartAM_PM == "AM")
            {
                SelectedRuleStartAMPMIndex = 0;
            }
            else
            {
                SelectedRuleStartAMPMIndex = 1;
            }

            // Set Stop Rule
            SelectedRuleStopDayIndex = ConvertDayToNumber(restriction.TimeStopDay);
            SelectedRuleStopHour = restriction.TimeStopHour;
            SelectedRuleStopMinute = restriction.TimeStopMinute;
            if (restriction.TimeStopAM_PM == "AM")
            {
                SelectedRuleStopAMPMIndex = 0;
            }
            else
            {
                SelectedRuleStopAMPMIndex = 1;
            }

            IsEditAdd = true;

            IsEditLocks = true;
        }
        

    }

    // cancel editing of rule
    [RelayCommand]
    async Task CancelEditRule()
    {
        IsEditLocks = false;
    }

    // New rule popup
    [RelayCommand]
    async Task AddRule()
    {
        if(!IsEditLocks)
        {
            // Set Start Rule
            SelectedRuleStartDayIndex = 0;
            SelectedRuleStartHour = "12";
            SelectedRuleStartMinute = "0";
            SelectedRuleStartAMPMIndex = 1;
        

            // Set Stop Rule
            SelectedRuleStopDayIndex = 1;
            SelectedRuleStopHour = "12";
            SelectedRuleStopMinute = "0";
            SelectedRuleStopAMPMIndex = 0;

            IsEditAdd = false;

            IsEditLocks = true;
        }
        
    }

    // Deletes locking rule
    [RelayCommand]
    async Task DeleteRule()
    {
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }

            await petService.deleteLock(selectedRuleId);// delete rule

            await GetLocksAsync();// get updated lock

            IsEditLocks = false;
        }
        catch (Exception ex)// error checking
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to delete Rule: {ex.Message}", "OK");
        }
        
    }

    // Saves currently edited lock to database whether it a new lock or change to an existing one
    [RelayCommand]
    async Task SaveLockRule()
    {
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }

            if (IsEditAdd) // edit current rule
            {
                // For for input Validity
                if ((Int32.Parse(SelectedRuleStartHour) > 12 || Int32.Parse(SelectedRuleStartHour) < 1) || (Int32.Parse(SelectedRuleStartMinute) > 59 || Int32.Parse(SelectedRuleStartMinute) < 0) || (Int32.Parse(SelectedRuleStopHour) > 12 || Int32.Parse(SelectedRuleStopHour) < 1) || (Int32.Parse(SelectedRuleStopMinute) > 59 || Int32.Parse(SelectedRuleStopMinute) < 0))
                {
                    return;
                }

                // Make new lock to pass to add lock
                Lock editLock = new Lock();
                editLock.Id = selectedRuleId;
                editLock.TimeStartDay = $"{SelectedRuleStartDayIndex}";
                editLock.TimeStartHour = $"{ConvertHourToMilitary(SelectedRuleStartHour, SelectedRuleStartAMPMIndex)}";
                editLock.TimeStartMinute = SelectedRuleStartMinute;
                editLock.TimeStopDay = $"{SelectedRuleStopDayIndex}";
                editLock.TimeStopHour = $"{ConvertHourToMilitary(SelectedRuleStopHour, SelectedRuleStopAMPMIndex)}";
                editLock.TimeStopMinute = SelectedRuleStopMinute;

                await petService.deleteLock(selectedRuleId); // delete old lock

                await petService.addLock(editLock); // add new lock

            }
            else // add new rule
            {
                // For for input Validity
                if ((Int32.Parse(SelectedRuleStartHour) > 12 || Int32.Parse(SelectedRuleStartHour) < 1) || (Int32.Parse(SelectedRuleStartMinute) > 59 || Int32.Parse(SelectedRuleStartMinute) < 0) || (Int32.Parse(SelectedRuleStopHour) > 12 || Int32.Parse(SelectedRuleStopHour) < 1) || (Int32.Parse(SelectedRuleStopMinute) > 59 || Int32.Parse(SelectedRuleStopMinute) < 0))
                {
                    return;
                }

                // Make new lock to pass to add lock
                Lock editLock = new Lock();
                if (Locks.Count > 0)
                {
                    editLock.Id = Locks[Locks.Count - 1].Id + 1; // set Id to highest current id + 1
                }
                else
                {
                    editLock.Id = 1;
                }
                editLock.TimeStartDay = $"{SelectedRuleStartDayIndex}";
                editLock.TimeStartHour = $"{ConvertHourToMilitary(SelectedRuleStartHour, SelectedRuleStartAMPMIndex)}";
                editLock.TimeStartMinute = SelectedRuleStartMinute;
                editLock.TimeStopDay = $"{SelectedRuleStopDayIndex}";
                editLock.TimeStopHour = $"{ConvertHourToMilitary(SelectedRuleStopHour, SelectedRuleStopAMPMIndex)}";
                editLock.TimeStopMinute = SelectedRuleStopMinute;
                await petService.addLock(editLock); // add new lock
            }

            await GetLocksAsync();

            IsEditLocks = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to save lock rule: {ex.Message}", "OK");
        }

    }

    // Function to control always locked functionality
    [RelayCommand]
    async Task AlwaysLockedRule() 
    {
        if(IsEditLocks)
        { 
            return; 
        }
        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)// Check for internet access
            {
                await Shell.Current.DisplayAlert("Internet Connectivity Issue",
                    $"Please check your internet and try again!", "OK");
                return;
            }

            if (!IsNotAlwaysLocked) // disable always locked
            {
                Title = "Access";
                IsNotAlwaysLocked = true;
                await petService.deleteLock(-1);
            }
            else // enable always locked
            {
                Title = "Access (Always Locked Enabled)";
                Lock alwaysLocked = new Lock();
                alwaysLocked.Id = -1;
                alwaysLocked.TimeStartDay = "";
                alwaysLocked.TimeStartHour = "";
                alwaysLocked.TimeStartMinute = "";
                alwaysLocked.TimeStopDay = "";
                alwaysLocked.TimeStopHour = "";
                alwaysLocked.TimeStopMinute = "";

                await petService.addLock(alwaysLocked);// add always locked ruel to database

                IsNotAlwaysLocked = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to change always locked rule: {ex.Message}", "OK");
        }

    }

    // Convert day of week to number to be stored in database
    private int ConvertDayToNumber(String day)
    {
        switch (day)
        {
            case "Sunday":
                return 0;
            case "Monday":
                return 1;
            case "Tuesday":
                return 2;
            case "Wednesday":
                return 3;
            case "Thursday":
                return 4;
            case "Friday":
                return 5;
            case "Saturday":
                return 6;
            default:
                return -1;
        }
    }

    // Convert Number stored to database to day of week
    private void ConvertNumberToDay(ref String day)
    {
        switch (day)
        {
            case "0":
                day = "Sunday";
                return;
            case "1":
                day =  "Monday";
                return;
            case "2":
                day =  "Tuesday";
                return;
            case "3":
                day = "Wednesday";
                return;
            case "4":
                day = "Thursday";
                return;
            case "5":
                day = "Friday";
                return;
            case "6":
                day = "Saturday";
                return;
            default:
                return;
        }
    }

    // Convert Miltary Time to Standard Time
    private void ConvertMilitaryToHour(ref String hour, ref String AM_PM)
    {
        switch (hour)
        {
            case "0":
                hour = "12";
                AM_PM = "AM";
                return;
            case "1":
                AM_PM = "AM";
                return;
            case "2":
                AM_PM = "AM";
                return;
            case "3":
                AM_PM = "AM";
                return;
            case "4":
                AM_PM = "AM";
                return;
            case "5":
                AM_PM = "AM";
                return;
            case "6":
                AM_PM = "AM";
                return;
            case "7":
                AM_PM = "AM";
                return;
            case "8":
                AM_PM = "AM";
                return;
            case "9":
                AM_PM = "AM";
                return;
            case "10":
                AM_PM = "AM";
                return;
            case "11":
                AM_PM = "AM";
                return;
            case "12":
                AM_PM = "PM";
                return;
            case "13":
                hour = "1";
                AM_PM = "PM";
                return;
            case "14":
                hour = "2";
                AM_PM = "PM";
                return;
            case "15":
                hour = "3";
                AM_PM = "PM";
                return;
            case "16":
                hour = "4";
                AM_PM = "PM";
                return;
            case "17":
                hour = "5";
                AM_PM = "PM";
                return;
            case "18":
                hour = "6";
                AM_PM = "PM";
                return;
            case "19":
                hour = "7";
                AM_PM = "PM";
                return;
            case "20":
                hour = "8";
                AM_PM = "PM";
                return;
            case "21":
                hour = "9";
                AM_PM = "PM";
                return;
            case "22":
                hour = "10";
                AM_PM = "PM";
                return;
            case "23":
                hour = "11";
                AM_PM = "PM";
                return;
            default:
                return;
        }
    }

    // Convert Standard Time to Military Time
    private int ConvertHourToMilitary(String hour, int AM_PM)
    {
        switch (hour)
        {
            case "1":
                if(AM_PM == 0) // AM/PM is AM
                {
                    return 1;
                }
                else
                {
                    return 13;
                }
            case "2":
                if (AM_PM == 0)
                {
                    return 2;
                }
                else
                {
                    return 14;
                }
            case "3":
                if (AM_PM == 0)
                {
                    return 3;
                }
                else
                {
                    return 15;
                }
            case "4":
                if (AM_PM == 0)
                {
                    return 4;
                }
                else
                {
                    return 16;
                }
            case "5":
                if (AM_PM == 0)
                {
                    return 5;
                }
                else
                {
                    return 17;
                }
            case "6":
                if (AM_PM == 0)
                {
                    return 6;
                }
                else
                {
                    return 18;
                }
            case "7":
                if (AM_PM == 0)
                {
                    return 7;
                }
                else
                {
                    return 19;
                }
            case "8":
                if (AM_PM == 0)
                {
                    return 8;
                }
                else
                {
                    return 20;
                }
            case "9":
                if (AM_PM == 0)
                {
                    return 9;
                }
                else
                {
                    return 21;
                }
            case "10":
                if (AM_PM == 0)
                {
                    return 10;
                }
                else
                {
                    return 22;
                }
            case "11":
                if(AM_PM == 0)
                {
                    return 11;
                }
                else
                {
                    return 23;
                }
            case "12":
                if (AM_PM == 0)
                {
                    return 0;
                }
                else
                {
                    return 12;
                }
            default:
                return -1;
        }
    }

    // Runs on appearing of page
    [RelayCommand]
    async Task Appearing()
    {
        try
        {
            await GetLocksAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }
}
