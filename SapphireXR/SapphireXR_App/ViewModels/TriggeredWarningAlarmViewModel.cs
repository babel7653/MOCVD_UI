﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
    internal partial class TriggeredWarningAlarmViewModel: ObservableObject
    {
        internal TriggeredWarningAlarmViewModel(PLCService.TriggerType type)
        {
            IconSource = (type == PLCService.TriggerType.Alarm) ? "/Resources/icons/icon=alert_red.png" : "/Resources/icons/icon=alert_yellow.png";
            TitleColor = (type == PLCService.TriggerType.Alarm) ? new SolidColorBrush(Color.FromRgb(0xEC, 0x3D, 0x3F)) : new SolidColorBrush(Color.FromRgb(0xFF, 0x8D, 0x60));
            Title = type.ToString();
            Message = "Please check the " + Title.ToLower() + " events";

            resetToPLC = (type == PLCService.TriggerType.Alarm) ? PLCService.WriteAlarmReset : PLCService.WriteWarningReset;
            refreshOnListFunc = (type == PLCService.TriggerType.Alarm) ? refreshAlarmList : refreshWarningList;
            OnList = refreshOnListFunc(type);

            onListUpdater = new DispatcherTimer();
            onListUpdater.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 500);
            onListUpdater.Tick += (sender, args) =>
            {
                OnList = refreshOnListFunc(type);
            };
            onListUpdater.Start();
        }

        private static string? GetAnalogDeviceNotificationName(uint index)
        {
            string key;
            switch (index)
            {
                case 0:
                    key = "R01";
                    break;

                case 1:
                    key = "R02";
                    break;

                case 2:
                    key = "R03";
                    break;

                case 3:
                    key = "M01";
                    break;

                case 4:
                    key = "M02";
                    break;

                case 5:
                    key = "M03";
                    break;

                case 6:
                    key = "M04";
                    break;

                case 7:
                    key = "M05";
                    break;

                case 8:
                    key = "M06";
                    break;

                case 9:
                    key = "M07";
                    break;

                case 10:
                    key = "M08";
                    break;

                case 11:
                    key = "M09";
                    break;

                case 12:
                    key = "M10";
                    break;

                case 13:
                    key = "M11";
                    break;

                case 14:
                    key = "M12";
                    break;

                case 15:
                    key = "M13";
                    break;

                case 16:
                    key = "M14";
                    break;

                case 17:
                    key = "M15";
                    break;

                case 18:
                    key = "M16";
                    break;

                case 19:
                    key = "M17";
                    break;

                case 20:
                    key = "M18";
                    break;

                case 21:
                    key = "M19";
                    break;

                case 22:
                    key = "E01";
                    break;

                case 23:
                    key = "E02";
                    break;

                case 24:
                    key = "E03";
                    break;

                case 25:
                    key = "E04";
                    break;

                case 26:
                    key = "E05";
                    break;

                case 27:
                    key = "E06";
                    break;

                case 28:
                    key = "E07";
                    break;

                default:
                    return null;
            }

            AnalogDeviceIO? analogDeviceIO = null;
            if(SettingViewModel.dAnalogDeviceIO.TryGetValue(key, out analogDeviceIO) == true)
            {
                return analogDeviceIO.Name;
            }
            else
            {
                return null;
            }
        }

        private static string? GetDigitalDeviceNotificationName(uint index)
        {
            string key;
            switch (index)
            {
                case 0:
                    key = "A01";
                    break;

                case 1:
                    key = "A02";
                    break;

                case 2:
                    key = "A03";
                    break;

                case 3:
                    key = "A04";
                    break;

                case 4:
                    key = "A05";
                    break;

                case 5:
                    key = "A06";
                    break;

                case 6:
                    key = "A07";
                    break;

                case 7:
                    key = "A08";
                    break;

                case 8:
                    key = "A09";
                    break;

                case 9:
                    key = "A10";
                    break;

                case 10:
                    key = "A11";
                    break;

                case 11:
                    key = "A12";
                    break;

                case 12:
                    key = "A13";
                    break;

                case 13:
                    key = "A14";
                    break;

                default:
                    return null;
            }
            SwitchDI? switchDI = null;
            if(SettingViewModel.dSwitchDI?.TryGetValue(key, out switchDI) == true)
            {
                return switchDI.Name;
            }
            else
            {
                return null;
            }
        }

        private List<string> refreshAlarmList(PLCService.TriggerType triggerType)
        {
            return refreshOnList(PLCService.ReadDigitalDeviceAlarms, PLCService.ReadAnalogDeviceAlarms, triggerType, KeysAlarmEventLogged);
        }

        private  List<string> refreshWarningList(PLCService.TriggerType triggerType)
        {
            return refreshOnList(PLCService.ReadDigitalDeviceWarnings, PLCService.ReadAnalogDeviceWarnings, triggerType, KeysWarningEventLogged);
        }

        private List<string> refreshOnList(Func<int> plcServiceReadDigitalState, Func<int> plcServiceReadAnalogState, PLCService.TriggerType triggerType, HashSet<string> keysEventLogged)
        {
            List<string> onList = new List<string>();

            int digitalDeviceAlarms = plcServiceReadDigitalState();
            for(uint digitalDevice = 0; digitalDevice < PLCService.NumDigitalDevice; ++digitalDevice)
            {
                string? notificationName = GetDigitalDeviceNotificationName(digitalDevice);
                if (notificationName != null)
                {
                    if (PLCService.ReadBit(digitalDeviceAlarms, (int)digitalDevice) == true)
                    {
                        onList.Add(notificationName);
                        if (keysEventLogged.Contains(notificationName) == false)
                        {
                            EventLogs.Instance.EventLogList.Add(new EventLog()
                            {
                                Date = DateTime.Now,
                                Message = notificationName,
                                Name = (triggerType == PLCService.TriggerType.Alarm) ? "Digital Alarm" : "Digital Warning",
                                Type = (triggerType == PLCService.TriggerType.Alarm) ? EventLog.LogType.Alarm : EventLog.LogType.Warning
                            });
                            keysEventLogged.Add(notificationName);
                        }
                    }
                    else
                    {
                        keysEventLogged.Remove(notificationName);
                    }
                }
            }

            int analogDeviceAlarms = plcServiceReadAnalogState();
            for(uint analogDevice = 0; analogDevice < PLCService.NumAnalogDevice; ++analogDevice)
            {
                string? notificationName = GetAnalogDeviceNotificationName(analogDevice);
                if (notificationName != null)
                {
                    if (PLCService.ReadBit(analogDeviceAlarms, (int)analogDevice) == true)
                    {
                        string message = notificationName + " Deviation!";
                        onList.Add(message);
                        if (keysEventLogged.Contains(notificationName) == false)
                        {
                            EventLogs.Instance.EventLogList.Add(new EventLog()
                            {
                                Date = DateTime.Now,
                                Message = message,
                                Name = (triggerType == PLCService.TriggerType.Alarm) ? "Analog Alarm" : "Analog Warning",
                                Type = (triggerType == PLCService.TriggerType.Alarm) ? EventLog.LogType.Alarm : EventLog.LogType.Warning
                            });
                            keysEventLogged.Add(notificationName);
                        }
                    }
                    else
                    {
                        keysEventLogged.Remove(notificationName);
                    }
                }
            }

            return onList;
        }


        [RelayCommand]
        private void Close(Window window)
        {
            resetToPLC();
            onListUpdater.Stop();
            window.Close();
        }

        [RelayCommand]
        private void Reset()
        {
            resetToPLC();
        }

        public string IconSource { get; set; }
        public string Title { get; set; }
        public Brush TitleColor { get; set; }
        public string Message { get; set; }

        [ObservableProperty]
        private List<string> _onList;

        private Action resetToPLC;
        private Func<PLCService.TriggerType, List<string>> refreshOnListFunc;
        private static HashSet<string> KeysAlarmEventLogged = new HashSet<string>();
        private static HashSet<string> KeysWarningEventLogged = new HashSet<string>();

        private DispatcherTimer onListUpdater;
    }
}
