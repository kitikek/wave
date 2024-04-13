namespace wave
{
    public partial class WhoAreYou : ContentPage
    {
        public static bool isStudentSelected;
        public static bool isTeacherSelected;
        public static bool isParentSelected;
        public static bool isDirectorSelected;
        public static event EventHandler StudentSelectedChanged;
        public static event EventHandler TeacherSelectedChanged;
        public static event EventHandler ParentSelectedChanged;
        public static event EventHandler DirectorSelectedChanged;

        public WhoAreYou()
        {
            InitializeComponent();
        }
        private async void StudentButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Authorization");
            isStudentSelected = true;
            isTeacherSelected = false;
            isParentSelected = false;
            isDirectorSelected = false;

            StudentSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        private async void TeacherButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Authorization");
            isStudentSelected = false;
            isTeacherSelected = true;
            isParentSelected = false;
            isDirectorSelected = false;

            TeacherSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        private async void ParentButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Authorization");
            isStudentSelected = false;
            isTeacherSelected = false;
            isParentSelected = true;
            isDirectorSelected = false;

            ParentSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        private async void DirectorButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Authorization");
            isStudentSelected = false;
            isTeacherSelected = false;
            isParentSelected = false;
            isDirectorSelected = true;

            DirectorSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
