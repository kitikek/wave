using MySqlConnector;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave;

public class GradeItem
{
    public string? Name { get; set; }
    public string? Date { get; set; }
    public string? Grade { get; set; }
}

public partial class Grade : ContentPage
{
    public Grade()
    {
        InitializeComponent();

        var items = new List<GradeItem> { new GradeItem { Name = "Тест:", Date = "Дата:", Grade = "Оценка:" } };
        var cs = ConnString.connString;
        int id = 0;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*), student_id FROM student JOIN users ON student_user_id=user_id AND user_login=@l AND user_password=@p", con))
            {
                cmd.Parameters.AddWithValue("@l", Authorization.Login);
                cmd.Parameters.AddWithValue("@p", Authorization.Password);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
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
            string sql = "SELECT test.test_name, test.test_date, test_visit.test_visit_mark " +
                "FROM test, test_visit " +
                "WHERE test_visit.test_visit_student_id=@id AND test_visit.test_visit_test_id=test.test_id " +
                "ORDER BY test.test_date DESC;";
            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string formattedDate = reader.GetDateTime(1).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    items.Add(new GradeItem { Name = reader.GetString(0), Date = formattedDate, Grade = reader.GetString(2) });
                }
            }
        }

        if (items.Count == 1)
        {
            items.Add(new GradeItem { Name = "Оценок пока нет", Date = "Никогда", Grade = "Ничего" });
        };

        gradeList.ItemsSource = items;
    }
}