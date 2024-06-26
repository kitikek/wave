using Microsoft.Maui.Controls.PlatformConfiguration;
using MySqlConnector;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave
{
    public partial class AttendanceTeacher : ContentPage
    {
        //�������� ������ ����� ������ �������, ������� ��� �����
        string groupName = "";

        //������������� ��������� ������ ��� ����������� ���������� ��������� � ������� ��������
        //����� ��������� � �� ���� ������ (����� ������ ������ ���� �������� � ��)
        List<int> ScheduleIds = new List<int>();
        List<int> LessonIds = new List<int>();
        List<int> StudentIds = new List<int>();

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    outerGrid.Loaded += OuterGrid_Loaded;
        //    this.SizeChanged += Form_SizeChanged;
        //}

        //private void Form_SizeChanged(object sender, EventArgs e)
        //{
        //}

        //private void OuterGrid_Loaded(object sender, EventArgs e)
        //{
        //}

        public AttendanceTeacher()
        {
            InitializeComponent();

            //���������� ������ values
            ValuePicker.ItemsSource = new List<string>{ "+", "-", "?"};
            ValuePicker.SelectedIndex = 0;

            //���������� ������ �����
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                var items = new List<string>();
                using (var cmd = new MySqlCommand("SELECT group_name FROM groups JOIN teacher ON group_teacher_id=teacher_id JOIN users ON user_id=teacher_user_id AND user_login=@log AND user_password=@pas ORDER BY group_name ASC;", con))
                {
                    cmd.Parameters.AddWithValue("@log", Authorization.Login);
                    cmd.Parameters.AddWithValue("@pas", Authorization.HashedPassword);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(reader.GetString(0));
                        }
                    }
                    GroupPicker.ItemsSource = items;
                }
            }

            //��� ��������� ������ ���������� ������� �������� ���� ����������� ������� � ����� ���������� � ������������
            GroupPicker.SelectedIndexChanged += BigLoad;

            void BigLoad(object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(GroupPicker.SelectedItem?.ToString()))
                {
                    //�������� ������ ������� ������������ ��� ������ �����
                    StudentsGrid.Children.Clear();
                    StudentsGrid.RowDefinitions.Clear();
                    StudentsGrid.ColumnDefinitions.Clear();
                    AttendanceGrid.Children.Clear();
                    AttendanceGrid.RowDefinitions.Clear();
                    AttendanceGrid.ColumnDefinitions.Clear();

                    //����� ������
                    groupName = GroupPicker.SelectedItem.ToString();

                    //���������� ����������� �������
                    ScheduleIds = FillPickerAndGetIds(ref SchedulePicker, "SELECT CONCAT(LEFT(day.day_name, 3), \" \", lesson.lesson_start, \"-\", lesson.lesson_end, \" (\", room.room_name, \")\"), lesson.lesson_id FROM groups JOIN lesson ON groups.group_id=lesson.lesson_group_id AND groups.group_name=@GroupName JOIN day ON day.day_id=lesson.lesson_day_id JOIN room ON room.room_id=lesson.lesson_room_id;");
                    LessonIds = FillPickerAndGetIds(ref LessonPicker, "SELECT CONCAT(DATE_FORMAT(lesson_exact.lesson_exact_data, \"%d.%m.%Y \"), LEFT(day.day_name, 3), \" \", lesson.lesson_start, \"-\", lesson.lesson_end, \" (\", room.room_name, \")\"), lesson_exact.lesson_exact_id FROM groups JOIN lesson ON groups.group_id=lesson.lesson_group_id AND groups.group_name=@GroupName JOIN day ON day.day_id=lesson.lesson_day_id JOIN room ON room.room_id=lesson.lesson_room_id JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id ORDER BY lesson_exact.lesson_exact_data ASC;");
                    StudentIds = FillPickerAndGetIds(ref StudentPicker, "SELECT CONCAT(users.user_surname, ' ', users.user_name), student.student_id FROM students_groups JOIN groups ON students_groups.group_id=groups.group_id AND groups.group_name=@GroupName JOIN student ON student.student_id=students_groups.student_id JOIN users ON users.user_id=student.student_user_id GROUP BY users.user_surname ASC;");
                    LessonPicker1.ItemsSource = LessonPicker.ItemsSource;

                    //�������� ������� ������������
                    Load(groupName);
                }
            };


            //��� ������� ������ Add ����������� ����� ������� � ������������ 
            AddButton.Clicked += (sender, e) =>
            {
                //�������� ������������� ������ �������
                if (SchedulePicker.SelectedItem!=null && !string.IsNullOrEmpty(groupName))
                {
                    long newId = 0;
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();
                        
                        //�������� ������� ����� � ���������� ���������� ����� �������� ���������� � ������ �������� ������ �� �����
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM lesson_exact WHERE lesson_exact_data=STR_TO_DATE(@D, \"%d.%m.%Y\") AND lesson_exact_lesson_id=@Lid;", con))
                        {
                            var Lid = ScheduleIds[SchedulePicker.SelectedIndex];
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@Lid", Lid);
                            cmd.Parameters.AddWithValue("@D", D);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //���������� ����������� ����� � �������
                        using (var cmd = new MySqlCommand("INSERT INTO `lesson_exact` (`lesson_exact_id`, `lesson_exact_data`, `lesson_exact_lesson_id`) VALUES (NULL, STR_TO_DATE(@D, \"%d.%m.%Y\"), @Lid);", con))
                        {
                            var Lid = ScheduleIds[SchedulePicker.SelectedIndex];
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@Lid", Lid);
                            cmd.Parameters.AddWithValue("@D", D);
                            cmd.ExecuteNonQuery();
                            newId = cmd.LastInsertedId;
                        }
                    }
                    //���������� ���������� �� ��������
                    BigLoad(sender, e);

                    //��������� �������� ������� ��� ���������� ��������
                    LessonPicker1.SelectedIndex = LessonIds.IndexOf((int)newId);
                    StudentPicker.SelectedIndex = 0;
                }
            };


            //��� ������� ������ Delete ��������� ��������� ���������� ����
            DeleteButton.Clicked += (sender, e) =>
            {
                //�������� ������������� ������ �������
                if (LessonPicker.SelectedItem != null && !string.IsNullOrEmpty(groupName))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM lesson_exact WHERE lesson_exact.lesson_exact_id = @Leid", con))
                        {
                            var Leid = LessonIds[LessonPicker.SelectedIndex];
                            cmd.Parameters.AddWithValue("@Leid", Leid);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //���������� ���������� �� ��������
                    BigLoad(sender, e);
                }
            };


            // ���������� ������� ��� ������ Update
            UpdateButton.Clicked += (sender, e) =>
            {
                //�������� ������������� ������ �������
                if (StudentPicker.SelectedItem != null && LessonPicker1.SelectedItem != null && !string.IsNullOrEmpty(groupName))
                {
                    //���������� ��������� �������� ��� ������������� ������������ ����������
                    int stIdx = StudentPicker.SelectedIndex;
                    int lIdx = LessonPicker1.SelectedIndex;

                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //������� ������ � ��������������� ���������, ���� ��� ����, ����� �������� ������������
                        //����� ��������� ������ � ��
                        using (var cmd = new MySqlCommand("DELETE FROM lesson_visit WHERE lesson_visit_student_id=@Stid AND lesson_visit_lesson_exact_id=@Leid; INSERT INTO lesson_visit(lesson_visit_id, lesson_visit_student_id, lesson_visit_lesson_exact_id, lesson_visit) VALUES (NULL, @Stid, @Leid, @Val);", con))
                        {
                            var Stid = StudentIds[StudentPicker.SelectedIndex];
                            var Leid = LessonIds[LessonPicker1.SelectedIndex];
                            cmd.Parameters.AddWithValue("@Leid", Leid);
                            cmd.Parameters.AddWithValue("@Stid", Stid);
                            cmd.Parameters.AddWithValue("@Val", ValuePicker.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //���������� ���������� �� ��������
                    BigLoad(sender, e);

                    //��������� �������� ������ ������� � ������� � ���������� �������
                    if (stIdx < StudentIds.Count - 1)
                    {
                        LessonPicker1.SelectedIndex = lIdx;
                        StudentPicker.SelectedIndex = stIdx + 1;
                    }
                };
            };
        }


        //���������� ������������ ������ � ������ ����������
        List<int> FillPickerAndGetIds(ref Picker p, string sql)
        {
            List <int> Ids = new List<int>();
            p.ItemsSource = FillList(sql, ref Ids);
            return Ids;
        }

        List<string> FillList(string sql, ref List<int> Ids)
        {
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                var items = new List<string>();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(reader.GetString(0));
                            Ids.Add(reader.GetInt32(1));
                        }
                    }
                    return items;
                }
            }
        }

        
        //�������� ������� ������������
        void Load(string groupName)
        {
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                //�������
                string attendanceQuery = "SELECT lesson_visit, lesson_visit_student_id, lesson_exact.lesson_exact_id FROM groups JOIN lesson ON lesson.lesson_group_id=groups.group_id AND groups.group_name=@GroupName JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id JOIN lesson_visit ON lesson_visit.lesson_visit_lesson_exact_id=lesson_exact.lesson_exact_id;"; // ORDER BY users.user_surname ASC, lesson_exact.lesson_exact_data ASC

                // ��������� ���������� ����� � ��������
                int rowCount = StudentIds.Count, columnCount = LessonIds.Count;

                //���������� ������ ������
                Label studentLabel1 = new Label
                {
                    Text = @"�������\����",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetRow(studentLabel1, 0);
                Grid.SetColumn(studentLabel1, 0);
                StudentsGrid.Children.Add(studentLabel1);

                //���������� ���������
                for (int i = 0; i < rowCount; i++)
                {
                    Label studentLabel = new Label
                    {
                        Text = StudentPicker.Items[i], //studentNames[i],
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(studentLabel, i + 1);
                    Grid.SetColumn(studentLabel, 0);
                    StudentsGrid.Children.Add(studentLabel);
                }

                //���������� ���
                for (int j = 0; j < columnCount; j++)
                {
                    Label dateLabel = new Label
                    {
                        Text = LessonPicker.Items[j].Substring(0, 10), //dateNames[j],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        //LineBreakMode = LineBreakMode.NoWrap,
                        //MinimumWidthRequest = 60,
                        //MinimumHeightRequest = 60,
                        //Rotation = 270
                    };
                    Grid.SetRow(dateLabel, 0);
                    Grid.SetColumn(dateLabel, j);
                    AttendanceGrid.Children.Add(dateLabel);
                }

                //�c������� ������� ��������
                StudentsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                for (int j = 0; j < columnCount + 1; j++)
                {
                    AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }
                for (int i = 0; i < rowCount + 1; i++)
                {
                    AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    StudentsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                //��������� � ���������� ������������
                using (var cmd = new MySqlCommand(attendanceQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Label attendanceLabel = new Label
                            {
                                Text = reader.GetString(0).ToString(),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            };
                            Grid.SetRow(attendanceLabel, StudentIds.IndexOf(reader.GetInt32(1)) + 1);
                            Grid.SetColumn(attendanceLabel, LessonIds.IndexOf(reader.GetInt32(2)));
                            AttendanceGrid.Children.Add(attendanceLabel);
                        }
                    }
                }
            }
        }
    }
}