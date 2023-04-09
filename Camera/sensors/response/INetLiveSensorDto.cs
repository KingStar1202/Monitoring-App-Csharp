using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.response
{
    public class INetLiveSensorDto
    {
        public int status { get; set; }
        public string state { get; set; }
        public string gasCode { get; set; }
        public double gasReading { get; set; }
        public string measurementTypeCode { get; set; }


    }
}
