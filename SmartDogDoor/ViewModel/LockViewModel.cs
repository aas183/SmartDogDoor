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
    private bool isEditAdd;

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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }


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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }

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
            //filterActivity();
        }
    }

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
                /*
                int index1 = lockRule.TimeStart.IndexOf("_");
                int index2 = lockRule.TimeStart.LastIndexOf("_");
                lockRule.TimeStartDay = lockRule.TimeStart.Substring(0, index1);
                lockRule.TimeStartHour = lockRule.TimeStart.Substring(index1+1, index2-index1-1);
                lockRule.TimeStartMinute = lockRule.TimeStart.Substring(index2+1, lockRule.TimeStart.Length-index2-1);
                */

                //Convert Information to display format
                // Convert Hour
                switch (lockRule.TimeStartHour)
                {
                    case "0":
                        lockRule.TimeStartHour = "12";
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "1":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "2":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "3":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "4":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "5":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "6":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "7":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "8":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "9":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "10":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "11":
                        lockRule.TimeStartAM_PM = "AM";
                        break;
                    case "12":
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "13":
                        lockRule.TimeStartHour = "1";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "14":
                        lockRule.TimeStartHour = "2";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "15":
                        lockRule.TimeStartHour = "3";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "16":
                        lockRule.TimeStartHour = "4";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "17":
                        lockRule.TimeStartHour = "5";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "18":
                        lockRule.TimeStartHour = "6";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "19":
                        lockRule.TimeStartHour = "7";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "20":
                        lockRule.TimeStartHour = "8";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "21":
                        lockRule.TimeStartHour = "9";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "22":
                        lockRule.TimeStartHour = "10";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    case "23":
                        lockRule.TimeStartHour = "11";
                        lockRule.TimeStartAM_PM = "PM";
                        break;
                    default:
                        break;
                }
                //Convvert Day
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
                

                // parse time stop information
                /*
                index1 = lockRule.TimeStop.IndexOf("_");
                index2 = lockRule.TimeStop.LastIndexOf("_");
                lockRule.TimeStopDay = lockRule.TimeStop.Substring(0, index1);
                lockRule.TimeStopHour = lockRule.TimeStop.Substring(index1+1, index2-index1-1);
                lockRule.TimeStopMinute = lockRule.TimeStop.Substring(index2+1, lockRule.TimeStop.Length-index2-1);
                */

                //Convert Information to display format
                // Convert Hour
                switch (lockRule.TimeStopHour)
                {
                    case "0":
                        lockRule.TimeStopHour = "12";
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "1":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "2":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "3":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "4":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "5":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "6":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "7":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "8":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "9":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "10":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "11":
                        lockRule.TimeStopAM_PM = "AM";
                        break;
                    case "12":
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "13":
                        lockRule.TimeStopHour = "1";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "14":
                        lockRule.TimeStopHour = "2";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "15":
                        lockRule.TimeStopHour = "3";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "16":
                        lockRule.TimeStopHour = "4";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "17":
                        lockRule.TimeStartHour = "5";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "18":
                        lockRule.TimeStopHour = "6";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "19":
                        lockRule.TimeStopHour = "7";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "20":
                        lockRule.TimeStopHour = "8";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "21":
                        lockRule.TimeStopHour = "9";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "22":
                        lockRule.TimeStopHour = "10";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    case "23":
                        lockRule.TimeStopHour = "11";
                        lockRule.TimeStopAM_PM = "PM";
                        break;
                    default:
                        break;
                }
                //Convvert Day
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

    [RelayCommand]
    async Task EditRule(Lock restriction)
    {
        // Set Start Rule
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

        IsEditLocks = true;

        isEditAdd = true;

    }

    [RelayCommand]
    async Task CancelEditRule()
    {
        IsEditLocks = false;
    }

    [RelayCommand]
    async Task AddRule()
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
        

        IsEditLocks = true;

        isEditAdd = false;
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
}
