using Microsoft.Maui.Controls;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

namespace wave
{
    public partial class UsersDirector : ContentPage
    {
        //название группы нужно многим методам, поэтому оно здесь
        string groupName = "";

        //инициализация элементов нужных для определения айдишников выбранных в списках значений
        //чтобы добавлять в БД было удобно (сразу знаешь нужный айди элемента в БД)
        List<int> SizeGroupIds = new List<int>();
        List<int> SchemeIds = new List<int>();
        List<int> TeacherIds = new List<int>();
        List<int> CourseIds = new List<int>();
        List<int> GroupIds = new List<int>();
        List<int> TypeIds = new List<int> { 1,2,3,4 };
        List<int> ParentIds = new List<int>();
        List<int> UserIds = new List<int>();
        List<int> StudentIds = new List<int>();
        List<int> StudentIds1 = new List<int>();
        List<int> StudentIdsAll = new List<int>();
        List<int> ContractIds = new List<int>();


        public UsersDirector()
        {
            InitializeComponent();

            //заполнение списка values
            pType.ItemsSource = new List<string> { "Руководитель", "Учитель", "Ученик", "Родитель" };

            //при изменении кого-либо начинается большая загрузка всех всплывающих списков
            //обновление информации на странице
            BigLoad();

            void BigLoad()
            {
                //заполнение всплывающих списков
                SizeGroupIds = FillPickerAndGetIds(ref pSize, "SELECT size_value, size_id FROM size ORDER BY size_value;");
                pSize1.ItemsSource = pSize.ItemsSource;
                SchemeIds = FillPickerAndGetIds(ref pScheme, "SELECT scheme_value, scheme_id FROM scheme ORDER BY scheme_value;");
                pScheme1.ItemsSource = pScheme.ItemsSource;
                TeacherIds = FillPickerAndGetIds(ref pTeacher, "SELECT CONCAT(user_surname, \" \", user_name, \" \", user_patronymic), teacher_id FROM teacher JOIN users ON teacher_user_id=user_id ORDER BY user_surname;");
                CourseIds = FillPickerAndGetIds(ref pCourse, "SELECT CONCAT(course_name, \" (\", size_value, \" чел) \", scheme_value), course_id FROM course JOIN size ON course_size_id=size_id JOIN scheme ON scheme_id=course_scheme_id ORDER BY course_name;");
                pCourse1.ItemsSource = pCourse.ItemsSource;
                GroupIds = FillPickerAndGetIds(ref pGroup, "SELECT CONCAT(group_name, ' (', user_surname, ' ', user_name, ', ', course_name, ' (', size_value, ' чел) ', scheme_value, ')'), group_id FROM groups JOIN course ON group_course_id=course_id JOIN size ON course_size_id=size_id JOIN scheme ON course_scheme_id=scheme_id JOIN teacher ON teacher_id=group_teacher_id JOIN users ON teacher_user_id=user_id ORDER BY group_name;");
                ParentIds = FillPickerAndGetIds(ref pParent, "SELECT CONCAT(user_surname, \" \", user_name, \" \", user_patronymic), parent_id FROM parent JOIN users ON parent_user_id=user_id;");
                UserIds = FillPickerAndGetIds(ref pUser, "SELECT CONCAT(usertype_name, ': ', users.user_surname, ' ', users.user_name, ' ', users.user_patronymic, ' (логин: ', user_login, ', телефон: ', user_phone, ')'), user_id FROM users JOIN usertype ON usertype_id=user_type_id ORDER BY usertype_id, user_surname;");
                FillPicker(ref pGroup1, "SELECT group_name FROM groups ORDER BY group_name;");
                pGroup2.ItemsSource = pGroup1.ItemsSource;
                pStudent.ItemsSource = new List<string>();
                pStudent1.ItemsSource = new List<string>();
                StudentIdsAll = FillPickerAndGetIds(ref pStudent2, "SELECT CONCAT(user_surname, ' ', user_name, ' ', user_patronymic), student.student_id FROM student JOIN users ON student_user_id=user_id ORDER BY user_surname, user_name;");
                pCourse2.ItemsSource = pCourse1.ItemsSource;
                pStudent3.ItemsSource = pStudent2.ItemsSource;
                pContract.ItemsSource = new List<string>();

            };

            pGroup1.SelectedIndexChanged += (sender, e) =>
            {
                if (pGroup1.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        var items = new List<string>();
                        var ids = new List<int>();
                        using (var cmd = new MySqlCommand("SELECT CONCAT(user_surname, ' ', user_name, ' ', user_patronymic), student.student_id FROM student JOIN users ON student_user_id=user_id WHERE student.student_id NOT IN (SELECT students_groups.student_id FROM students_groups WHERE students_groups.group_id=@GroupId) ORDER BY user_surname, user_name;", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", GroupIds[pGroup1.SelectedIndex]);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    items.Add(reader.GetString(0));
                                    ids.Add(reader.GetInt32(1));
                                }
                            }
                        }
                        pStudent.ItemsSource = items;
                        StudentIds = ids;
                    }
                }
            };

