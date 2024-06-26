﻿
namespace wave
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.X = 140;
            window.Y = 20;

            window.Width = 1300;
            window.Height = 800;

            window.MinimumWidth = 1300;
            window.MinimumHeight = 800;

            return window;
        }
    }
}
