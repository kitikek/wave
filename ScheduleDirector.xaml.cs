using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System;
using System.Collections.ObjectModel;

namespace wave;

public partial class ScheduleDirector : ContentPage
{
    public static ObservableCollection<Lesson> LessonsList = new ObservableCollection<Lesson>();
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
        BindingContext = this;
        UpdateUIBasedOnConditions();
    }
    private void UpdateUIBasedOnConditions()
    {
        foreach (var lesson in LessonsList)
        {
            #region England
            if (lesson.Room == "Англия" && lesson.Day == 1)
            {
                if (MondayLabelEngland.Text != null)
                    MondayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    MondayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 2)
            {
                if (TuesdayLabelEngland.Text != null)
                    TuesdayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    TuesdayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 3)
            {
                if (WednesdayLabelEngland.Text != null)
                    WednesdayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    WednesdayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 4)
            {
                if (ThursdayLabelEngland.Text != null)
                    ThursdayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    ThursdayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 5)
            {
                if (FridayLabelEngland.Text != null)
                    FridayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    FridayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 6)
            {
                if (SaturdayLabelEngland.Text != null)
                    SaturdayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SaturdayLabelEngland.Text = lesson.Str;
            }
            if (lesson.Room == "Англия" && lesson.Day == 7)
            {
                if (SundayLabelEngland.Text != null)
                    SundayLabelEngland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SundayLabelEngland.Text = lesson.Str;
            }
            #endregion

            #region Ireland
            if (lesson.Room == "Ирландия" && lesson.Day == 1)
            {
                if (MondayLabelIreland.Text != null)
                    MondayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    MondayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 2)
            {
                if (TuesdayLabelIreland.Text != null)
                    TuesdayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    TuesdayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 3)
            {
                if (WednesdayLabelIreland.Text != null)
                    WednesdayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    WednesdayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 4)
            {
                if (ThursdayLabelIreland.Text != null)
                    ThursdayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    ThursdayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 5)
            {
                if (FridayLabelIreland.Text != null)
                    FridayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    FridayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 6)
            {
                if (SaturdayLabelIreland.Text != null)
                    SaturdayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SaturdayLabelIreland.Text = lesson.Str;
            }
            if (lesson.Room == "Ирландия" && lesson.Day == 7)
            {
                if (SundayLabelIreland.Text != null)
                    SundayLabelIreland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SundayLabelIreland.Text = lesson.Str;
            }
            #endregion

            #region Scotland
            if (lesson.Room == "Шотландия" && lesson.Day == 1)
            {
                if (MondayLabelScotland.Text != null)
                    MondayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    MondayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 2)
            {
                if (TuesdayLabelScotland.Text != null)
                    TuesdayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    TuesdayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 3)
            {
                if (WednesdayLabelScotland.Text != null)
                    WednesdayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    WednesdayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 4)
            {
                if (ThursdayLabelScotland.Text != null)
                    ThursdayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    ThursdayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 5)
            {
                if (FridayLabelScotland.Text != null)
                    FridayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    FridayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 6)
            {
                if (SaturdayLabelScotland.Text != null)
                    SaturdayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SaturdayLabelScotland.Text = lesson.Str;
            }
            if (lesson.Room == "Шотландия" && lesson.Day == 7)
            {
                if (SundayLabelScotland.Text != null)
                    SundayLabelScotland.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SundayLabelScotland.Text = lesson.Str;
            }
            #endregion

            #region Wales
            if (lesson.Room == "Уэльс" && lesson.Day == 1)
            {
                if (MondayLabelWales.Text != null)
                    MondayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    MondayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 2)
            {
                if (TuesdayLabelWales.Text != null)
                    TuesdayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    TuesdayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 3)
            {
                if (WednesdayLabelWales.Text != null)
                    WednesdayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    WednesdayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 4)
            {
                if (ThursdayLabelWales.Text != null)
                    ThursdayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    ThursdayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 5)
            {
                if (FridayLabelWales.Text != null)
                    FridayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    FridayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 6)
            {
                if (SaturdayLabelWales.Text != null)
                    SaturdayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SaturdayLabelWales.Text = lesson.Str;
            }
            if (lesson.Room == "Уэльс" && lesson.Day == 7)
            {
                if (SundayLabelWales.Text != null)
                    SundayLabelWales.Text += $"{Environment.NewLine}{lesson.Str}";
                else
                    SundayLabelWales.Text = lesson.Str;
            }
            #endregion
        }
    }

    public class Lesson
    {
        public int LessonName { get; set; }
        public int Day { get; set; }
        public string Room { get; set; }
        public string Str { get; set; }
    }
    public async void ChangeButtonClicked(object sender, EventArgs e)
    {
        this.ShowPopup(new ChangeScheduleDirector());
    }
}