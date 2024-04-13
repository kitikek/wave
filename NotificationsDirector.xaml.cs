using MySqlConnector;
using System.Globalization;

namespace wave;

public partial class NotificationsDirector : ContentPage
{
    Entry TextEntry;
    DatePicker Datepicker;
    StackLayout NotificationStack;

    public NotificationsDirector()
    {
        InitializeComponent();

        TextEntry = new Entry
        {
            Placeholder = "Введите текст уведомления"
        };

        Datepicker = new DatePicker
        {
            Format = "yyyy-mm-dd",
            MinimumDate = DateTime.Now
        };

        Button saveButton = new Button
        {
            Text = "Сохранить"
        };
        saveButton.Clicked += SaveNotification_Clicked;

        NotificationStack = new StackLayout();

        StackLayout mainStack = new StackLayout
        {
            Children =
            {
                TextEntry,
                Datepicker,
                saveButton,
                NotificationStack
            }
        };

        Content = mainStack;

        UpdateNotificationsView();
    }

    private void SaveNotification_Clicked(object sender, EventArgs e)
    {
        string notificationText = TextEntry.Text;
        DateTime notificationDate = Datepicker.Date;

        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            string sql = "INSERT INTO notification (notification_text, notification_date) VALUES (@text, @date);";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@text", notificationText);
            cmd.Parameters.AddWithValue("@date", notificationDate);
            cmd.ExecuteNonQuery();
        }

        UpdateNotificationsView();
    }

    private void UpdateNotificationsView()
    {
        NotificationStack.Children.Clear();

        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            string sql = "SELECT notification_text, notification_date FROM notification ORDER BY notification_date DESC;";
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