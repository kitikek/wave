using MySqlConnector;

namespace wave;

public partial class Scedule : ContentPage
{
    public Scedule()
    {
        InitializeComponent();
        LoadSchedule();
        
    }

    public void LoadSchedule()
    {
        LessonsStackLayout.Children.Clear();
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string desiredLogin = Authorization.Login;
            if (WhoAreYou.isStudentSelected)
            {

                int studentId = GetStudentIdByLogin(connection, desiredLogin);

                if (studentId != 0)
                {
                    int groupId = GetGroupIdByStudentId(connection, studentId);

                    if (groupId != 0)
                    {
                        List<string> lessonsInfo = GetLessonsInfoByGroupId(connection, groupId);

                        if (lessonsInfo.Count == 0)
                        {
                            Label noLessonsLabel = new Label
                            {
                                Text = "Расписания пока нет",
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.CenterAndExpand
                            };

                            LessonsStackLayout.Children.Add(noLessonsLabel);
                        }
                        else
                        {
                            foreach (string lessonInfo in lessonsInfo)
                            {
                                var frame = new Frame
                                {
                                    BackgroundColor = Color.FromHex("#ffffff"),
                                    Content = new Label { Text = lessonInfo },
                                    Margin = new Thickness(10)
                                };

                                LessonsStackLayout.Children.Add(frame);
                            }
                        }
                    }
                    else
                    {
                        Label groupNotFoundErrorLabel = new Label
                        {
                            Text = "Не найдена группа",
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        };

                        LessonsStackLayout.Children.Add(groupNotFoundErrorLabel);
                    }
                }
                else
                {
                    Label studentNotFoundErrorLabel = new Label
                    {
                        Text = "Не найден ученик",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    LessonsStackLayout.Children.Add(studentNotFoundErrorLabel);
                }
            }
            else if (WhoAreYou.isTeacherSelected)
            {
                int teacherId = GetTeacherIdByLogin(connection, desiredLogin);

                if (teacherId != 0)
                {
                    List<string> teacherLessonsInfo = GetLessonsInfoByTeacherId(connection, teacherId);

                    if (teacherLessonsInfo.Count == 0)
                    {
                        Label noLessonsLabel = new Label
                        {
                            Text = "Расписания пока нет",
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        };

                        LessonsStackLayout.Children.Add(noLessonsLabel);
                    }
                    else
                    {
                        foreach (string lessonInfo in teacherLessonsInfo)
                        {
                            var frame = new Frame
                            {
                                BackgroundColor = Color.FromHex("#ffffff"),
                                Content = new Label { Text = lessonInfo },
                                Margin = new Thickness(10)
                            };

                            LessonsStackLayout.Children.Add(frame);
                        }
                    }
                }
                else
                {
                    Label teacherNotFoundErrorLabel = new Label
                    {
                        Text = "Не найден учитель",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    LessonsStackLayout.Children.Add(teacherNotFoundErrorLabel);
                }
            }
            else if (WhoAreYou.isParentSelected)
            {
                int childId = GetStudentIdByUserId(connection, ChildChoose.chosenId);
                if (childId != 0)
                {
                    int groupId = GetGroupIdByStudentId(connection, childId);
                    if (groupId != 0)
                    {
                        List<string> lessonsInfo = GetLessonsInfoByGroupId(connection, groupId);

                        if (lessonsInfo.Count == 0)
                        {
                            Label noLessonsLabel = new Label
                            {
                                Text = "Расписания пока нет",
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                VerticalOptions = LayoutOptions.CenterAndExpand
                            };

                            LessonsStackLayout.Children.Add(noLessonsLabel);
                        }
                        else
                        {
                            foreach (string lessonInfo in lessonsInfo)
                            {
                                var frame = new Frame
                                {
                                    BackgroundColor = Color.FromHex("#ffffff"),
                                    Content = new Label { Text = lessonInfo },
                                    Margin = new Thickness(10)
                                };

                                LessonsStackLayout.Children.Add(frame);
                            }
                        }
                    }
                    else
                    {
                        Label groupNotFoundErrorLabel = new Label
                        {
                            Text = "Не найдена группа",
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        };

                        LessonsStackLayout.Children.Add(groupNotFoundErrorLabel);
                    }
                }
                else
                {
                    Label studentNotFoundErrorLabel = new Label
                    {
                        Text = "Не найден ученик",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    LessonsStackLayout.Children.Add(studentNotFoundErrorLabel);
                }
            }

            connection.Close();
        }
    }

    private int GetStudentIdByLogin(MySqlConnection connection, string login)
    {
        string query = "SELECT student_id FROM student s JOIN users u ON s.student_user_id = u.user_id WHERE u.user_login = @login";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@login", login);

            object result = command.ExecuteScalar();
            if (result == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }
    }

    private int GetGroupIdByStudentId(MySqlConnection connection, int studentId)
    {
        string query = "SELECT group_id FROM students_groups WHERE student_id = @studentId";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@studentId", studentId);

            object result = command.ExecuteScalar();
            if (result == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }
    }


    private List<string> GetLessonsInfoByGroupId(MySqlConnection connection, int groupId)
    {
        List<string> lessonsInfo = new List<string>();

        string query = "SELECT l.lesson_start, l.lesson_end, g.group_name, d.day_name, r.room_name FROM lesson l JOIN groups g ON l.lesson_group_id = g.group_id JOIN day d ON l.lesson_day_id = d.day_id JOIN room r ON l.lesson_room_id = r.room_id WHERE l.lesson_group_id = @groupId ORDER BY d.day_id";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@groupId", groupId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string lessonInfo = $"Группа: {reader["group_name"]},\n" +
                                        $"{reader["day_name"]}, " + $" {reader["lesson_start"]} - {reader["lesson_end"]}, \n" +
                                        $"Кабинет: {reader["room_name"]}";

                    lessonsInfo.Add(lessonInfo);
                }
            }
        }

        return lessonsInfo;
    }
    private int GetTeacherIdByLogin(MySqlConnection connection, string login)
    {
        string query = "SELECT teacher_id FROM teacher t JOIN users u ON t.teacher_user_id = u.user_id WHERE u.user_login = @login";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@login", login);

            object result = command.ExecuteScalar();
            if (result == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(result);
            }
        }
    }
    int GetStudentIdByUserId(MySqlConnection connection, int userId)
    {
        int studentId = 0;
        string query = "SELECT student_id FROM student WHERE student_user_id = @userId";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@userId", userId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    studentId = reader.GetInt32("student_id");
                }
            }
        }

        return studentId;
    }
    private List<string> GetLessonsInfoByTeacherId(MySqlConnection connection, int teacherId)
    {
        List<string> lessonsInfo = new List<string>();

        string query = "SELECT l.lesson_start, l.lesson_end, g.group_name, d.day_name, r.room_name FROM lesson l JOIN groups g ON l.lesson_group_id = g.group_id JOIN day d ON l.lesson_day_id = d.day_id JOIN room r ON l.lesson_room_id = r.room_id WHERE g.group_teacher_id = @teacherId";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@teacherId", teacherId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string lessonInfo = $"Группа: {reader["group_name"]},\n" +
                        $"{reader["day_name"]}, " + $" {reader["lesson_start"]} - {reader["lesson_end"]}, \n" +
                        $"Кабинет: {reader["room_name"]}";
                        lessonsInfo.Add(lessonInfo);
                }
            }
        }

        return lessonsInfo;
    }
}