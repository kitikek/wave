namespace wave
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            WhoAreYou.StudentSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isStudentSelected)
                {
                    Home.Items.Remove(PaymentContent);
                    Home.Items.Remove(TeacherAttContent);
                    Home.Items.Remove(TeacherGradeContent);
                    Home.Items.Add(BaseAttContent);
                    Home.Items.Add(BaseGradeContent);
                }
            };
            WhoAreYou.TeacherSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isTeacherSelected)
                {
                    Home.Items.Remove(PaymentContent);
                    Home.Items.Remove(BaseGradeContent);
                    Home.Items.Remove(BaseAttContent);
                    Home.Items.Add(TeacherAttContent);
                    Home.Items.Add(TeacherGradeContent);                    
                }
            };
            WhoAreYou.ParentSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isParentSelected)
                {
                    Home.Items.Add(PaymentContent);
                    Home.Items.Remove(TeacherAttContent);
                    Home.Items.Remove(TeacherGradeContent);
                    Home.Items.Add(BaseAttContent);
                    Home.Items.Add(BaseGradeContent);
                }
            };
            WhoAreYou.DirectorSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isDirectorSelected)
                {
                    Home.Items.Remove(PaymentContent);
                    Home.Items.Remove(TeacherAttContent);
                    Home.Items.Remove(TeacherGradeContent);
                    Home.Items.Add(BaseAttContent);
                    Home.Items.Add(BaseGradeContent);
                }
            };
        }

    }
}
