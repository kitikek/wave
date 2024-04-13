using Microsoft.Maui.Controls;

namespace wave
{
    public partial class DocumentsParent : ContentPage
    {
        public DocumentsParent()
        {
            InitializeComponent();
            AddOpenButton();
        }

        private void AddOpenButton()
        {
            StackLayout stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 10 
            };

            Label label = new Label
            {
                Text = "Чтобы открыть папку с документами, нажмите на кнопку",
                HorizontalOptions = LayoutOptions.Center
            };
            stackLayout.Children.Add(label);

            Button openButton = new Button
            {
                Text = "Открыть",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            openButton.Clicked += async (sender, args) =>
            {
                await Launcher.OpenAsync("https://drive.google.com/drive/folders/18El5UBnRANgIMfvuoymusHMervVa-DNK?usp=sharing");
            };
            stackLayout.Children.Add(openButton);

            Content = stackLayout;
        }
    }
}
