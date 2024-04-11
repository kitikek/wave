using Microsoft.Maui.Controls.PlatformConfiguration;
using MySqlConnector;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace wave
{
    public partial class AttendanceTeacher : ContentPage
    {
        //название группы нужно многим методам, поэтому оно здесь
        string groupName = "";

        //инициализация элементов нужных для определения айдишников выбранных в списках значений
        //чтобы добавлять в БД было удобно (сразу знаешь нужный айди элемента в БД)
        List<int> ScheduleIds = new List<int>();
        List<int> LessonIds = new List<int>();
        List<int> StudentIds = new List<int>();
        List<int> LessonIds1 = new List<int>();

        public AttendanceTeacher()
        {
            InitializeComponent();

            //заполнение списка values
            ValuePicker.ItemsSource = new List<string>{ "+", "-", "?"};
            ValuePicker.SelectedIndex = 0;

            //заполнение списка групп
            FillPicker(ref GroupPicker, "SELECT group_name FROM groups ORDER BY group_name ASC;");


            //при изменении группы начинается большая загрузка всех всплывающих списков и вывод информации о посещаемости
            GroupPicker.SelectedIndexChanged += BigLoad;

            void BigLoad(object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(GroupPicker.SelectedItem?.ToString()))
                {
                    //очищение старой таблицы посещаемости для вывода новой
                    AttendanceGrid.Children.Clear();
                    AttendanceGrid.RowDefinitions.Clear();
                    AttendanceGrid.ColumnDefinitions.Clear();

                    //выбор группы
                    groupName = GroupPicker.SelectedItem.ToString();

                    //заполнение всплывающих списков
                    ScheduleIds = FillPickerAndGetIds(ref SchedulePicker, "SELECT CONCAT(LEFT(day.day_name, 3), \" \", lesson.lesson_start, \"-\", lesson.lesson_end, \" (\", room.room_name, \")\"), lesson.lesson_id FROM groups JOIN lesson ON groups.group_id=lesson.lesson_group_id AND groups.group_name=@GroupName JOIN day ON day.day_id=lesson.lesson_day_id JOIN room ON room.room_id=lesson.lesson_room_id;");
                    LessonIds = FillPickerAndGetIds(ref LessonPicker, "SELECT CONCAT(DATE_FORMAT(lesson_exact.lesson_exact_data, \"%d.%m.%Y \"), LEFT(day.day_name, 3), \" \", lesson.lesson_start, \"-\", lesson.lesson_end, \" (\", room.room_name, \")\"), lesson_exact.lesson_exact_id FROM groups JOIN lesson ON groups.group_id=lesson.lesson_group_id AND groups.group_name=@GroupName JOIN day ON day.day_id=lesson.lesson_day_id JOIN room ON room.room_id=lesson.lesson_room_id JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id ORDER BY lesson_exact.lesson_exact_data ASC;");
                    StudentIds = FillPickerAndGetIds(ref StudentPicker, "SELECT CONCAT(users.user_surname, ' ', users.user_name), student.student_id FROM students_groups JOIN groups ON students_groups.group_id=groups.group_id AND groups.group_name=@GroupName JOIN student ON student.student_id=students_groups.student_id JOIN users ON users.user_id=student.student_user_id GROUP BY users.user_surname ASC;");
                    LessonIds1 = FillPickerAndGetIds(ref LessonPicker1, "SELECT CONCAT(DATE_FORMAT(lesson_exact.lesson_exact_data, \"%d.%m.%Y \"), LEFT(day.day_name, 3), \" \", lesson.lesson_start, \"-\", lesson.lesson_end, \" (\", room.room_name, \")\"), lesson_exact.lesson_exact_id FROM groups JOIN lesson ON groups.group_id=lesson.lesson_group_id AND groups.group_name=@GroupName JOIN day ON day.day_id=lesson.lesson_day_id JOIN room ON room.room_id=lesson.lesson_room_id JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id ORDER BY lesson_exact.lesson_exact_data ASC;");

                    //загрузка таблицы посещаемости
                    Load(groupName);
                }
            };


            //при нажатии кнопки Add добавляется новый столбец в посещаемость 
            AddButton.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (SchedulePicker.SelectedItem!=null && !string.IsNullOrEmpty(groupName))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия урока с выбранными свойствами чтобы отменить добавление в случае создания такого же урока
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM lesson_exact WHERE lesson_exact_data=STR_TO_DATE(@D, \"%d.%m.%Y\") AND lesson_exact_lesson_id=@Lid;", con))
                        {
                            var Lid = ScheduleIds[SchedulePicker.SelectedIndex];
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@Lid", Lid);
                            cmd.Parameters.AddWithValue("@D", D);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление конкретного урока в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `lesson_exact` (`lesson_exact_id`, `lesson_exact_data`, `lesson_exact_lesson_id`) VALUES (NULL, STR_TO_DATE(@D, \"%d.%m.%Y\"), @Lid);", con))
                        {
                            var Lid = ScheduleIds[SchedulePicker.SelectedIndex];
                            var D = Dat.Date.ToShortDateString();
                            cmd.Parameters.AddWithValue("@Lid", Lid);
                            cmd.Parameters.AddWithValue("@D", D);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad(sender, e);

                    //установка значений списков для добавления учеников
                    LessonPicker1.SelectedIndex = LessonIds.Count-1;
                    StudentPicker.SelectedIndex = 0;
                }
            };


            //при нажатии кнопки Delete удаляется выбранный конкретный урок
            DeleteButton.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
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
                    //обновление информации на странице
                    BigLoad(sender, e);
                }
            };


            // Обработчик события для кнопки Update
            UpdateButton.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (StudentPicker.SelectedItem != null && LessonPicker1.SelectedItem != null && !string.IsNullOrEmpty(groupName))
                {
                    //запоминаем выбранные элементы для автоматизации последующего заполнения
                    int stIdx = StudentPicker.SelectedIndex;
                    int lIdx = LessonPicker1.SelectedIndex;

                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //удаляем данные о соответствующем посещении, если они были, чтобы избежать дублирования
                        //затем добавляем запись в БД
                        using (var cmd = new MySqlCommand("DELETE FROM lesson_visit WHERE lesson_visit_student_id=@Stid AND lesson_visit_lesson_exact_id=@Leid; INSERT INTO lesson_visit(lesson_visit_id, lesson_visit_student_id, lesson_visit_lesson_exact_id, lesson_visit) VALUES (NULL, @Stid, @Leid, @Val);", con))
                        {
                            var Stid = StudentIds[StudentPicker.SelectedIndex];
                            var Leid = LessonIds1[LessonPicker1.SelectedIndex];
                            cmd.Parameters.AddWithValue("@Leid", Leid);
                            cmd.Parameters.AddWithValue("@Stid", Stid);
                            cmd.Parameters.AddWithValue("@Val", ValuePicker.SelectedItem.ToString());
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad(sender, e);

                    //установка значений нужных списков и переход к следующему ученику
                    if (stIdx < StudentIds.Count - 1)
                    {
                        LessonPicker1.SelectedIndex = lIdx;
                        StudentPicker.SelectedIndex = stIdx + 1;
                    }
                };
            };
        }


        //заполнение всплывающего списка
        void FillPicker(ref Picker p, string sql)
        {
            p.ItemsSource = FillList(sql); 
        }

        List<string> FillList(string sql)
        {
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                var items = new List<string>();
                using (var cmd = new MySqlCommand(sql, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(reader.GetString(0));
                    }
                }
                return items;
            }
        }

        //заполнение всплывающего списка и списка айдишников
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

        
        //загрузка таблицы посещаемости
        void Load(string groupName)
        {
            using (var con = new MySqlConnection(ConnString.connString))
            {
                con.Open();

                //запросы
                string attendanceQuery = "SELECT lesson_visit, student.student_id, lesson_exact.lesson_exact_id FROM groups JOIN lesson ON lesson.lesson_group_id=groups.group_id AND groups.group_name=@GroupName JOIN lesson_exact ON lesson_exact.lesson_exact_lesson_id=lesson.lesson_id JOIN lesson_visit ON lesson_visit.lesson_visit_lesson_exact_id=lesson_exact.lesson_exact_id JOIN student ON student.student_id=lesson_visit.lesson_visit_student_id JOIN users ON users.user_id=student.student_user_id;"; // ORDER BY users.user_surname ASC, lesson_exact.lesson_exact_data ASC

                // Получение количества строк и столбцов
                int rowCount = StudentIds.Count, columnCount = LessonIds.Count;

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
                        Text = StudentPicker.Items[i], //studentNames[i],
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
                        Text = LessonPicker.Items[j].Substring(0, 10), //dateNames[j],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(dateLabel, 0);
                    Grid.SetColumn(dateLabel, j + 1);
                    AttendanceGrid.Children.Add(dateLabel);
                }

                //учтановка свойств столбцов
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                for (int j = 1; j < columnCount + 1; j++)
                {
                    AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }
                for (int i = 0; i < rowCount + 1; i++)
                {
                    AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                }

                //Получение и заполнение посещаемости
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
                            Grid.SetColumn(attendanceLabel, LessonIds.IndexOf(reader.GetInt32(2)) + 1);
                            AttendanceGrid.Children.Add(attendanceLabel);
                        }
                    }
                }
            }
        }
    }
}