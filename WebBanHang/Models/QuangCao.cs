using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("QuangCao")]
    public class QuangCao
    {
        [Key]
        public int ID { get; set; }
        public string Hinh { get; set; }
        [ForeignKey("MaHH")]
        public int MaHH { get; set; }
    }
}
