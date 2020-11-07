using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("Oder")]
    public class Oder
    {
        [Key]
        public int ID { get; set; }
        public bool Status { get; set; }
        public int CustomerID { get; set; }
        public string ShipName { get; set; }
        public int ShipMobile { get; set; }
        public string ShipAddress { get; set; }
        public string ShipEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
