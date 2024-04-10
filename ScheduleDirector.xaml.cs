using MySqlConnector;
using System;
using System.Collections.ObjectModel;

namespace wave;

public partial class ScheduleDirector : ContentPage
{
    private ObservableCollection<Lesson> LessonsList = new ObservableCollection<Lesson>();
    public ScheduleDirector()
    {
        InitializeComponent();

        var cs = ConnString.connString;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();
            var cmd = new MySqlCommand("SELECT l.lesson_id, l.lesson_day_id, CONCAT(l.lesson_start, ' - ', l.lesson_end) AS lessonTime, g.group_name AS groupName, r.room_name AS roomName, CONCAT(LEFT(u.user_name, 1), '.', LEFT(u.user_patronymic, 1), '.') AS teacherInitials FROM lesson l JOIN groups g ON l.lesson_group_id = g.group_id JOIN room r ON l.lesson_room_id = r.room_id JOIN teacher t ON g.group_teacher_id = t.teacher_id JOIN users u ON t.teacher_user_id = u.user_id;", con);

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var lessonId = (int)dr["lesson_id"];
                    var day = (int)dr["lesson_day_id"];
                    var room = dr["roomName"].ToString();
                    var lessonTime = dr["lessonTime"].ToString();
                    var groupName = dr["groupName"].ToString();
                    var teacher = dr["teacherInitials"].ToString(); 

                    LessonsList.Add(new Lesson() { LessonName = lessonId, Day = day, Room = room, Str = $"{lessonTime}  {groupName}  {teacher}" });
                }
            }
        }
        UpdateUIBasedOnConditions();
    }
    private void UpdateUIBasedOnConditions()
    {
        foreach (var lesson in LessonsList)
        {
            if (lesson.Room == "Англия" && lesson.Day == 1)
            {
                if (MondayLabel.Text != null)
                    MondayLabel.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    MondayLabel.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 2)
            {
                if (TuesdayLabel.Text != null)
                    TuesdayLabel.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    TuesdayLabel.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 3)
            {
                if (WednesdayLabel.Text != null)
                    WednesdayLabel.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    WednesdayLabel.Text = lesson.Str;
            }
        }
    }

    public class Lesson
    {
        public int LessonName { get; set; }
        public int Day { get; set; }
        public string Room { get; set; }
        public string Str { get; set; }
    }
}