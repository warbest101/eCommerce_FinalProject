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
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
        [Display(Name = "Hình")]
        public string Hinh { get; set; }
        [ForeignKey("MaLoai")]
        [Display(Name = "Loại")]
        public int MaLoai { get; set; }
        public Loai loai { get; set; }
    }
}
