using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Numerics;
using SapphireXR_App.Models;
using static SapphireXR_App.ViewModels.ManualBatchViewModel;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Enums;
using SapphireXR_App.WindowServices;
using System.Windows.Controls.Primitives;

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

        public static string GetResourceAbsoluteFilePath(string subPath)
        {
            return GetAbsoluteFilePathFromAppRelativePath("\\Resources\\" + subPath);
        }

        public static string GetAbsoluteFilePathFromAppRelativePath(string subPath)
        {
            return AppDomain.CurrentDomain.BaseDirectory + subPath.TrimStart('\\');
        }

        public static void SetIfChanged(bool newValue, ref bool? prevValue, Action<bool> onChanged)
        {
            if (prevValue == null || prevValue != newValue)
            {
                onChanged(newValue);
                prevValue = newValue;
            }
        }

        public static void CostraintTextBoxColumnOnlyNumber(object sender, FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                string validatedStr = flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber.validate(textBox);
                if (validatedStr != textBox.Text)
                {
                    int textCaret = Math.Max(textBox.CaretIndex - 1, 0);
                    textBox.Text = validatedStr;
                    textBox.CaretIndex = textCaret;
                }
            }
        }

        public static void CostraintTextBoxColumnMaxNumber(object sender, FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue, uint maxValue)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                (string valiatedStr, FlowControllerTextBoxValidaterMaxValue.Result result) = flowControllerDataGridTextColumnTextBoxValidaterMaxValue.validate(textBox, maxValue);
                if (FlowControllerTextBoxValidaterMaxValue.Result.NotNumber <= result && result <= FlowControllerTextBoxValidaterMaxValue.Result.ExceedMax)
                {
                    int textCaret = Math.Max(textBox.CaretIndex - 1, 0);
                    textBox.Text = valiatedStr;
                    textBox.CaretIndex = textCaret;
                }
            }
        }

        public static void CostraintTextBoxColumnMaxNumber(object sender, FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                (string? validatedStr, FlowControllerTextBoxValidaterMaxValue.Result result) = flowControllerDataGridTextColumnTextBoxValidaterMaxValue.validate(textBox, e);
                if (FlowControllerTextBoxValidaterMaxValue.Result.NotNumber <= result && result <= FlowControllerTextBoxValidaterMaxValue.Result.ExceedMax)
                {
                    int caretIndex = Math.Max(textBox.CaretIndex - 1, 0);
                    textBox.Text = validatedStr;
                    textBox.CaretIndex = caretIndex;
                }
            }
        }

        public static void ConstraintEmptyToZeroOnDataGridCellCommit(object sender, DataGridCellEditEndingEventArgs e, IList<string> headers)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                string? columnHeader = e.Column.Header as string;
                if (columnHeader != null && headers.Contains(columnHeader) == true)
                {
                    TextBox? editingElement = e.EditingElement as TextBox;
                    if (editingElement != null && editingElement.Text == "")
                    {
                        editingElement.Text = "0";
                    }
                }
            }
        }

        public static bool SynchronizeExpected<T>(T expected, Func<T> checkFunc, long timeOutMS) where T : INumber<T>
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (checkFunc() == expected)
                {
                    return true;
                }

                if (timeOutMS <= stopwatch.ElapsedMilliseconds)
                {
                    return false;
                }
            }
        }

        public static string ToEventLogFormat(DateTime dateTime)
        {
            return dateTime.ToString("yyyy.MM.dd HH:mm:ss");
        }

        public static string FloatingPointStrWithMaxDigit(float value, int maxNumberDigit)
        {
            var numberDecimalDigits = (float value, int maxNumberDigit) =>
            {
                int intValue = (int)value;
                if (0 <= intValue && intValue < 10)
                {
                    return maxNumberDigit - 1;
                }
                else if (10 <= intValue && intValue < 100)
                {
                    return maxNumberDigit - 2;
                }
                else if (100 <= intValue && intValue < 1000)
                {
                    return maxNumberDigit - 3;
                }
                else
                {
                    return 0;
                }
            };
            return value.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = numberDecimalDigits(value, AppSetting.FloatingPointMaxNumberDigit) });
        }

        public static void LoadBatchToPLC(Batch batch)
        {
            if (batch.RampingTime != null)
            {
                PLCService.WriteFlowControllerTargetValue(batch.AnalogIOUserStates.Select((AnalogIOUserState analogIOUserState) => (analogIOUserState.ID, analogIOUserState.Value)).ToArray(),
                    (short)batch.RampingTime);
                int partSize = sizeof(int) * 8;
                BitArray firstValveStates = new BitArray(partSize);
                BitArray secondValveStates = new BitArray(partSize);
                foreach (DigitalIOUserState digitalIOUserState in batch.DigitalIOUserStates)
                {
                    int index;
                    if (PLCService.ValveIDtoOutputSolValveIdx1.TryGetValue(digitalIOUserState.ID, out index) == true)
                    {
                        firstValveStates[index] = digitalIOUserState.On;
                    }
                    else if (PLCService.ValveIDtoOutputSolValveIdx2.TryGetValue(digitalIOUserState.ID, out index) == true)
                    {
                        secondValveStates[index] = digitalIOUserState.On;
                    }
                }
                PLCService.WriteValveState(firstValveStates, secondValveStates);
            }
        }

        public static object? GetSettingValue(JToken rootToken, string key)
        {
            if (rootToken != null)
            {
                JToken? token = rootToken[key];
                if (token != null)
                {
                    return JsonConvert.DeserializeObject(token.ToString());
                }
            }

            return null;
        }

        public static string? GetGasDeviceName(string id)
        {
            return SettingViewModel.GasIO.Where((Device device) => device.ID == id).Select((Device device) => device.Name != null ? device.Name : default).FirstOrDefault();
        }

        public static string? GetFlowControllerName(string id)
        {
            return SettingViewModel.dAnalogDeviceIO.Where((KeyValuePair<string, AnalogDeviceIO> device) => device.Key == id).Select((KeyValuePair<string, AnalogDeviceIO> device) => device.Value.Name != null ? device.Value.Name : default).FirstOrDefault();
        }

        public static string? GetValveName(string id)
        {
            return SettingViewModel.ValveDeviceIO.Where((Device device) => device.ID == id).Select((Device device) => device.Name != null ? device.Name : default).FirstOrDefault();
        }

        public static void ConfirmBeforeToggle(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ToggleButton? toggleSwitch = sender as ToggleButton;
            if (toggleSwitch != null)
            {
                string destState = toggleSwitch.IsChecked == true ? "On" : "Off";
                if (ValveOperationEx.Show("", destState + " 상태로 변경하시겠습니까?") == DialogResult.Cancel)
                {
                    e.Handled = true;
                }
                else
                {
                    toggleSwitch.IsChecked = !toggleSwitch.IsChecked;
                    toggleSwitch.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }

        public static void ShowFlowControllersMaxValueExceededIfExist(HashSet<string>? fcMaxValueExceeded)
        {
            if (fcMaxValueExceeded != null && 0 < fcMaxValueExceeded.Count)
            {
                string message = "Recipe를 읽어오는 중 최대값을 초과한 Flow Controller 값들이 발견되었습니다. 이 값들은 최대값으로 강제됩니다:\r\n";
                foreach (string flowController in fcMaxValueExceeded)
                {
                    message += flowController + " = " + SettingViewModel.ReadMaxValue(flowController) + "\r\n";
                }

                MessageBox.Show(message);
            }
        }

        public static void ConstraintEmptyToZeroOnDataGridCellCommitForRecipeRunEdit(object sender, DataGridCellEditEndingEventArgs e)
        {
            ConstraintEmptyToZeroOnDataGridCellCommit(sender, e, ["Ramp", "Hold", "M01", "M02", "M03", "M04", "M05", "M06", "M07", "M08", "M09", "M10", "M11", "M12", "M13", "M14", "M15", "M16",
               "M17", "M18", "M19", "Loop", "Jump", "Susceptor Temp.", "Reactor Press.", "Sus. Rotation", "Compare Temp."]);
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
