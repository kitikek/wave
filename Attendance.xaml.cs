using CommunityToolkit.Maui.Converters;
using MySqlConnector;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave;

public class AttendanceItem
{
    public string? Date { get; set; }
    public string? Group { get; set; }
    public string? Visit { get; set; }
}

public partial class Attendance : ContentPage
{
    public Attendance()
    {
        InitializeComponent();

        var items = new List<AttendanceItem> { new AttendanceItem { Date = "Дата:", Group = "Группа:", Visit = "Отметка:" } };
        var cs = ConnString.connString;
        int id = 0;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*), student_id FROM student JOIN users ON student_user_id=user_id AND user_login=@l AND user_password=@p", con))
            {
                cmd.Parameters.AddWithValue("@l", Authorization.Login);
                cmd.Parameters.AddWithValue("@p", Authorization.HashedPassword);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (reader.GetInt32(0) > 0)
                    {
                        id = reader.GetInt32(1);
                    }
                }
            }
        }

        using (var con = new MySqlConnection(cs))
        {
            con.Open();
            string sql = "SELECT lesson_exact.lesson_exact_data, groups.group_name, lesson_visit.lesson_visit " +
                "FROM lesson_visit " +
                "JOIN student " +
                "ON student.student_id=@id AND student.student_id=lesson_visit.lesson_visit_student_id " +
                "JOIN lesson_exact " +
                "ON lesson_visit.lesson_visit_lesson_exact_id=lesson_exact.lesson_exact_id " +
                "JOIN lesson " +
                "ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id " +
                "JOIN groups " +
                "ON lesson.lesson_group_id=groups.group_id " +
                "ORDER BY lesson_exact.lesson_exact_data DESC;";
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string formattedDate = reader.GetDateTime(0).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    items.Add(new AttendanceItem { Date = formattedDate, Group = reader.GetString(1), Visit = reader.GetString(2) });
                }
            }
        }

        if (items.Count == 1)
        {
            items.Add(new AttendanceItem { Date = "Оценок пока нет", Group="", Visit="" });
        };

        attendanceList.ItemsSource = items;
    }
}