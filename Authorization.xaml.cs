using Microsoft.Extensions.Logging.Abstractions;

namespace wave;

public partial class Authorization : ContentPage
{
	public Authorization()
	{
		InitializeComponent();

        // Subscribe to TextChanged events of the Entry fields
        LoginEntry.TextChanged += HandleTextChanged;
        PasswordEntry.TextChanged += HandleTextChanged;

        // Disable the button initially
        LoginButton.IsEnabled = false;
    }
    private void HandleTextChanged(object sender, TextChangedEventArgs e)
    {
        // Check if both login and password fields are not empty
        if (!string.IsNullOrEmpty(LoginEntry.Text) && !string.IsNullOrEmpty(PasswordEntry.Text))
        {
            // Enable the button if both fields have content
            LoginButton.IsEnabled = true;
            LoginButton.BackgroundColor = Colors.BlueViolet;
        }
        else
        {
            // Otherwise, disable the button
            LoginButton.IsEnabled = false;
        }
    }
    private async void LoginButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the Home flyout item after button click
        await Shell.Current.GoToAsync("//Notification");
    }

}