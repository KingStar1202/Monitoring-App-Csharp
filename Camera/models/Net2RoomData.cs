using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("room_data")]
    public class Net2RoomData
    {
        [Column("id")]
        public int id { get; set; }
        [Column("room_name")]
        public string roomName { get; set; }
        [Column("person")]
        public string person { get; set; }
        [Column("lastLogin")]
        public string lastLogin { get; set; }

    }
}
