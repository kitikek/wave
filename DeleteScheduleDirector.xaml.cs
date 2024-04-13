using CommunityToolkit.Maui.Views;
using MySqlConnector;

namespace wave;

public partial class DeleteScheduleDirector : Popup
{
    public DeleteScheduleDirector()
    {
        InitializeComponent();
    }
    public void DeleteButtonClicked(object sender, EventArgs e)
    {
        var id = EntryId.Text;
        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            var cmd = new MySqlCommand("DELETE FROM lesson WHERE lesson.lesson_id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
    }
}