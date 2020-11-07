using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("BaiViet")]
    public class BaiViet
    {
        [Key]
        public int ID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string Hinh { get; set; }
        [ForeignKey("MaLoai")]
        public int MaLoai { get; set; }
    }
}
