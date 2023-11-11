using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

public class Activity
{

    public string ID { get; set; }
    public string InOut { get; set; }
    public string TimeStamp { get; set; }
    public string Image { get; set; }
}
