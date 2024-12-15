using SapphireXR_App.Common;
using SapphireXR_App.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TwinCAT.Ads;

namespace SapphireXR_App.Models
{
    static class PLCService
    {
        // Connect to PLC
        public static string AddressPLC { get; set; } = "PLC Address : ";
        public static string ModePLC { get; set; } = "System Mode : ";

        // Variable handles to be connected plc variables
        static uint hStatePLC = 0;
        public static bool TcStatePLC { get; set; }

        //Create an instance of the TcAdsClient()
        public static AdsClient Ads { get; set; }
        static AmsNetId amsNetId = new("10.10.10.10.1.1");

        static PLCService()
        {
            connectedNotifier = ObservableManager<PLCConnection>.Get("PLCService.Connected");
            Ads = new AdsClient();
            try
            {
                Ads.Connect(AmsNetId.Local, 851);
                if (Ads.IsConnected)
                {
                    hStatePLC = Ads.CreateVariableHandle("MAIN.StatePLC");
                    TcStatePLC = (bool)Ads.ReadAny(hStatePLC, typeof(bool));
                    AddressPLC = $"PLC Address : {Ads.Address}";
                    ModePLC = "System Mode : Ready";
                    connectedNotifier.Issue(PLCConnection.Connecrted);
                }
            }
            catch
            {
                MessageBox.Show("TwinCAT이 연결되지 않았습니다.");
                AddressPLC = "PLC Address : ";
                ModePLC = "System Mode : Not Connected";
            }

            ValveIDtoOutputSolValveIdx = new Dictionary<string, uint>
            {
                //여기에 밸브 와 인덱스 값 매핑 정보를 넣어주세요. 
                { "V01", 0 }, { "V02", 1 }, { "V03", 2 }, { "V04", 3 }, { "V05", 4 },
                { "V06", 5 }, { "V07", 6 }, { "V08", 7 }, { "V09", 8 }, { "V10", 9 },
                { "V11", 10 }, { "V12", 11 }, { "V13", 12 }, { "V14", 13 }, { "V15", 14 },
                { "V16", 15 }, { "V36", 16 }, { "V37", 17 }, { "V38", 18 }, { "V39", 19 },
                { "V40", 20 }, { "V41", 21 }, { "V42", 22 }, { "V43", 23 },  { "V44", 24 },
                { "V45", 25 }
            };
        }

        // Read from PLC State
        public static uint hReadValveStatePLC { get; set; }
        private static uint[] aReadValveStatePLC { get; set; }
        public static void ReadValveState()
        {
            // Solenoid Valve State Read(Update)
            try
            {
                hReadValveStatePLC = Ads.CreateVariableHandle("GVL_IO.aOutputSolValve");
                aReadValveStatePLC = (uint[])Ads.ReadAny(hReadValveStatePLC, typeof(uint[]), new int[] { 2 }); // Convert to Array

                BitArray baReadValveStatePLC1 = new BitArray(new int[] { (int)aReadValveStatePLC[0] });
                BitArray baReadValveStatePLC2 = new BitArray(new int[] { (int)aReadValveStatePLC[1] });

                //baReadValveStatePLC1[0] ? true : false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private static ObservableManager<PLCConnection>.DataIssuerBase connectedNotifier;


        public static Dictionary<string, uint> ValveIDtoOutputSolValveIdx
        {
            get; set;
        }
    }
}
