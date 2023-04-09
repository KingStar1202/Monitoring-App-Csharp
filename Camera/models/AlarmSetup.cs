using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("alarm_setup")]
    public class AlarmSetup
    {
        [Column("Id")]
        public int id { get; set; }
        [Column("alarm_outside_ip")]
        public string outsideAlamIp { get; set; }
        [Column("alarm_inside_ip")]
        public string insideAlarmIp { get; set; }
        [Column("name")]
        public string name { get; set; }

    }
}
