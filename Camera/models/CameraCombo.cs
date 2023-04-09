using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("camera_combo")]
    public class CameraCombo
    {
        [Column("id")]
        public int id { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("outside_ip")]
        public string outsideIp { get; set; }
        [Column("inside_ip")]
        public string insideIp { get; set; }

    }
}
