using MySqlConnector;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace wave;

public partial class PaymentDirector : ContentPage
{
    private Picker groupPicker;
    private Picker studentPicker;
    private Entry amountEntry;
    private Entry chequeEntry;
    private DatePicker datePicker;
    private StackLayout mainStackLayout;
    private int fullPrice;
    private string selectedGroup;
    private string selectedStudentName;
    private string selectedStudentSurname;

    public PaymentDirector()
    {
        InitializeComponent();
        SetupUI();
    }

    private void SetupUI()
    {
        groupPicker = new Picker();
        studentPicker = new Picker();
        amountEntry = new Entry { Placeholder = "Сумма платежа" };
        chequeEntry = new Entry { Placeholder = "Номер чека" };
        datePicker = new DatePicker { Format = "yyyy-MM-dd", Date = DateTime.Now };
        mainStackLayout = new StackLayout { VerticalOptions = LayoutOptions.FillAndExpand };

        groupPicker.SelectedIndexChanged += GroupPicker_SelectedIndexChanged;
        studentPicker.SelectedIndexChanged += StudentPicker_SelectedIndexChanged;
        ScrollView scrollView = new ScrollView
        {
            Content = mainStackLayout,
            VerticalOptions = LayoutOptions.FillAndExpand 
        };

        Content = new StackLayout
        {
            Children = {
            new Label { Text = "Выберите группу:" },
            groupPicker,
            new Label { Text = "Выберите ученика:" },
            studentPicker,
            amountEntry,
            chequeEntry,
            datePicker,
            new Button { Text = "Добавить платеж", Command = new Command(AddPayment) },
            new Button { Text = "Выгрузить в Excel", Command = new Command(ExportToExcel) },
            scrollView
        },
            VerticalOptions = LayoutOptions.FillAndExpand 
        };
        


        LoadGroups();
    }
    private void ExportToExcel()
    {
        try
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string selectedGroup = groupPicker.SelectedItem?.ToString();
            DateTime Fulldate = DateTime.Now;
            string date = Fulldate.ToString("dd.MM.yyyy");
            string fileName = $"{selectedGroup}_payments_{date}.xlsx";
            string filePath = @"C:\Users\79519\OneDrive\Рабочий стол\Курсач\" + fileName;
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Платежи");
                worksheet.Cells[1, 1].Value = "ФИО";
                worksheet.Cells[1, 2].Value = "Сумма платежа";
                worksheet.Cells[1, 3].Value = "Детали платежа";
                worksheet.Cells[1, 4].Value = "Дата платежа";

                int row = 1;
                DataTable paymentData = GetPayments(selectedGroup);
                foreach (DataRow rowData in paymentData.Rows)
                {
                    int columnIndex = 2;
                    foreach (var item in rowData.ItemArray)
                    {
                        worksheet.Cells[row, columnIndex].Value = item.ToString();
                        columnIndex++;
                    }
                    row++;
                }
                package.Save();
            }

