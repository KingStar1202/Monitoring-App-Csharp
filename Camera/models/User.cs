using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.models
{
    [Table("users")]
    public class User
    {
        [Column("Id")]
        [Key]
        public int id { get; set; }
        [Column("username")]
        public string username { get; set; }
        [Column("password")]
        public string password { get; set; }
        [Column("full_name")]
        public string fullName { get; set; }
        [Column("created_at")]
        public DateTime? createAt { get; set; }
        [Column("role")]
        public string role { get; set; }


    }
}
