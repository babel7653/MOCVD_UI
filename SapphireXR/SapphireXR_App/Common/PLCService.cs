using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace SapphireXR_App.Common
{
    static class PLCService
    {
        static PLCService() 
        {
            Ads = new AdsClient();
            ValveIDtoOutputSolValveIdx = new Dictionary<string, uint>
            {
                //여기에 밸브 와 인덱스 값 매핑 정보를 넣어주세요. 
                { "V05", 4 }
            };
        }

        public static AdsClient Ads
        {
            get; set;
        }

        public static Dictionary<string, uint> ValveIDtoOutputSolValveIdx
        {
            get; set;
        }
    }
}
