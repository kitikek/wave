using MySqlConnector;
using System.Globalization;

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

        var items = new List<AttendanceItem>();
        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            string sql = "SELECT lesson_visit.lesson_visit_date, groups.group_name, lesson_visit.lesson_visit " +
                "FROM lesson_visit " +
                "JOIN student " +
                "ON student.student_id=1 AND student.student_id=lesson_visit.lesson_visit_student_id " +
                "JOIN lesson " +
                "ON lesson_visit.lesson_visit_lesson_id=lesson.lesson_id " +
                "JOIN groups " +
                "ON lesson.lesson_group_id=groups.group_id " +
                "ORDER BY lesson_visit.lesson_visit_date DESC;";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string formattedDate = reader.GetDateTime(0).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                items.Add(new AttendanceItem { Date = formattedDate, Group = reader.GetString(1), Visit = reader.GetString(2) });
            }
        }

        if (items.Count == 0)
        {
            items.Add(new AttendanceItem { Group = "ќценок пока нет" });
        };

        attendanceList.ItemsSource = items;
    }
}