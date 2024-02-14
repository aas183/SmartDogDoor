using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

public class Lock
{
    public int Id { get; set; }
    public string TimeStart { get; set; }
    public string TimeSop { get; set; }
  
}