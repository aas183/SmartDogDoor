using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

// Model for locking restrictions
public class Lock
{
    public int Id { get; set; }

    public string TimeStartDay { get; set; }
    public string TimeStartHour { get; set; }
    public string TimeStartMinute { get; set; }
    public string TimeStopDay { get; set; }
    public string TimeStopHour { get; set; }
    public string TimeStopMinute { get; set; }
    public string TimeStartDisplay { get; set; }
    public string TimeStartAM_PM { get; set; }
    public string TimeStopAM_PM { get; set; }
    public string TimeStopDisplay { get; set; }
    public int ruleNumber { get; set; }

}