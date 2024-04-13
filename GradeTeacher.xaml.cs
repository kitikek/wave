using MySqlConnector;
using System;

namespace wave
{
    public partial class GradeTeacher : ContentPage
    {
        //�������� ������ ����� ������ �������, ������� ��� �����
        string groupName = "";

        //������������� ��������� ������ ��� ����������� ���������� ��������� � ������� ��������
        //����� ��������� � �� ���� ������ (����� ������ ������ ���� �������� � ��)
        List<int> TestIds = new List<int>();
        List<int> StudentIds = new List<int>();

        public GradeTeacher()
        {
            InitializeComponent();

            //���������� ������ values
            ValuePicker.ItemsSource = new List<string> { "0", "1", "2", "3", "4", "5", "-" };
            ValuePicker.SelectedIndex = 4;

            //���������� ������ �����
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                var items = new List<string>();
                using (var cmd = new MySqlCommand("SELECT group_name FROM groups JOIN teacher ON group_teacher_id=teacher_id JOIN users ON user_id=teacher_user_id AND user_login=@log AND user_password=@pas ORDER BY group_name ASC;", con))
                {
                    cmd.Parameters.AddWithValue("@log", Authorization.Login);
                    cmd.Parameters.AddWithValue("@pas", Authorization.Password);
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
                    ResultsGrid.Children.Clear();
                    ResultsGrid.RowDefinitions.Clear();
                    ResultsGrid.ColumnDefinitions.Clear();

                    //����� ������
                    groupName = GroupPicker.SelectedItem.ToString();

                    //���������� ����������� �������
                    TestIds = FillPickerAndGetIds(ref TestPicker, "SELECT CONCAT(test_name, '\\n', DATE_FORMAT(test_date, \"%d.%m.%Y\")), test_id FROM test JOIN groups ON test_group_id=groups.group_id AND groups.group_name=@GroupName ORDER BY test_date ASC;");
                    StudentIds = FillPickerAndGetIds(ref StudentPicker, "SELECT CONCAT(users.user_surname, ' ', users.user_name), student.student_id FROM students_groups JOIN groups ON students_groups.group_id=groups.group_id AND groups.group_name=@GroupName JOIN student ON student.student_id=students_groups.student_id JOIN users ON users.user_id=student.student_user_id GROUP BY users.user_surname ASC;");
                    TestPicker1.ItemsSource = TestPicker.ItemsSource;

                    //�������� ������� ������������
                    Load(groupName);
                }
            };

