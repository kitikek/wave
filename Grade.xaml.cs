using MySqlConnector;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave;

public class GradeItem
{
    public string Name { get; set; }
    public string Date { get; set; }
    public string Grade { get; set; }
}

public partial class Grade : ContentPage
{
    public Grade()
    {
        InitializeComponent();

        var items = new List<GradeItem>();
        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            string sql = "SELECT test.test_name, test.test_date, test_visit.test_visit_mark FROM test, test_visit WHERE test_visit.test_visit_student_id=1 AND test_visit.test_visit_test_id=test.test_id ORDER BY test.test_date DESC;";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string formattedDate = reader.GetDateTime(1).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                items.Add(new GradeItem { Name = reader.GetString(0), Date = formattedDate, Grade = reader.GetString(2) });
            }
        }

        if (items.Count == 0)
        {
            items.Add(new GradeItem { Name = "ќценок пока нет"});
        };

        gradeList.ItemsSource = items;
    }
}