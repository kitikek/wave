using MySqlConnector;
using System.Data;

namespace wave;

public partial class Payment : ContentPage
{
    private int fullPrice;
    private StackLayout mainStackLayout;

    public Payment()
    {
        InitializeComponent();
        mainStackLayout = new StackLayout();
        Content = new ScrollView { Content = mainStackLayout };
        LoadCourseInfo();
    }

    private void LoadCourseInfo()
    {
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string parentLogin = Authorization.Login;
            int totalPrice = 0;
            int singlePayment = 0;

            string query = @"SELECT c.course_name, c.course_price, c.course_time, s.scheme_value, con.contract_sale
                        FROM course c
                        JOIN contract con ON c.course_id = con.contract_course_id
                        JOIN scheme s ON s.scheme_id = c.course_scheme_id
                        JOIN student st ON st.student_id = con.contract_student_id
                        JOIN parent p ON p.parent_id = st.student_parent_id
                        JOIN users u ON u.user_id = p.parent_user_id
                        WHERE u.user_login = @parentLogin";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@parentLogin", parentLogin);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string courseName = reader["course_name"].ToString();
                        int coursePrice = Convert.ToInt32(reader["course_price"]);
                        int courseTime = Convert.ToInt32(reader["course_time"]);
                        int schemeValue = Convert.ToInt32(reader["scheme_value"]);
                        int discount = Convert.ToInt32(reader["contract_sale"]);

                        if (discount > 0)
                        {
                            totalPrice = coursePrice * courseTime - discount;
                            fullPrice = totalPrice;
                            singlePayment = totalPrice / schemeValue;

                            AddCourseInfoToStackLayout(courseName, coursePrice, totalPrice, singlePayment);
                        }
                        else
                        {
                            totalPrice = coursePrice * courseTime;
                            fullPrice = totalPrice;
                            singlePayment = totalPrice / schemeValue;

                            AddCourseInfoToStackLayout(courseName, coursePrice, totalPrice, singlePayment);
                        }
                    }
                    else
                    {
                        Label noPaymentInfoLabel = new Label
                        {
                            Text = "Информация об оплатах отсутствует",
                            Margin = new Thickness(10, 5, 10, 5)
                        };

                        mainStackLayout.Children.Add(noPaymentInfoLabel);
                    }
                }
            }

            LoadPaymentInfo();
        }
    }

    private void LoadPaymentInfo()
    {
        string parentLogin = Authorization.Login;
        string connectionString = ConnString.connString;
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();

            string queryPayments = @"SELECT p.payment_sum, p.payment_details, p.payment_date
                                FROM payment p
                                    INNER JOIN contract c ON p.payment_contract_id = c.contract_id
                                    INNER JOIN student s ON c.contract_student_id = s.student_id
                                    INNER JOIN parent pt ON s.student_parent_id = pt.parent_id
                                    INNER JOIN users u ON pt.parent_user_id = u.user_id
                                    WHERE u.user_login = @Login;";

            using (MySqlCommand commandPayments = new MySqlCommand(queryPayments, con))
            {
                commandPayments.Parameters.AddWithValue("@Login", parentLogin);

                using (MySqlDataAdapter adapterPayments = new MySqlDataAdapter(commandPayments))
                {
                    DataTable dataTablePayments = new DataTable();
                    adapterPayments.Fill(dataTablePayments);

                    int totalPayments = 0;

                    foreach (DataRow rowPayment in dataTablePayments.Rows)
                    {
                        int paymentSum = Convert.ToInt32(rowPayment["payment_sum"]);
                        totalPayments += paymentSum;
                        string paymentDetails = rowPayment["payment_details"].ToString();
                        string paymentDate = rowPayment["payment_date"].ToString();

                        AddPaymentDetailsToStackLayout(paymentSum, paymentDetails, paymentDate);
                    }

                    int remainingBalance;
                    if (totalPayments > 0)
                    {
                        remainingBalance = fullPrice - totalPayments;
                    }
                    else
                    {
                        remainingBalance = fullPrice;
                    }

                    Label remainingBalanceLabel = new Label
                    {
                        Text = $"Остаток: {remainingBalance}",
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(20, 0, 0, 0) 
                    };
                    mainStackLayout.Children.Add(remainingBalanceLabel);
                }
            }

            con.Close();
        }
    }

    private void AddCourseInfoToStackLayout(string courseName, int coursePrice, int totalPrice, int singlePayment)
    {
        Label courseLabel = new Label
        {
            Text = $"Курс: {courseName}",
            Margin = new Thickness(10, 0, 10, 0)
        };

        Label discountedPriceLabel = new Label
        {
            Text = $"Стоимость со скидкой: {totalPrice}",
            Margin = new Thickness(10, 0, 10, 0) 
        };

        Label singlePaymentLabel = new Label
        {
            Text = $"Разовый платеж: {singlePayment}",
            Margin = new Thickness(10, 0, 10, 0) 
        };

        mainStackLayout.Children.Add(courseLabel);
        mainStackLayout.Children.Add(discountedPriceLabel);
        mainStackLayout.Children.Add(singlePaymentLabel);

        mainStackLayout.Spacing = 0;
    }

    private void AddPaymentDetailsToStackLayout(int paymentSum, string paymentDetails, string paymentDate)
    {
        DateTime parsedPaymentDate = DateTime.Parse(paymentDate);
        string formattedPaymentDate = parsedPaymentDate.ToString("dd.MM.yyyy");
        Frame paymentFrame = new Frame
        {
            BackgroundColor = Color.FromHex("#ffffff"),
            Content = new StackLayout
            {
                Children =
                {
                    new Label { Text = $"Сумма платежа: {paymentSum}" },
                    new Label { Text = $"Номер платежа: {paymentDetails}" },
                    new Label { Text = $"Дата: {formattedPaymentDate}" }
                }
            },
            Padding = new Thickness(10),
            Margin = new Thickness(10),
        };

        mainStackLayout.Children.Add(paymentFrame);
    }
}