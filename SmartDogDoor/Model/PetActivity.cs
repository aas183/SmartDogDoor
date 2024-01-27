using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

public class PetActivity
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string InOut { get; set; }
    public Color InOutColor { get; set; }
    public string TimeStamp { get; set; }
    public string Image { get; set; }
}
