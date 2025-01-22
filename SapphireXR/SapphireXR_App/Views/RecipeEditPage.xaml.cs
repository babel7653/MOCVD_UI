﻿using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class RecipeEditPage : Page
    {
        public RecipeEditPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(RecipeEditViewModel));
        }
    }
}
