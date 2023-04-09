using Camera.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace  Camera.models
{
    public class PpeIconRoom
    {
        public int PpeIconId { get; set; }
        public PpeIcon PpeIcon { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