            pGroup2.SelectedIndexChanged += (sender, e) =>
            {
                if (pGroup2.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        var items = new List<string>();
                        var ids = new List<int>();
                        using (var cmd = new MySqlCommand("SELECT CONCAT(user_surname, ' ', user_name, ' ', user_patronymic), student.student_id FROM student JOIN users ON student_user_id = user_id JOIN students_groups ON student.student_id = students_groups.student_id AND students_groups.group_id=@GroupId ORDER BY user_surname, user_name;", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupId", GroupIds[pGroup2.SelectedIndex]);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    items.Add(reader.GetString(0));
                                    ids.Add(reader.GetInt32(1));
                                }
                            }
                        }
                        pStudent1.ItemsSource = items;
                        StudentIds1 = ids;
                    }
                }
            };

            pStudent3.SelectedIndexChanged += (sender, e) =>
            {
                if (pStudent3.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        var items = new List<string>();
                        var ids = new List<int>();
                        using (var cmd = new MySqlCommand("SELECT CONCAT('(', course_name, ' (', size_value, ' чел) ', scheme_value, ', Скидка: ', contract_sale, 'руб.)'), contract_id FROM contract JOIN course ON contract_course_id=course_id AND contract_student_id=@Sid JOIN size ON size_id=course_size_id JOIN scheme ON scheme_id=course_scheme_id;", con))
                        {
                            cmd.Parameters.AddWithValue("@Sid", StudentIdsAll[pStudent3.SelectedIndex]);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    items.Add(reader.GetString(0));
                                    ids.Add(reader.GetInt32(1));
                                }
                            }
                        }
                        pContract.ItemsSource = items;
                        ContractIds = ids;
                    }
                }
            };

            //добавление нового размера группы
            btnAddGroupSize.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (!string.IsNullOrEmpty(txtSize.Text))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM size WHERE size_value=@Size;", con))
                        {
                            cmd.Parameters.AddWithValue("@Size", txtSize.Text);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `size`(`size_id`, `size_value`) VALUES (NULL, @Size);", con))
                        {
                            cmd.Parameters.AddWithValue("@Size", txtSize.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                    txtSize.Text = "";
                }
            };

            //добавление новой схемы оплаты
            btnAddScheme.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (!string.IsNullOrEmpty(txtScheme.Text))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM scheme WHERE scheme_value=@Scheme;", con))
                        {
                            cmd.Parameters.AddWithValue("@Scheme", txtScheme.Text);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `scheme`(`scheme_id`, `scheme_value`) VALUES (NULL, @Scheme);", con))
                        {
                            cmd.Parameters.AddWithValue("@Scheme", txtScheme.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                    txtScheme.Text = "";
                }
            };

            //удаление размера группы
            btnDelGroupSize.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pSize.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `size` WHERE size_id=@Size;", con))
                        {
                            cmd.Parameters.AddWithValue("@Size", SizeGroupIds[pSize.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //удаление схемы оплаты
            btnDelScheme.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pScheme.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `scheme` WHERE scheme_id=@Scheme;", con))
                        {
                            cmd.Parameters.AddWithValue("@Scheme", SchemeIds[pScheme.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //добавление нового курса
            btnAddCourse.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (!string.IsNullOrEmpty(txtCourse.Text) && pSize1.SelectedItem != null && !string.IsNullOrEmpty(txtHours.Text) && !string.IsNullOrEmpty(txtHoursCost.Text) && pScheme1.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM course WHERE course_name=@CourseName AND course_size_id=@SizeId AND course_time=@Time AND course_price=@Price AND course_scheme_id=@SchemeId;", con))
                        {
                            cmd.Parameters.AddWithValue("@CourseName", txtCourse.Text);
                            cmd.Parameters.AddWithValue("@SizeId", SizeGroupIds[pSize1.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Time", txtHours.Text);
                            cmd.Parameters.AddWithValue("@Price", txtHoursCost.Text);
                            cmd.Parameters.AddWithValue("@SchemeId", SchemeIds[pScheme1.SelectedIndex]);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `course`(`course_id`, `course_name`, `course_size_id`, `course_time`, `course_price`, `course_scheme_id`) VALUES (NULL, @CourseName, @SizeId, @Time, @Price, @SchemeId);", con))
                        {
                            cmd.Parameters.AddWithValue("@CourseName", txtCourse.Text);
                            cmd.Parameters.AddWithValue("@SizeId", SizeGroupIds[pSize1.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Time", txtHours.Text);
                            cmd.Parameters.AddWithValue("@Price", txtHoursCost.Text);
                            cmd.Parameters.AddWithValue("@SchemeId", SchemeIds[pScheme1.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                    txtCourse.Text = "";
                    txtHours.Text = "";
                    txtHoursCost.Text = "";
                }
            };

            //добавление новой группы
            btnAddGroup.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (!string.IsNullOrEmpty(txtGroup.Text) && pTeacher.SelectedItem!=null && pCourse.SelectedItem!=null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM groups WHERE group_name=@GroupName AND group_teacher_id=@Tid AND group_course_id=@Cid;", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupName", txtGroup.Text);
                            cmd.Parameters.AddWithValue("@Tid", TeacherIds[pTeacher.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Cid", CourseIds[pCourse.SelectedIndex]);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `groups`(`group_id`, `group_name`, `group_teacher_id`, `group_course_id`) VALUES (NULL, @GroupName, @Tid, @Cid);", con))
                        {
                            cmd.Parameters.AddWithValue("@GroupName", txtGroup.Text);
                            cmd.Parameters.AddWithValue("@Tid", TeacherIds[pTeacher.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Cid", CourseIds[pCourse.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                    txtGroup.Text = "";
                }
            };

            //удаление курса
            btnDelCourse.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pCourse1.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `course` WHERE course_id=@Cid;", con))
                        {
                            cmd.Parameters.AddWithValue("@Cid", CourseIds[pCourse1.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //удаление группы
            btnDelGroup.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pGroup.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `groups` WHERE group_id=@Gid;", con))
                        {
                            cmd.Parameters.AddWithValue("@Gid", GroupIds[pGroup.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //добавление нового пользователя
            btnAddUser.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                long Uid=0;
                if (!string.IsNullOrEmpty(txtSurname.Text) && !string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtFatherName.Text) && !string.IsNullOrEmpty(txtPhoneNumber.Text) && !string.IsNullOrEmpty(txtLogin.Text) && !string.IsNullOrEmpty(txtPassword.Text) && pType.SelectedItem != null
                && !(pType.SelectedIndex==2 && pParent.SelectedItem==null))
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        string hashedPassword;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] passwordBytes = Encoding.UTF8.GetBytes(txtPassword.Text);
                            byte[] passwordHashBytes = sha256.ComputeHash(passwordBytes);
                            hashedPassword = BitConverter.ToString(passwordHashBytes).Replace("-", "").ToLower();
                        }

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE user_surname=@Sur AND user_name=@Nam AND user_patronymic=@Fat AND user_phone=@Phone AND user_login=@Log AND user_password=@Pas AND user_type_id=@TypeId;", con))
                        {
                            cmd.Parameters.AddWithValue("@Sur", txtSurname.Text);
                            cmd.Parameters.AddWithValue("@Nam", txtName.Text);
                            cmd.Parameters.AddWithValue("@Fat", txtFatherName.Text);
                            cmd.Parameters.AddWithValue("@Phone", txtPhoneNumber.Text);
                            cmd.Parameters.AddWithValue("@Log", txtLogin.Text);
                            cmd.Parameters.AddWithValue("@Pas", hashedPassword);
                            cmd.Parameters.AddWithValue("@TypeId", TypeIds[pType.SelectedIndex]);
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `users`(`user_id`, `user_surname`, `user_name`, `user_patronymic`, `user_phone`, `user_login`, `user_password`, `user_type_id`) VALUES (NULL, @Sur, @Nam, @Fat, @Phone, @Log, @Pas, @TypeId);", con))
                        {
                            cmd.Parameters.AddWithValue("@Sur", txtSurname.Text);
                            cmd.Parameters.AddWithValue("@Nam", txtName.Text);
                            cmd.Parameters.AddWithValue("@Fat", txtFatherName.Text);
                            cmd.Parameters.AddWithValue("@Phone", txtPhoneNumber.Text);
                            cmd.Parameters.AddWithValue("@Log", txtLogin.Text);
                            cmd.Parameters.AddWithValue("@Pas", hashedPassword);
                            cmd.Parameters.AddWithValue("@TypeId", TypeIds[pType.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                            Uid = cmd.LastInsertedId;
                        }

                        //добавление конкретного типа пользователя
                        //учитель
                        if (pType.SelectedIndex == 1)
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO `teacher`(`teacher_id`, `teacher_user_id`) VALUES (NULL, @Uid);", con))
                            {
                                cmd.Parameters.AddWithValue("@Uid", Uid);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //ученик
                        if (pType.SelectedIndex == 2)
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO `student`(`student_id`, `student_parent_id`, `student_dirthday`, `student_user_id`) VALUES (NULL, @Pid, STR_TO_DATE(@Dat, \"%d.%m.%Y\"), @Uid);", con))
                            {
                                cmd.Parameters.AddWithValue("@Uid", Uid);
                                cmd.Parameters.AddWithValue("@Pid", ParentIds[pParent.SelectedIndex]);
                                cmd.Parameters.AddWithValue("@Dat", Dat.Date.ToShortDateString());
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //родитель
                        if (pType.SelectedIndex == 3)
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO `parent`(`parent_id`, `parent_user_id`) VALUES (NULL, @Uid);", con))
                            {
                                cmd.Parameters.AddWithValue("@Uid", Uid);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    //обновление информации на странице
                    BigLoad();
                    txtSurname.Text = "";
                    txtName.Text = "";
                    txtSurname.Text = "";
                    txtFatherName.Text = "";
                    txtPhoneNumber.Text = "";
                    txtLogin.Text = "";
                    txtPassword.Text = "";
                }
            };

            //удаление пользователя
            btnDelUser.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pUser.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `users` WHERE user_id=@Uid;", con))
                        {
                            cmd.Parameters.AddWithValue("@Uid", UserIds[pUser.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //добавление студента в группу
            btnAddStudGr.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pGroup1.SelectedItem != null && pStudent.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `students_groups`(`student_id`, `group_id`) VALUES (@Sid, @Gid);", con))
                        {
                            cmd.Parameters.AddWithValue("@Gid", GroupIds[pGroup1.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Sid", StudentIds[pStudent.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //удаление студента из группы
            btnDelStudGr.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pGroup2.SelectedItem != null && pStudent1.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `students_groups` WHERE student_id=@Sid AND group_id=@Gid;", con))
                        {
                            cmd.Parameters.AddWithValue("@Gid", GroupIds[pGroup2.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Sid", StudentIds1[pStudent1.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //добавление нового договора на студента
            btnAddContract.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (!string.IsNullOrEmpty(txtDiscount.Text) && pStudent2.SelectedItem!=null && pCourse2.SelectedItem!=null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        //проверка наличия элемента с выбранным значением чтобы отменить добавление в случае создания такого же элемента
                        using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM contract WHERE contract_student_id=@Sid AND contract_course_id=@Cid AND contract_sale=@Dis;", con))
                        {
                            cmd.Parameters.AddWithValue("@Sid", StudentIdsAll[pStudent2.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Cid", CourseIds[pCourse2.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Dis", Convert.ToInt32(txtDiscount.Text));
                            if (Convert.ToInt64(cmd.ExecuteScalar()) > 0)
                                return;
                        }

                        //добавление элемента в таблицу
                        using (var cmd = new MySqlCommand("INSERT INTO `contract`(`contract_id`, `contract_student_id`, `contract_course_id`, `contract_sale`) VALUES (NULL, @Sid, @Cid, @Dis);", con))
                        {
                            cmd.Parameters.AddWithValue("@Sid", StudentIdsAll[pStudent2.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Cid", CourseIds[pCourse2.SelectedIndex]);
                            cmd.Parameters.AddWithValue("@Dis", Convert.ToInt32(txtDiscount.Text));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                    txtDiscount.Text = "";
                }
            };

            //удаление договора на студента
            btnDelContract.Clicked += (sender, e) =>
            {
                //проверка заполненности нужных списков
                if (pContract.SelectedItem != null)
                {
                    using (var con = new MySqlConnection(ConnString.connString))
                    {
                        con.Open();

                        using (var cmd = new MySqlCommand("DELETE FROM `contract` WHERE contract_id=@Conid;", con))
                        {
                            cmd.Parameters.AddWithValue("@Conid", ContractIds[pContract.SelectedIndex]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //обновление информации на странице
                    BigLoad();
                }
            };

            //установка событий проверки числовых значений
            txtDiscount.TextChanged += OnEntryTextChanged;
            txtHours.TextChanged += OnEntryTextChanged;
            txtHoursCost.TextChanged += OnEntryTextChanged;
            //проверка на ввод числовых значений
            void OnEntryTextChanged(object sender, TextChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    if (!int.TryParse(e.NewTextValue, out _) || e.NewTextValue.Length > 6)
                    {
                        ((Entry)sender).Text = e.OldTextValue;
                    }
                }
            }
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
    }
}
