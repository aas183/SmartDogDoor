using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

public class Lock
{
    public int Id { get; set; }
    public string TimeStart { get; set; }
    public string TimeStop { get; set; }
    public int ruleNumber { get; set; }
    public string TimeStartDay { get; set; }
    public string TimeStartHour { get; set; }
    public string TimeStartMinute { get; set; }
    public string TimeStopDay { get; set; }
    public string TimeStopHour { get; set; }
    public string TimeStopMinute { get; set; }

}