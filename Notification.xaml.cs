using MySqlConnector;
using System.Globalization;

namespace wave;

public partial class Notification : ContentPage
{
    public Notification()
    {
        InitializeComponent();

        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            string sql = "SELECT notification_text, notification_date FROM notification;";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string formattedDate = reader.GetDateTime(1).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

                Label dateLabel = new Label
                {
                    Text = formattedDate,
                    FontFamily = "Arial",
                    TextColor = Color.FromHex("#2D4D8F") 
                };

                Frame notificationFrame = new Frame
                {
                    BorderColor = Color.FromHex("#2D4D8F"),
                    CornerRadius = 5,
                    Padding = 10,
                    Margin = new Thickness(10),
                    Content = new StackLayout
                    {
                        Children =
                            {
                                dateLabel,
                                new Label
                                {
                                    Text = reader.GetString(0),
                                    TextColor = Color.FromHex("#2D4D8F"),
                                    FontFamily = "Gill Sans"
                                }
                            }
                    }
                };

                NotificationStack.Children.Add(notificationFrame);
            }
        }

        if (NotificationStack.Children.Count == 0)
        {
            Label noNotificationLabel = new Label
            {
                Text = "Уведомлений сейчас нет(",
                TextColor = Color.FromHex("#2D4D8F"),
                FontFamily = "Gill Sans",
                HorizontalOptions = LayoutOptions.Center
            };

            NotificationStack.Children.Add(noNotificationLabel);
        }

    }
}




    

	
