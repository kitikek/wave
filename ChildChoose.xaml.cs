namespace wave;

public partial class ChildChoose : ContentPage
{
	public static int childCount;

    public static string[] children = new string[childCount];
	public ChildChoose()
	{
		InitializeComponent();
	}
	public async void CountChildren(object sender, EventArgs e)
	{

	}
}