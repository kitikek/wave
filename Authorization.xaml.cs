using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Graphics;
using MySqlConnector;

namespace wave;

public partial class Authorization : ContentPage
{
    public static string Login { get; set; }
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
        Login = LoginEntry.Text;        
        if (!WhoAreYou.isParentSelected)
        {
            await Shell.Current.GoToAsync("//Notification");
        }
        else
        {
            //var cs = ConnString.connString;

            //using (var con = new MySqlConnection(cs))
            //{
            //    con.Open();

            //    var cmd = new MySqlCommand(@"SELECT p.parent_id FROM parent p JOIN users u ON p.parent_user_id = u.user_id WHERE u.user_login = Login;", con);
                
            //    using (var dr = cmd.ExecuteReader())
            //    {
            //        while (dr.Read())
            //        {
                        
            //        }
            //    }

            //}

                await Shell.Current.GoToAsync("//ChildChoose");
        }
    }
  
}