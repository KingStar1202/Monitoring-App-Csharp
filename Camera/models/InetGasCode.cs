using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("inet_gas_code")]
    public class InetGasCode
    {
        [Column("id")]
        public int id { get; set; }
        [Column("gas_code")]
        public string gasCode { set; get; }
        [Column("description")]
        public string description { get; set; }
    }
}
