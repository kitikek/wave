using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace wave;

public partial class AddScheduleDirector : Popup
{
    private ObservableCollection<Group> groups = new ObservableCollection<Group>();
    private ObservableCollection<Day> days = new ObservableCollection<Day>();
    private ObservableCollection<Room> rooms = new ObservableCollection<Room>();
    public AddScheduleDirector()
	{
		InitializeComponent();

        FillGroupPicker();
        FillDayPicker();
        FillRoomPicker();
    }
    private void FillGroupPicker()
    {
        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            using (var cmd = new MySqlCommand("SELECT * FROM groups ORDER BY group_name ASC", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    groups.Add(new Group
                    {
                        Id = reader.GetInt32("group_id"),
                        Name = reader.GetString("group_name")
                    });
                }
            }
        }
        GroupPicker.ItemsSource = groups;
    }

    private void FillDayPicker()
    {
        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            using (var cmd = new MySqlCommand("SELECT * FROM day", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    days.Add(new Day
                    {
                        Id = reader.GetInt32("day_id"),
                        Name = reader.GetString("day_name")
                    });
                }
            }
        }
        DayPicker.ItemsSource = days;
    }

    private void FillRoomPicker()
    {
        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            using (var cmd = new MySqlCommand("SELECT * FROM room ORDER BY room_name ASC", con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rooms.Add(new Room
                    {
                        Id = reader.GetInt32("room_id"),
                        Name = reader.GetString("room_name")
                    });
                }
            }
        }
        RoomPicker.ItemsSource = rooms;
    }

    public void AddLesson(object sender, EventArgs e)
    {
        var selectedGroup = (Group)GroupPicker.SelectedItem;
        var selectedDay = (Day)DayPicker.SelectedItem;
        var startTime = EntryLessonStart.Text;
        var endTime = EntryLessonEnd.Text;
        var selectedRoom = (Room)RoomPicker.SelectedItem;

        using (var con = new MySqlConnection(ConnString.connString))
        {
            con.Open();
            using (var cmd = new MySqlCommand("INSERT INTO lesson (lesson_id, lesson_group_id, lesson_day_id, lesson_start, lesson_end, lesson_room_id) VALUES (NULL, @groupId, @dayId, @lessonStart, @lessonEnd, @roomId)", con))
            {
                cmd.Parameters.AddWithValue("@groupId", selectedGroup.Id);
                cmd.Parameters.AddWithValue("@dayId", selectedDay.Id);
                cmd.Parameters.AddWithValue("@lessonStart", startTime);
                cmd.Parameters.AddWithValue("@lessonEnd", endTime);
                cmd.Parameters.AddWithValue("@roomId", selectedRoom.Id);

                int rowsAffected = cmd.ExecuteNonQuery();
            }
        }
    }
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Day
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public void ClosePopup(object sender, EventArgs e)
    {
        Close();
    }
}