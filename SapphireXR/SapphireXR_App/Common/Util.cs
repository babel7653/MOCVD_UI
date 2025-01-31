using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Input;

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
        static readonly System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[0-9]");

        static public bool CheckValid(string curStr, string newStr, int maxValue)
        {
            if (IsTextNumeric(newStr) == true)
            {
                string nextValueStr = curStr + newStr;
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

        static public void OnlyAllowConstrainedNumber(RoutedEventArgs e, string curStr,  string newStr, int maxValue)
        {
            e.Handled = !CheckValid(curStr, newStr, maxValue);
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
