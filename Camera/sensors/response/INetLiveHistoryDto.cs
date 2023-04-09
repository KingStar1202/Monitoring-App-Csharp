using System;

namespace Camera.sensors.response
{
    public class INetLiveHistoryDto
    {
        public string id{ get; set; }

        public string equipmentId{ get; set; }

        public string sn{ get; set; }

        public string updateTime{ get; set; }

        public string type{ get; set; }

        public string equipmentCode{ get; set; }

        public bool reserved{ get; set; }

        public bool monitored{ get; set; }

        public string beginTime{ get; set; }

        public string lastContact{ get; set; }

        public string lastLiveUploadId{ get; set; }

        public int lastStatus{ get; set; }

        public INetLivePositionDto lastDevicePosition{ get; set; }

        public string lastDevicePositionSource{ get; set; }

        public string lastDevicePositionTime{ get; set; }

        public string lastSite{ get; set; }

        public bool shortTermLost{ get; set; }

        public bool longTermLost{ get; set; }

        public string lastRecUpdateEntity{ get; set; }

        public string lastRecUpdateEntityId{ get; set; }
    }
}