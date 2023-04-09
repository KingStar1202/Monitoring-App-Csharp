using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.response
{
    public class INetLiveResponseDto
    {
        public  string requestTime{get;set;}

        public List<INetLiveGatewayDto> gateway { get; set; }

        public List<INetLiveInstrumentDto> instrument { get; set; }

        public List<INetLiveHistoryDto> history { get; set; }
    }
}
