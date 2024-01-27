namespace SmartDogDoor;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(View.DetailsPage), typeof(View.DetailsPage));
    }
}
