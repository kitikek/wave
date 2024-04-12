namespace wave
{
    public partial class UsersDirector : ContentPage
    {
        
        public UsersDirector()
        {
            InitializeComponent();

            txtHours.TextChanged += OnEntryTextChanged;
            txtHoursCost.TextChanged += OnEntryTextChanged;
            void OnEntryTextChanged(object sender, TextChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    if (!int.TryParse(e.NewTextValue, out _))
                    {
                        ((Entry)sender).Text = e.OldTextValue;
                    }
                }
            }
        }
    }
}
