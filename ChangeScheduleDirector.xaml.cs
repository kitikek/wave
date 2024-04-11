using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace wave;

public partial class ChangeScheduleDirector : Popup
{
    public ObservableCollection<Lesson> Items { get; } = new ObservableCollection<Lesson>();
    public ChangeScheduleDirector()
    {
        InitializeComponent();

        foreach(var lesson in ScheduleDirector.LessonsList)
        {
            Items.Add(new Lesson
            {
                LessonName = lesson.LessonName,
                Day = lesson.Day,
                Room = lesson.Room,
                Str = lesson.Str
            });
        }
        BindingContext = this;
    }
    public class Lesson
    {
        public int LessonName { get; set; }
        public int Day { get; set; }
        public string Room { get; set; }
        public string Str { get; set; }
    }
    public async void ClosePopup(object sender, EventArgs e)
    {
        Close();
    }
}