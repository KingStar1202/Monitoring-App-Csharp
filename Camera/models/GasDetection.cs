using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("gas_detection")]
    public class GasDetection
    {
        [Column("id")]
        public int id { get; set; }
        [Column("serial_number")]
        public string serialNumber { get; set; }
    }
}
