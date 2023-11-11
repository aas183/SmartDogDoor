using SmartDogDoor.Services;
namespace SmartDogDoor.ViewModel;

public partial class PetViewModel : BaseViewModel
{
    PetService petService;
    public ObservableCollection<Pet> Pets { get; } = new();
    public PetViewModel(PetService petService)
    {
        Title = "Pets";
        this.petService = petService;
    }

    [RelayCommand]
    async Task GoToDetailsAsync(Pet pet)
    {
        if (pet is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
            new Dictionary<string, object>
            {
                {"Pet", pet }
            });
    }

    [RelayCommand]
    async Task GetPetsAsync ()
    {
        if(IsBusy) return;

        try
        {
            IsBusy = true;
            var pets = await petService.GetPets();

            if(Pets.Count != 0)
                Pets.Clear();

            foreach (var pet in pets)
                Pets.Add(pet);
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
}
