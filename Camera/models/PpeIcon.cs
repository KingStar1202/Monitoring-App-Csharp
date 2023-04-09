using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("ppe_icon")]
    public class PpeIcon
    {
        [Key]
        [Column("id")]
        public int id { get; set; }
        [Column("code")]
        public string code { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("image_file")]
        public string imageFile { get; set; }
        public virtual ICollection<Room> rooms { get; set; }

    }
}
