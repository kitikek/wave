namespace wave;

public partial class Authorization : ContentPage
{
	public Authorization()
	{
		InitializeComponent();
	}
    private async void LoginButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the Home flyout item after button click
        await Shell.Current.GoToAsync("//Notification");
    }

}