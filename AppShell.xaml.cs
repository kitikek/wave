namespace wave
{
    public partial class AppShell : Shell
    {
        private int parentSelectionCount = 0;
        public AppShell()
        {
            InitializeComponent();

            WhoAreYou.StudentSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isStudentSelected)
                {
                    Home.Items.Remove(PaymentContent);
                }
            };
            WhoAreYou.TeacherSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isTeacherSelected)
                {
                    Home.Items.Remove(PaymentContent);
                }
            };
            WhoAreYou.ParentSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isParentSelected)
                {
                    Home.Items.Add(PaymentContent);
                }
            };
            WhoAreYou.DirectorSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isDirectorSelected)
                {
                    Home.Items.Remove(PaymentContent);
                }
            };
        }

    }
}
