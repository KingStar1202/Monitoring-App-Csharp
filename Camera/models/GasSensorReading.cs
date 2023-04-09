using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("gas_sensor_reading")]
    public class GasSensorReading
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("room_id")]
        public Room room { get; set; }
        [Column("type")]
        public string type { get; set; }
        [Column("state")]
        public string state { get; set; }
        [Column("value")]
        public double value { get; set; }
        [Column("measurement_type")]
        public string measurementType { get; set; }
        [Column("created_at")]
        public DateTime createdAt { get; set; }
    }
}
