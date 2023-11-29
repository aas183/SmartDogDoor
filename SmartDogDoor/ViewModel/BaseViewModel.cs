namespace SmartDogDoor.ViewModel;


public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;//For keeping track if app is busy

    [ObservableProperty]
    string title;//for title of each page

    public bool IsNotBusy => !IsBusy;
}
