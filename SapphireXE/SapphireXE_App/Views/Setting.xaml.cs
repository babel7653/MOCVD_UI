using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SapphireXE_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using SapphireXE_App.Models;
using System.Linq;

namespace SapphireXE_App.Views
{
    public partial class Setting : Page
    {
        public Setting()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(SettingViewModel));
            comboSystemStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
            comboAlarmStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
            comboRecipeEnd.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
        }
    }
}
