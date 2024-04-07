namespace wave;

public partial class WhoAreYou : ContentPage
{
	public WhoAreYou()
	{
		InitializeComponent();
	}
    private async void StudentButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Authorization");
    }
}