            DisplayAlert("Выгрузка завершена", $"Данные платежей для группы {selectedGroup} успешно выгружены в файл {fileName}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Ошибка", $"Возникла ошибка при выгрузке данных в Excel: {ex.Message}", "OK");
        }
    }
    private void LoadGroups()
    {
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT * FROM groups";
            MySqlCommand command = new MySqlCommand(query, connection);
            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    groupPicker.Items.Add(reader["group_name"].ToString());
                }
            }
        }
    }

    private void GroupPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        studentPicker.Items.Clear();
        selectedGroup = groupPicker.SelectedItem?.ToString();

        mainStackLayout.Children.Clear();

        string connectionString = ConnString.connString;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT student.student_id, CONCAT(users.user_surname, ' ', users.user_name, ' ', users.user_patronymic) AS full_name FROM student JOIN students_groups ON student.student_id = students_groups.student_id JOIN groups ON groups.group_id = students_groups.group_id JOIN users ON student.student_user_id = users.user_id WHERE groups.group_name = @GroupName";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@GroupName", selectedGroup);

            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    studentPicker.Items.Add(reader["full_name"].ToString());
                }
            }
        }
    }

    private void StudentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        mainStackLayout.Children.Clear();

        string selectedStudentFullName = studentPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedStudentFullName))
        {
            string[] names = selectedStudentFullName.Split(' ');
            if (names.Length >= 2)
            {
                selectedStudentName = names[1]; 
                selectedStudentSurname = names[0]; 

                LoadCourseInfo(selectedStudentName, selectedStudentSurname);
                LoadPaymentInfo(selectedStudentName, selectedStudentSurname);
            }
        }
    }

    private void AddPayment()
    {
        try
        {
            string amount = amountEntry.Text;
            string chequeNumber = chequeEntry.Text;
            string date = datePicker.Date.ToString("yyyy-MM-dd");
            int groupId = 0;
            int studentId = 0;

            string connectionString = ConnString.connString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string queryGroupId = "SELECT group_id FROM groups WHERE group_name = @GroupName";
                MySqlCommand commandGroupId = new MySqlCommand(queryGroupId, connection);
                commandGroupId.Parameters.AddWithValue("@GroupName", selectedGroup);
                groupId = Convert.ToInt32(commandGroupId.ExecuteScalar());

                string queryStudentId = "SELECT student_id FROM student JOIN users ON student.student_user_id = users.user_id WHERE users.user_name = @UserName AND users.user_surname = @UserSurname";
                MySqlCommand commandStudentId = new MySqlCommand(queryStudentId, connection);
                commandStudentId.Parameters.AddWithValue("@UserName", selectedStudentName);
                commandStudentId.Parameters.AddWithValue("@UserSurname", selectedStudentSurname);
                studentId = Convert.ToInt32(commandStudentId.ExecuteScalar());

                string queryPaymentInsert = "INSERT INTO payment (payment_contract_id, payment_sum, payment_date, payment_details) VALUES (@ContractId, @Amount, @Date, @ChequeNumber)";
                MySqlCommand commandPaymentInsert = new MySqlCommand(queryPaymentInsert, connection);
                commandPaymentInsert.Parameters.AddWithValue("@ContractId", GetContractId(groupId, studentId));
                commandPaymentInsert.Parameters.AddWithValue("@Amount", amount);
                commandPaymentInsert.Parameters.AddWithValue("@ChequeNumber", chequeNumber);
                commandPaymentInsert.Parameters.AddWithValue("@Date", date);
                commandPaymentInsert.ExecuteNonQuery();
                mainStackLayout.Clear();
                LoadCourseInfo(selectedStudentName, selectedStudentSurname);
                LoadPaymentInfo(selectedStudentName, selectedStudentSurname);
            }
            amountEntry.Text = "";
            chequeEntry.Text = "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении платежа: {ex.Message}");
        }
    }

    private int GetContractId(int groupId, int studentId)
    {
        int contractId = 0;
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string query = "SELECT contract_id FROM contract WHERE contract_student_id = @StudentId AND contract_course_id = @GroupId";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@StudentId", studentId);
            command.Parameters.AddWithValue("@GroupId", groupId);
            connection.Open();
            contractId = (int)command.ExecuteScalar();
        }
        return contractId;
    }




    private void LoadCourseInfo(string selectedStudentName, string selectedStudentSurname)
    {
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            int totalPrice = 0;
            int singlePayment = 0;

            string query = @"SELECT c.course_name, c.course_price, c.course_time, s.scheme_value, con.contract_sale
                FROM course c
                JOIN contract con ON c.course_id = con.contract_course_id
                JOIN scheme s ON s.scheme_id = c.course_scheme_id
                JOIN student st ON st.student_id = con.contract_student_id
                JOIN users u ON u.user_id = st.student_user_id
                WHERE u.user_name = @studentName AND u.user_surname = @studentSurname";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@studentName", selectedStudentName);
                command.Parameters.AddWithValue("@studentSurname", selectedStudentSurname);

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
                        }
                        else
                        {
                            totalPrice = coursePrice * courseTime;
                        }

                        fullPrice = totalPrice;
                        singlePayment = totalPrice / schemeValue;

                        AddCourseInfoToStackLayout(courseName, coursePrice, totalPrice, singlePayment);
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
        }
    }
    private DataTable GetPayments(string selectedGroup)
    {
        DataTable paymentData = new DataTable();
        string connectionString = ConnString.connString;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            string query = @"SELECT payment_sum, payment_details, payment_date
                             FROM payment
                             INNER JOIN contract ON payment.payment_contract_id = contract.contract_id
                             INNER JOIN groups ON contract.contract_course_id = groups.group_id
                             WHERE groups.group_name = @GroupName";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@GroupName", selectedGroup);

            connection.Open();

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
            {
                adapter.Fill(paymentData);
            }
        }

        return paymentData;
    }
    private void LoadPaymentInfo(string selectedStudentName, string selectedStudentSurname)
    {
        string connectionString = ConnString.connString;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string queryPayments = @"SELECT p.payment_sum, p.payment_details, p.payment_date
                            FROM payment p
                            INNER JOIN contract c ON p.payment_contract_id = c.contract_id
                            INNER JOIN student s ON c.contract_student_id = s.student_id
                            INNER JOIN users u ON s.student_user_id = u.user_id
                            WHERE u.user_name = @selectedStudentName AND u.user_surname = @selectedStudentSurname";

            using (MySqlCommand command = new MySqlCommand(queryPayments, connection))
            {
                command.Parameters.AddWithValue("@selectedStudentName", selectedStudentName);
                command.Parameters.AddWithValue("@selectedStudentSurname", selectedStudentSurname);

                using (MySqlDataAdapter adapterPayments = new MySqlDataAdapter(command))
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