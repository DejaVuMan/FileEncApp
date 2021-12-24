using FileEncApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace FileEncApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}