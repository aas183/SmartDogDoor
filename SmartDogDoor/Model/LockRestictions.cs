using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

public class LockRetrictions
{

    public string TimeStart { get; set; }
    public string TimeSop { get; set; }
    public string LockUnlock { get; set; }
  
}