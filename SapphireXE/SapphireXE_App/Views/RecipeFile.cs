using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SapphireXE_App.Views
{
  /// <summary>
  /// RecipeControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class RecipeControl : Page
  {
    private void FileNew_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog newfile = new OpenFileDialog();
      newfile.Multiselect = false;
      newfile.Filter = "모든파일(*.*)|*.*";

      if (newfile.ShowDialog() == true)
      {
        string filepath = newfile.FileName;
        MessageBox.Show(filepath, "New file");

      }
    }

    private void FileOpen_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFile = new OpenFileDialog();
      openFile.Multiselect = false;
      openFile.Filter = "모든파일(*.*)|*.*";

      if (openFile.ShowDialog() == true)
      {
        string filepath = openFile.FileName;
        MessageBox.Show(filepath, "Open file");

      }
    }

    private void FileSave_Click(object sender, RoutedEventArgs e)
    {
      SaveFileDialog saveFile = new SaveFileDialog();
      saveFile.Filter = "모든파일(*.*)|*.*";

      if (saveFile.ShowDialog() == true)
      {
        string filepath = saveFile.FileName;
        MessageBox.Show(filepath, "Save file");

      }

    }

    private void FileSaveAs_Click(object sender, RoutedEventArgs e)
    {
      SaveFileDialog saveAsFlie = new SaveFileDialog();
      saveAsFlie.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
      saveAsFlie.Title = "Save an Excel File";

      if (saveAsFlie.ShowDialog() == true)
      {
        string filepath = saveAsFlie.FileName;
        MessageBox.Show(filepath, "Save as file");

      }

    }
  }
}
