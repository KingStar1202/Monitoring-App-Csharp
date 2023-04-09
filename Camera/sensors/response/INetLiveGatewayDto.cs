using System.Collections.Generic;
using System;

namespace Camera.sensors.response
{
    public class INetLiveGatewayDto
    {
        public string id { get; set ; }

        public string sn{ get; set; }

        public string equipmentCode{ get; set; }

        public string type{ get; set; }

        public string time{ get; set; }

        public string updateTime{ get; set; }

        public double batteryLevel{ get; set; }

        public int rssi{ get; set; }

        public string site{ get; set; }

        public string status{ get; set; }

        public List<string> states{ get; set; }
    }
}