using MySqlConnector;

namespace wave
{
    public partial class Support : ContentPage
    {
        List<Question> QuestionList = new List<Question>();
        StackLayout contentStack;
        Label emailLabel;

        public Support()
        {
            InitializeComponent();

            var cs = ConnString.connString;

            using (var con = new MySqlConnection(cs))
            {
                con.Open();
                var cmd = new MySqlCommand(@"SELECT q.question_id, q.question_section_id, q.question_text, q.question_answer, s.section_name
                                    FROM question q
                                    INNER JOIN section s ON q.question_section_id = s.section_id", con);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        QuestionList.Add(new Question()
                        {
                            Id = (int)dr["question_id"],
                            Section = dr["section_name"].ToString(),
                            Text = dr["question_text"].ToString(),
                            Answer = dr["question_answer"].ToString()
                        });
                    }
                }

                Picker sectionPicker = new Picker
                {
                    Title = "¬ыберите тему, по которой у вас вопрос:",
                    ItemsSource = QuestionList.Select(q => q.Section).Distinct().ToList(),
                    Margin = 20
                };
                sectionPicker.SelectedIndexChanged += FilterQuestionsBySection;
                (Content as VerticalStackLayout)?.Children.Add(sectionPicker);

                contentStack = new StackLayout();
                (Content as VerticalStackLayout)?.Children.Add(contentStack);

                emailLabel = new Label
                {
                    Text = "≈сли вы не нашли ответ на свой вопрос, напишите нам!\neamelnik_1@edu.hse.ru\nVdskachko@edu.hse.ru\nevbezvodinskikh@edu.hse.ru",
                    Margin = new Thickness(30, 100, 30, 10),
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                };
                (Content as VerticalStackLayout)?.Children.Add(emailLabel);
            }
        }

        private void FilterQuestionsBySection(object sender, EventArgs e)
        {
            string selectedSection = (sender as Picker)?.SelectedItem?.ToString();
            DisplayQuestionsForSection(selectedSection);
        }

        private void DisplayQuestionsForSection(string section)
        {
            contentStack.Children.Clear();

            foreach (var question in QuestionList.Where(q => string.IsNullOrEmpty(section) || q.Section == section))
            {
                Frame questionFrame = new Frame
                {
                    Padding = new Thickness(5),
                    BackgroundColor = Color.FromHex("#F5F5F5"),
                    Margin = 10,
                    Content = new Label
                    {
                        Text = question.Text
                    }
                };

                Frame answerFrame = new Frame
                {
                    Padding = new Thickness(5),
                    BackgroundColor = Color.FromHex("#ADD8E6"),
                    Margin = new Thickness(30, 0, 10, 0),
                    Content = new Label
                    {
                        Text = question.Answer
                    }
                };

                contentStack.Children.Add(questionFrame);
                contentStack.Children.Add(answerFrame);
            }

        }

        public class Question
        {
            public int Id { get; set; }
            public string Section { get; set; }
            public string Text { get; set; }
            public string Answer { get; set; }
        }
    }
}