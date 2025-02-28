using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Resources;
using System.Windows.Controls;

namespace SapphireXR_App.Common
{
    static class Util
    {
        public static T? FindParent<T>(DependencyObject child, string parentName)
          where T : DependencyObject
        {
            if (child == null) return null;

            T? foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                if (frameworkElement?.Name == parentName && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }

        public static bool IsTextNumeric(string str)
        {
            return reg.IsMatch(str);

        }
        static readonly System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^\\d+$");

        static public bool CheckValid(string curStr, string newStr, int caretPosition, int maxValue)
        {
            if (IsTextNumeric(newStr) == true)
            {
                
                string nextValueStr = curStr.Substring(0, caretPosition) + newStr + curStr.Substring(caretPosition);
                int nextValue = int.Parse(nextValueStr);
                if (nextValue <= maxValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false; ;
            }
        }

        public static void OnlyAllowNumber(RoutedEventArgs e, string textInput)
        {
            e.Handled = !IsTextNumeric(textInput);
        }

        public static void OnlyAllowConstrainedNumber(RoutedEventArgs e, string curStr,  string newStr, int caretPosition, int maxValue)
        {
            e.Handled = !CheckValid(curStr, newStr, caretPosition, maxValue);
        }

        public static string? GetResourceAbsoluteFilePath(string subPath)
        {
            Uri uri = new Uri("/Resources/" + subPath.TrimStart('/'), UriKind.Relative);
            StreamResourceInfo info = System.Windows.Application.GetContentStream(uri);
            string? filePath = (info.Stream as FileStream)?.Name;
            info.Stream.Close();

            return filePath;
        }

        public static void SetIfChanged(bool newValue, ref bool? prevValue, Action<bool> onChanged)
        {
            if (prevValue == null || prevValue != newValue)
            {
                onChanged(newValue);
                prevValue = newValue;
            }
        }

        public static void CostraintTextBoxColumnOnlyNumber(TextBox textBox, FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber)
        {
            string validatedFlowControllerValue = flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber.validate(textBox);
            if (validatedFlowControllerValue != textBox.Text)
            {
                int textCaret = textBox.CaretIndex;
                textBox.Text = validatedFlowControllerValue;
                textBox.CaretIndex = textCaret;
            }
        }

        public static readonly Dictionary<string, string> RecipeFlowControlFieldToControllerID = new Dictionary<string, string>
        {
            { "M01", "MFC01" }, { "M02", "MFC02" }, { "M03", "MFC03" }, { "M04", "MFC04" }, { "M05", "MFC05" },
            { "M06", "MFC06" }, { "M07", "MFC07" }, { "M08", "MFC08" }, { "M09", "MFC09" }, { "M10", "MFC10" },
            { "M11", "MFC11" }, { "M12", "MFC12" }, { "M13", "MFC13" }, { "M14", "MFC14" }, { "M15",  "MFC15" },
            { "M16", "MFC16" }, { "M17", "MFC17" }, { "M18", "MFC18" }, {"M19", "MFC19"  },
            { "E01", "EPC01" },  { "E02", "EPC02" }, { "E03", "EPC03" }, { "E04", "EPC04" }, { "E05", "EPC05" },
            { "E06", "EPC06" }, { "E07", "EPC07" }, { "STemp", "Temperature" }, { "RPress", "Pressure" }, { "SRotation", "Rotation" }
        };
    }
}
