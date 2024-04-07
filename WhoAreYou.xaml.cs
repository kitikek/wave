namespace wave;

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

        StudentSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    private async void TeacherButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Authorization");
        isTeacherSelected = true;

        TeacherSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    private async void ParentButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Authorization");
        isParentSelected = true;

        ParentSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    private async void DirectorButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Authorization");
        isDirectorSelected = true;

        DirectorSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
}