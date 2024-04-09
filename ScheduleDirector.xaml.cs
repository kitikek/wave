using System;
using System.Collections.ObjectModel;

namespace wave;

public partial class ScheduleDirector : ContentPage
{
    static Random random = new();
    public ObservableCollection<Student> Items { get; } = new();
    public ScheduleDirector()
	{
		InitializeComponent();

        for (int i = 0; i < 10; i++)
        {
            Items.Add(new Student
            {
                Id = i,
                Name = "Person " + i,
                Age = random.Next(14, 85),
            });
        }
    }
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}