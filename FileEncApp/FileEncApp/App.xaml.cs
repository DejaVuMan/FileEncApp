using FileEncApp.Services;
using FileEncApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FileEncApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();

            // Get the current theme
            //OSAppTheme currentTheme = Application.Current.RequestedTheme; // returns "Light" or "Dark"
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
