using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace wave;

public partial class ChangeScheduleDirector : Popup
{
    public ObservableCollection<Lesson> Items { get; } = new ObservableCollection<Lesson>();
    public ChangeScheduleDirector()
    {
        InitializeComponent();

        foreach (var lesson in ScheduleDirector.LessonsList)
        {
            Items.Add(new Lesson
            {
                Id = lesson.LessonName,
                Day = lesson.DayName,
                Room = lesson.Room,
                Time = lesson.Time,
                Group = lesson.Group,
                Teacher = lesson.Teacher,

            }); ;
        }
        BindingContext = this;
    }
    public void ChangeButtonClicked(object sender, EventArgs e)
    {
        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            int roomId;
            int dayId;
            int groupId;

            // Получение roomId
            using (var roomCommand = new MySqlCommand("SELECT room_id FROM room WHERE room_name = @Room", con))
            {
                roomCommand.Parameters.AddWithValue("@Room", EntryRoom.Text);
                roomId = Convert.ToInt32(roomCommand.ExecuteScalar());
            }

            // Получение dayId
            using (var dayCommand = new MySqlCommand("SELECT day_id FROM day WHERE day_name = @Day", con))
            {
                dayCommand.Parameters.AddWithValue("@Day", EntryDay.Text);
                dayId = Convert.ToInt32(dayCommand.ExecuteScalar());
            }

            // Получение groupId
            using (var groupCommand = new MySqlCommand("SELECT group_id FROM groups WHERE group_name = @Group", con))
            {
                groupCommand.Parameters.AddWithValue("@Group", EntryGroup.Text);
                groupId = Convert.ToInt32(groupCommand.ExecuteScalar());
            }

            using (var command = new MySqlCommand("UPDATE lesson SET lesson_day_id = @DayId, lesson_room_id = @RoomId, lesson_start = @TimeStart, lesson_end = @TimeEnd, lesson_group_id = @GroupId WHERE lesson_id = @Id", con))
            {
                command.Parameters.AddWithValue("@DayId", dayId);
                command.Parameters.AddWithValue("@RoomId", roomId);
                command.Parameters.AddWithValue("@TimeStart", EntryTimeStart.Text);
                command.Parameters.AddWithValue("@TimeEnd", EntryTimeEnd.Text);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@Id", EntryId.Text);

                command.ExecuteNonQuery();
            }
        }
        Close();

    }

    public class Lesson
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string Room { get; set; }
        public string Time { get; set; }
        public string Group { get; set; }
        public string Teacher { get; set; }

    }
}
    
