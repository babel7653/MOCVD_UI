using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using SapphireXR_App.Enums;
using System.Windows;
using TwinCAT.TypeSystem;



namespace SapphireXR_App.Views
{
    public partial class HomePage : Page
    {
        public static bool IsMaintenanceMode { get; set; } = false;
        public HomePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(HomeViewModel));
        }
    }
}
