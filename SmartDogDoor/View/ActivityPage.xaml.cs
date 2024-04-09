using Microsoft.Maui.Controls;

namespace SmartDogDoor.View;

public partial class ActivityPage : ContentPage
{
    ActivityViewModel ViewModel;
     public ActivityPage(ActivityViewModel viewModel)
     {
         InitializeComponent();
        // ViewModel = viewModel;
         BindingContext = viewModel;
    }

    
    void OnTypePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex == 0)
        {
            petPicker.IsVisible = false;

        }
        else
        {
            petPicker.IsVisible = true;
        }
    }

    void zoomActivity(object sender, EventArgs e)
    {
        ScrollView.ScrollToAsync(0, 0, false);
    }
}