            //��� ������� ������ Add ����������� ����� ������� � �����
            AddButton.Clicked += (sender, e) =>
            {
                //�������� ������������� ������ �������
                if (!string.IsNullOrEmpty(TestName.Text) && !string.IsNullOrEmpty(groupName))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //�������� ������� ����� � ���������� ���������� ����� �������� ���������� � ������ �������� ������ �� �����
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM test WHERE test.test_name=@TN AND test.test_date=STR_TO_DATE(@D, \"%d.%m.%Y\");", con))
                        {
                            var TN = TestName.Text;
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@TN", TN);
                            cmd.Parameters.AddWithValue("@D", D);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //���������� ����������� ����� � �������
                        using (var cmd = new MySqlCommand("INSERT INTO `test` (`test_id`, `test_name`, `test_date`, `test_group_id`) VALUES (NULL, @TN, STR_TO_DATE(@D, \"%d.%m.%Y\"), (SELECT groups.group_id FROM groups WHERE groups.group_name=@GroupName));", con))
                        {
                            var TN = TestName.Text;
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@TN", TN);
                            cmd.Parameters.AddWithValue("@D", D);
                            cmd.Parameters.AddWithValue("@GroupName", groupName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //���������� ���������� �� ��������
                    BigLoad(sender, e);

                    //��������� �������� ������� ��� ���������� ��������
                    TestPicker1.SelectedIndex = TestIds.Count - 1;
                    StudentPicker.SelectedIndex = 0;
                }
            };


            //��� ������� ������ Delete ��������� ��������� ���������� ����
            DeleteButton.Clicked += (sender, e) =>
            {
                //�������� ������������� ������ �������
                if (TestPicker.SelectedItem != null && !string.IsNullOrEmpty(groupName))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `test` WHERE test_id=@Tid;", con))
                        {
                            var Tid = TestIds[TestPicker.SelectedIndex];
                            cmd.Parameters.AddWithValue("@Tid", Tid);
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
                if (StudentPicker.SelectedItem != null && TestPicker1.SelectedItem != null && !string.IsNullOrEmpty(groupName))
                {
                    //���������� ��������� �������� ��� ������������� ������������ ����������
                    int stIdx = StudentPicker.SelectedIndex;
                    int tIdx = TestPicker1.SelectedIndex;

                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //������� ������ � ��������������� ������, ���� ��� ����, ����� �������� ������������
                        //����� ��������� ������ � ��
                        using (var cmd = new MySqlCommand("DELETE FROM `test_visit` WHERE test_visit_student_id=@Stid AND test_visit_test_id=@Tid; INSERT INTO `test_visit`(`test_visit_id`, `test_visit_student_id`, `test_visit_test_id`, `test_visit_mark`) VALUES (NULL, @Stid, @Tid, @Val);", con))
                        {
                            var Stid = StudentIds[StudentPicker.SelectedIndex];
                            var Tid = TestIds[TestPicker1.SelectedIndex];
                            cmd.Parameters.AddWithValue("@Tid", Tid);
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
                        TestPicker1.SelectedIndex = tIdx;
                        StudentPicker.SelectedIndex = stIdx + 1;
                    }
                };
            };
        }


        //���������� ������������ ������ � ������ ����������
        List<int> FillPickerAndGetIds(ref Picker p, string sql)
        {
            List<int> Ids = new List<int>();
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


        //�������� ������� ������
        void Load(string groupName)
        {
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                //�������
                string marksQuery = "SELECT test_visit.test_visit_mark, test_visit_student_id, test_visit_test_id FROM groups JOIN students_groups ON groups.group_name=@GroupName AND groups.group_id=students_groups.group_id JOIN test_visit ON test_visit.test_visit_student_id=students_groups.student_id;";

                // ��������� ���������� ����� � ��������
                int rowCount = StudentIds.Count, columnCount = TestIds.Count;

                //���������� ������ ������
                Label Label1 = new Label
                {
                    Text = @"�������\������",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetRow(Label1, 0);
                Grid.SetColumn(Label1, 0);
                ResultsGrid.Children.Add(Label1);

                //���������� ���������
                for (int i = 0; i < rowCount; i++)
                {
                    Label studentLabel = new Label
                    {
                        Text = StudentPicker.Items[i],
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(studentLabel, i + 1);
                    Grid.SetColumn(studentLabel, 0);
                    ResultsGrid.Children.Add(studentLabel);
                }

                //���������� �����
                for (int j = 0; j < columnCount; j++)
                {
                    Label testLabel = new Label
                    {
                        Text = TestPicker.Items[j],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    Grid.SetRow(testLabel, 0);
                    Grid.SetColumn(testLabel, j + 1);
                    ResultsGrid.Children.Add(testLabel);
                }

                //��������� � ���������� ������
                using (var cmd = new MySqlCommand(marksQuery, con))
                {
                    cmd.Parameters.AddWithValue("@GroupName", groupName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Label marksLabel = new Label
                            {
                                Text = reader.GetString(0).ToString(),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            };
                            Grid.SetRow(marksLabel, StudentIds.IndexOf(reader.GetInt32(1)) + 1);
                            Grid.SetColumn(marksLabel, TestIds.IndexOf(reader.GetInt32(2)) + 1);
                            ResultsGrid.Children.Add(marksLabel);
                        }
                    }
                }

                //�c������� ������� ��������
                for (int j = 0; j < columnCount + 1; j++)
                {
                    ResultsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }
                for (int i = 0; i < rowCount + 1; i++)
                {
                    ResultsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }
            }
        }
    }
}
