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
                    AllShell.Items.Remove(Parent);
                    AllShell.Items.Remove(Director);
                    AllShell.Items.Remove(Teacher);
                    AllShell.Items.Add(Student);
                }
            };
            WhoAreYou.TeacherSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isTeacherSelected)
                {
                    AllShell.Items.Remove(Student);
                    AllShell.Items.Remove(Parent);                    
                    AllShell.Items.Remove(Director);
                    AllShell.Items.Add(Teacher);
                }
            };
            WhoAreYou.ParentSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isParentSelected)
                {
                    AllShell.Items.Remove(Student);
                    AllShell.Items.Remove(Teacher);
                    AllShell.Items.Remove(Director);
                    AllShell.Items.Add(Parent);
                }
            };
            WhoAreYou.DirectorSelectedChanged += (sender, e) =>
            {
                if (WhoAreYou.isDirectorSelected)
                {
                    AllShell.Items.Remove(Student);
                    AllShell.Items.Remove(Parent);
                    AllShell.Items.Remove(Teacher);
                    AllShell.Items.Add(Director);
                }
            };
        }

    }
}
