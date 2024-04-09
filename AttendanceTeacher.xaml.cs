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

                // Получение данных для всплывающего списка групп
                string groupQuery = "SELECT group_name FROM groups ORDER BY group_name ASC;";

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

            string groupName = "";
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

                //запросы
                string rowCountQuery = $"SELECT COUNT(*) FROM students_groups, groups WHERE groups.group_name=@GroupName AND groups.group_id=students_groups.group_id;";
                string columnCountQuery = $"SELECT COUNT(*) FROM groups JOIN lesson ON groups.group_name=@GroupName AND groups.group_id=lesson.lesson_group_id JOIN lesson_exact ON lesson.lesson_id=lesson_exact.lesson_exact_lesson_id;";
                string studentNamesQuery = $"SELECT CONCAT(users.user_surname, ' ', users.user_name) FROM students_groups JOIN groups ON students_groups.group_id=groups.group_id AND groups.group_name=@GroupName JOIN student ON student.student_id=students_groups.student_id JOIN users ON users.user_id=student.student_user_id GROUP BY users.user_surname ASC;;";
                string dateNamesQuery = $"SELECT lesson_exact_data FROM groups JOIN lesson ON groups.group_name=@GroupName AND groups.group_id=lesson.lesson_group_id JOIN lesson_exact ON lesson.lesson_id=lesson_exact.lesson_exact_lesson_id GROUP BY lesson_exact.lesson_exact_data ASC;";
                string attendanceQuery = $"SELECT lesson_visit FROM groups JOIN lesson ON lesson.lesson_group_id=groups.group_id AND groups.group_name=@GroupName JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id JOIN lesson_visit ON lesson_visit.lesson_visit_lesson_exact_id=lesson_exact.lesson_exact_id JOIN student ON student.student_id=lesson_visit.lesson_visit_student_id JOIN users ON users.user_id=student.student_user_id ORDER BY users.user_surname ASC, lesson_exact.lesson_exact_data ASC;";

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

                //Получение имен студентов
                var studentNames = new List<string>();
                using (var cmd = new MySqlCommand(studentNamesQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            studentNames.Add(reader.GetString(0).ToString());
                        }
                    }
                }

                //Получение имен дат
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

                //Заполнение первой ячейки
                Label studentLabel1 = new Label
                {
                    Text = @"Ученики\Даты",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetRow(studentLabel1, 0);
                Grid.SetColumn(studentLabel1, 0);
                AttendanceGrid.Children.Add(studentLabel1);

                //заполнение студентов
                for (int i = 0; i < rowCount; i++)
                {
                    Label studentLabel = new Label
                    {
                        Text = studentNames[i],
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(studentLabel, i + 1);
                    Grid.SetColumn(studentLabel, 0);
                    AttendanceGrid.Children.Add(studentLabel);
                }

                //заполнение дат
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

                //Получение посещаемости
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

                //Заполнение ячеек посещаемости
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
                for (int j = 1; j < columnCount + 1; j++)
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