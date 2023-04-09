using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("operator_awareness")]
    public class OperatorAwareness
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("user")]
        public string user { get; set; }
        [Column("time_taken")]
        public long timeTaken { get; set; }
        [Column("created_at")]
        public DateTime createAt { get; set; }
    }
}
