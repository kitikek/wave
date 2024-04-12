using MySqlConnector;

namespace wave
{
    public partial class UsersDirector : ContentPage
    {
        //�������� ������ ����� ������ �������, ������� ��� �����
        string groupName = "";

        //������������� ��������� ������ ��� ����������� ���������� ��������� � ������� ��������
        //����� ��������� � �� ���� ������ (����� ������ ������ ���� �������� � ��)
        List<int> SizeGroupIds = new List<int>();
        List<int> SchemeIds = new List<int>();
        List<int> TeacherIds = new List<int>();
        List<int> CourseIds = new List<int>();
        List<int> GroupIds = new List<int>();
        List<int> TypeIds = new List<int> { 1,2,3,4 };
        List<int> ParentIds = new List<int>();
        List<int> UserIds = new List<int>();
        List<int> StudentIds = new List<int>();


        public UsersDirector()
        {
            InitializeComponent();

            //���������� ������ values
            pType.ItemsSource = new List<string> { "������������", "�������", "������", "��������" };

            //��� ��������� ����-���� ���������� ������� �������� ���� ����������� �������
            //���������� ���������� �� ��������
            BigLoad();

            void BigLoad()
            {
                //���������� ����������� �������
                SizeGroupIds = FillPickerAndGetIds(ref pSize, "SELECT size_value, size_id FROM size;");
                pSize1.ItemsSource = pSize.ItemsSource;
                SchemeIds = FillPickerAndGetIds(ref pScheme, "SELECT scheme_value, scheme_id FROM scheme;");
                pScheme1.ItemsSource = pScheme.ItemsSource;
                TeacherIds = FillPickerAndGetIds(ref pTeacher, "SELECT CONCAT(user_surname, \" \", user_name, \" \", user_patronymic), teacher_id FROM teacher JOIN users ON teacher_user_id=user_id;");
                CourseIds = FillPickerAndGetIds(ref pCourse, "SELECT CONCAT(course_name, \" (\", size_value, \" ���) \", scheme_value), course_id FROM course JOIN size ON course_size_id=size_id JOIN scheme ON scheme_id=course_scheme_id;");
                pCourse1.ItemsSource = pCourse.ItemsSource;
                GroupIds = FillPickerAndGetIds(ref pScheme, "SELECT CONCAT(group_name, \" - (\", course_name, \" (\", size_value, \" ���) \", scheme_value, \")\"), group_id FROM groups JOIN course ON group_course_id=course_id JOIN size ON course_size_id=size_id JOIN scheme ON course_scheme_id=scheme_id;");
                ParentIds = FillPickerAndGetIds(ref pParent, "SELECT CONCAT(user_surname, \" \", user_name, \" \", user_patronymic), parent_id FROM parent JOIN users ON parent_user_id=user_id;");

            };






            //��������� ������� �������� �������� ��������
            txtHours.TextChanged += OnEntryTextChanged;
            txtHoursCost.TextChanged += OnEntryTextChanged;

            //�������� �� ���� �������� ��������
            void OnEntryTextChanged(object sender, TextChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    if (!int.TryParse(e.NewTextValue, out _) || e.NewTextValue.Length > 4)
                    {
                        ((Entry)sender).Text = e.OldTextValue;
                    }
                }
            }

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
