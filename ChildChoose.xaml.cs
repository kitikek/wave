using MySqlConnector;



namespace wave;

public partial class ChildChoose : ContentPage
{
	public static int childCount;

    List<Child> childrenList = new List<Child>();
    List<int> idChild = new List<int>();
    public static int chosenId;
    public ChildChoose()
    {
        InitializeComponent();

        var cs = ConnString.connString;

        var userLogin = Authorization.Login;

        using (var con = new MySqlConnection(cs))
        {
            con.Open();

            int id = 0;
            var name = "";
            long childCount = 0;

            var cmd = new MySqlCommand(@"SELECT p.parent_id FROM parent p JOIN users u ON p.parent_user_id = u.user_id WHERE u.user_login = @Login;", con);
            cmd.Parameters.AddWithValue("@Login", userLogin);

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    id = (int)dr["parent_id"];
                }
            }
            cmd = new MySqlCommand(@"SELECT COUNT(*) AS children_count FROM student WHERE student_parent_id = @id;", con);
            cmd.Parameters.AddWithValue("@id", id);
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    childCount = (long)dr["children_count"];
                }
            }
            cmd = new MySqlCommand(@"SELECT student_user_id FROM student WHERE student_parent_id = @id;", con);
            cmd.Parameters.AddWithValue("@id", id);
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    id = (int)dr["student_user_id"];
                    idChild.Add(id);
                }
            }
            for (int i = 0; i < childCount; i++)
            {
                cmd = new MySqlCommand(@"SELECT user_name FROM users WHERE user_id = @id;", con);
                cmd.Parameters.AddWithValue("@id", idChild[i]);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        name = dr["user_name"].ToString();

                        childrenList.Add(new Child()
                        {
                            Id = idChild[i],
                            Name = name
                        });
                    }
                }
            }
        }
        for (int j = 0; j < childrenList.Count; j++)
        {
            int childId = childrenList[j].Id;
            Button ChildChooseButton = new Button
            {
                Text = childrenList[j].Name,
                FontSize = 30,
                Margin = 20,
                MaximumWidthRequest = 400,
                MinimumWidthRequest = 300,
                MinimumHeightRequest = 80
            };
            ChildChooseButton.Clicked += async (sender, e) =>
            {
                chosenId = childId;
                await Shell.Current.GoToAsync("//Parent//Notification");
            };
            (Content as VerticalStackLayout)?.Children.Add(ChildChooseButton);
        }
    }
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public async void YourChildren(object sender, EventArgs e)
    {
        
    }
}