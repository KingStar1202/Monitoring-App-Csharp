﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.sensors.dto
{
    public class INetLiveAuth
    {
        public string clientId {get;set;}
        public string clientSecret { get; set; }
        public string user { get; set; }
        public string pw { get; set; }
        public string accountId { get; set; }
    }
}
