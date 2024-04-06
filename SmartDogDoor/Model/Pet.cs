using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

// Model for pet
public class Pet
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string InOut { get; set; }
    public Color InOutColor { get; set; }

}
