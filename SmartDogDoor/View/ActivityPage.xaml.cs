namespace SmartDogDoor.View;

public partial class ActivityPage : ContentPage
{
     public ActivityPage(ActivityViewModel viewModel)
     {
         InitializeComponent();
         BindingContext = viewModel;
     }
}