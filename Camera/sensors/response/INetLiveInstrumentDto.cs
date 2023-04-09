using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.response
{
    public class INetLiveInstrumentDto
    {
        public string Id { get; set; }

        public string sn { get; set; }

        public string time { get; set; }

        public string updateTime { get; set; }

        public int sequence { get; set; }

        public int status { get; set; }

        public List<string> states { get; set; }

        public bool alarm { get; set; }

        public string user { get; set; }

        public string userAL { get; set; }

        public string site { get; set; }

        public string siteAL { get; set; }

        public string equipmentCode { get; set; }

        public string gatewayId { get; set; }

        public string userId { get; set; }

        public List<INetLiveSensorDto> sensors { get; set; }
    }
}
