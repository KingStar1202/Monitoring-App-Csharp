using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.dto
{
    public class INetSensorModel
    {
        public string updateTime { get; set; }
        public string historyTime { get; set; }
        public List<string> sn { get; set; }
    }

    public class INetSensorQuery
    {
        public INetSensorModel query { get; set; }
    }
}
