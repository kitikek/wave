using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MySqlConnector;
using System.Linq;

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

            //    int id = 0;
            //    var cmd = new MySqlCommand(@"SELECT p.parent_id FROM parent p JOIN users u ON p.parent_user_id = u.user_id WHERE u.user_login = @Login;", con);
            //    cmd.Parameters.AddWithValue("@Login", Login);

            //    using (var dr = cmd.ExecuteReader())
            //    {
            //        while (dr.Read())
            //        {
            //            id = (int)dr["parent_id"];
            //        }
            //    }
            //    cmd = new MySqlCommand(@"SELECT COUNT(*) AS children_count FROM student WHERE student_parent_id = @id;", con);
            //    cmd.Parameters.AddWithValue("@id", id);
            //    using (var dr = cmd.ExecuteReader())
            //    {
            //        while (dr.Read())
            //        {
            //            ChildChoose.childCount = (int)dr["children_count"];
            //        }

            //    }
            //    cmd = new MySqlCommand(@"SELECT student_id FROM student WHERE student_parent_id = @id;", con);
            //    cmd.Parameters.AddWithValue("@id", id);
            //    using (var dr = cmd.ExecuteReader())
            //    {
            //        while (dr.Read())
            //        {
            //            id = (int)dr["student_id"];
            //            var name = new MySqlCommand(@"SELECT user_name FROM users WHERE user_id = @id;", con);
            //            name.Parameters.AddWithValue("@id", id);
            //            while (dr.Read())
            //            {
            //                ChildChoose.children.Append(dr["user_name"].ToString());
            //            }
                        
            //        }

            //    }

            //}

            await Shell.Current.GoToAsync("//Notification");
        }
    }
  
}