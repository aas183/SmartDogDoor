using System.Text.Json.Serialization;

namespace SmartDogDoor.Model;

//class for pet infromation database table information
public class Pet
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string InOut { get; set; }
    public Color InOutColor { get; set; }

}
