using Camera.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camera.camera.types;

namespace Camera.models
{
    [Table("rooms")]
    public class Room
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("roomId")]
        public int roomId { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("inside_camera")]
        public string insideCamera { get; set; }
        [Column("outside_camera")]
        public string outsideCamera { get; set; }
        [Column("access_control")]
        public string accessControl { get; set; }
        [Column("numberOfPeopleInside")]
        public int? mumberOfPeopleInside { get; set; }
        [Column("x")]
        public int x { get; set; }
        [Column("y")]
        public int y { get; set; }
        [Column("instrument_sn")]
        public string instrumentSn { get; set; }

        public virtual ICollection<GasSensorReading> gasSensorReadingList { get; set; }
        public virtual ICollection<PpeIcon> ppeIcons { get; set; }
        public virtual AlarmSetup alarmSetup { get; set; }
        [NotMapped]
        public CameraType outsideCameraType { get; set; }
        [NotMapped]
        public CameraType insideCameraType { get; set; }

    }
}
