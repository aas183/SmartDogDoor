namespace SmartDogDoor.View;

public partial class LockPage : ContentPage
{
     public LockPage(LockViewModel viewModel)
     {
         InitializeComponent();
         BindingContext = viewModel;
     }
}
