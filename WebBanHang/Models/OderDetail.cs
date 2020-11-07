using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("OderDetail")]
    public class OderDetail
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("OderID")]
        public int OderID { get; set; }
        public int Quantity { get; set; }
        public double Gia { get; set; }
        [ForeignKey("MaHH")]
        public int MaHH { get; set; }
        public Oder oder { get; set; }
    }
}
