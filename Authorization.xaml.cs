using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using MySqlConnector;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace wave;

public partial class Authorization : ContentPage
{
    public static string Login { get; set; }
    public static string Password { get; set; }
    public static string HashedPassword { get; set; }
    private bool isPasswordVisible = false;
    private Color originalEntryColor;
    public Authorization()
	{
		InitializeComponent();
        LoginEntry.TextChanged += HandleTextChanged;
        PasswordEntry.TextChanged += HandleTextChanged;
        LoginButton.IsEnabled = false;
        originalEntryColor = LoginEntry.BackgroundColor;
    }
    public bool ValidateUser(string login, string password)
    {
        bool isValid = false;
        int selectedUserTypeId = -1;

        if (WhoAreYou.isDirectorSelected)
        {
            selectedUserTypeId = 1;
        }
        else if (WhoAreYou.isTeacherSelected)
        {
            selectedUserTypeId = 2;
        }
        else if (WhoAreYou.isStudentSelected)
        {
            selectedUserTypeId = 3;
        }
        else if (WhoAreYou.isParentSelected)
        {
            selectedUserTypeId = 4;
        }
        else
        {
            return false;
        }

        string connectionString = ConnString.connString;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT user_login, user_password FROM users WHERE user_login = @login AND user_type_id = @userTypeId";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@userTypeId", selectedUserTypeId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string hashedPasswordFromDb = reader.GetString("user_password");

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                        byte[] passwordHashBytes = sha256.ComputeHash(passwordBytes);
                        HashedPassword = BitConverter.ToString(passwordHashBytes).Replace("-", "").ToLower();

                        if (HashedPassword == hashedPasswordFromDb)
                        {
                            isValid = true;
                        }
                    }
                }
            }

            connection.Close();
        }

        return isValid;
    }
    private void HandleTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(LoginEntry.Text) && !string.IsNullOrEmpty(PasswordEntry.Text))
        {
            LoginButton.IsEnabled = true;
            LoginButton.BackgroundColor = Color.FromHex("#0c447f");
        }
        else
        {
            LoginButton.IsEnabled = false;
        }
    }
    private void PasswordVisibilityButton_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        if (PasswordEntry.IsPassword)
        {
            PasswordVisibilityButton.Source = "hidepassword.png";
            PasswordVisibilityLabel.Text = "Показать пароль";
        }
        else
        {
            PasswordVisibilityButton.Source = "showpassword.png";
            PasswordVisibilityLabel.Text = "Скрыть пароль";
        }
    }
    private async void LoginButtonClicked(object sender, EventArgs e)
    {
        if (ValidateUser(LoginEntry.Text, PasswordEntry.Text))
        {
            Login = LoginEntry.Text;
            Password = PasswordEntry.Text;

            LoginEntry.Text = "";
            PasswordEntry.Text = "";

            if (WhoAreYou.isDirectorSelected)
            {
                await Shell.Current.GoToAsync("//Director");
            }
            else if (WhoAreYou.isParentSelected)
            {
                await Shell.Current.GoToAsync("//ChildChoose");
            }
            else if (WhoAreYou.isTeacherSelected)
            {
                await Shell.Current.GoToAsync("//Teacher");
            }
            else if (WhoAreYou.isStudentSelected)
            {
                await Shell.Current.GoToAsync("//Student");
            }

            ErrorMessageLabel.IsVisible = false;
            LoginEntry.BackgroundColor = originalEntryColor;
            PasswordEntry.BackgroundColor = originalEntryColor;
        }
        else
        {
            LoginEntry.BackgroundColor = Color.FromHex("#FFEEEE");
            PasswordEntry.BackgroundColor = Color.FromHex("#FFEEEE");
            ErrorMessageLabel.Text = "Неверный логин или пароль";
            ErrorMessageLabel.IsVisible = true;
        }
    }
    
    //else
    //{
    //    //var cs = ConnString.connString;

    //    //using (var con = new MySqlConnection(cs))
    //    //{
    //    //    con.Open();

    //    //    int id = 0;
    //    //    var cmd = new MySqlCommand(@"SELECT p.parent_id FROM parent p JOIN users u ON p.parent_user_id = u.user_id WHERE u.user_login = @Login;", con);
    //    //    cmd.Parameters.AddWithValue("@Login", Login);

    //    //    using (var dr = cmd.ExecuteReader())
    //    //    {
    //    //        while (dr.Read())
    //    //        {
    //    //            id = (int)dr["parent_id"];
    //    //        }
    //    //    }
    //    //    cmd = new MySqlCommand(@"SELECT COUNT(*) AS children_count FROM student WHERE student_parent_id = @id;", con);
    //    //    cmd.Parameters.AddWithValue("@id", id);
    //    //    using (var dr = cmd.ExecuteReader())
    //    //    {
    //    //        while (dr.Read())
    //    //        {
    //    //            ChildChoose.childCount = (int)dr["children_count"];
    //    //        }

    //    //    }
    //    //    cmd = new MySqlCommand(@"SELECT student_id FROM student WHERE student_parent_id = @id;", con);
    //    //    cmd.Parameters.AddWithValue("@id", id);
    //    //    using (var dr = cmd.ExecuteReader())
    //    //    {
    //    //        while (dr.Read())
    //    //        {
    //    //            id = (int)dr["student_id"];
    //    //            var name = new MySqlCommand(@"SELECT user_name FROM users WHERE user_id = @id;", con);
    //    //            name.Parameters.AddWithValue("@id", id);
    //    //            while (dr.Read())
    //    //            {
    //    //                ChildChoose.children.Append(dr["user_name"].ToString());
    //    //            }

    //    //        }

    //    //    }

    //    //}

    //    //await Shell.Current.GoToAsync("//Notification");
    //}
}
  
