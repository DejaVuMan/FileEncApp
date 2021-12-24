using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FileEncApp.ViewModels
{
    public class MainPageModel : BaseViewModel
    {
        public MainPageModel()
        {
            Title = "Main Page";
            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        //public ICommand OpenWebCommand { get; }
    }
}