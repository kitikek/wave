using MySqlConnector;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave
{
    public partial class AttendanceTeacher : ContentPage
    {
        public AttendanceTeacher()
        {
            InitializeComponent();
            var cs = ConnString.connString;
            using (var con = new MySqlConnection(cs))
            {
                con.Open();

                // Запросы для получения данных
                string groupQuery = "SELECT group_name FROM groups ORDER BY group_name ASC;";

                // Получение данных для всплывающего списка
                var groups = new List<string>();
                using (var cmd = new MySqlCommand(groupQuery, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(reader["group_name"].ToString());
                    }
                }
                GroupPicker.ItemsSource = groups; // Заполнение всплывающего списка
            }

            string groupName = "АЯ ПК 3";
            Load(groupName);
            GroupPicker.SelectedIndexChanged += (sender, e) =>
            {
                string selectedGroup = GroupPicker.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedGroup))
                {
                    AttendanceGrid.Children.Clear();
                    AttendanceGrid.RowDefinitions.Clear();
                    AttendanceGrid.ColumnDefinitions.Clear();
                    groupName = selectedGroup;
                    Load(groupName);
                }
            };
        }

        void Load(string groupName)
        {
            var cs = ConnString.connString;
            using (var con = new MySqlConnection(cs))
            {
                con.Open();

                string rowCountQuery = $"SELECT COUNT(*) FROM students_groups, groups WHERE groups.group_name=@GroupName AND groups.group_id=students_groups.group_id;";
                string columnCountQuery = $"SELECT COUNT(*) FROM groups JOIN lesson ON groups.group_name=@GroupName AND groups.group_id=lesson.lesson_group_id JOIN lesson_exact ON lesson.lesson_id=lesson_exact.lesson_exact_lesson_id;";
                string studentNamesQuery = $"SELECT student_id FROM students_groups, groups WHERE groups.group_name=@GroupName AND students_groups.group_id=groups.group_id GROUP BY student_id ASC;";
                string dateNamesQuery = $"SELECT lesson_exact_data FROM groups JOIN lesson ON groups.group_name=@GroupName AND groups.group_id=lesson.lesson_group_id JOIN lesson_exact ON lesson.lesson_id=lesson_exact.lesson_exact_lesson_id GROUP BY lesson_exact.lesson_exact_data ASC;";
                string attendanceQuery = $"SELECT lesson_visit FROM groups JOIN lesson ON lesson.lesson_group_id=groups.group_id AND groups.group_name=@GroupName JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id JOIN lesson_visit ON lesson_visit.lesson_visit_lesson_exact_id=lesson_exact.lesson_exact_id ORDER BY lesson_visit.lesson_visit_student_id ASC, lesson_exact.lesson_exact_data ASC;";

                // Получение количества строк и столбцов
                int rowCount, columnCount;
                using (var cmd = new MySqlCommand(rowCountQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    rowCount = Convert.ToInt32(cmd.ExecuteScalar()); // Получение количества строк
                }
                using (var cmd = new MySqlCommand(columnCountQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    columnCount = Convert.ToInt32(cmd.ExecuteScalar()); // Получение количества столбцов
                }

                // Создание таблицы с именами студентов и дат
                var studentNames = new List<string>();
                using (var cmd = new MySqlCommand(studentNamesQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            studentNames.Add(reader["student_id"].ToString());
                        }
                    }
                }

                var dateNames = new List<string>();
                using (var cmd = new MySqlCommand(dateNamesQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dateNames.Add(reader.GetDateTime(0).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                        }
                    }
                }

                for (int i = 0; i < rowCount; i++)
                {
                    Label studentLabel = new Label
                    {
                        Text = studentNames[i],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(studentLabel, i + 1);
                    Grid.SetColumn(studentLabel, 0);
                    AttendanceGrid.Children.Add(studentLabel);
                }

                for (int j = 0; j < columnCount; j++)
                {
                    Label dateLabel = new Label
                    {
                        Text = dateNames[j],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(dateLabel, 0);
                    Grid.SetColumn(dateLabel, j + 1);
                    AttendanceGrid.Children.Add(dateLabel);
                }

                var attendances = new List<string>();
                using (var cmd = new MySqlCommand(attendanceQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(reader["lesson_visit"].ToString());
                        }
                    }
                }

                // Пример заполнения ячеек таблицы
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        Label attendanceLabel = new Label
                        {
                            Text = attendances[i * columnCount + j],
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };
                        Grid.SetRow(attendanceLabel, i + 1);
                        Grid.SetColumn(attendanceLabel, j + 1);
                        AttendanceGrid.Children.Add(attendanceLabel);
                    }
                }
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                for (int j = 0; j < columnCount; j++)
                {
                    AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }
                for (int i = 0; i < rowCount + 1; i++)
                {
                    AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                }
            }
        }
    }
}