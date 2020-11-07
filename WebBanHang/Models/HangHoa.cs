using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("HangHoa")]
    public class HangHoa
    {
        [Key]
        public int MaHH { get; set; }
        [Required]
        [MaxLength(100)]
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public string MoTa { get; set; }
        [Range(0, double.MaxValue)]
        public double DonGia { get; set; }
        [Range(0, int.MaxValue)]

        public int SoLuong { get; set; }
        [ForeignKey("MaLoai")]
        public int MaLoai { get; set; }
        public DateTime NgayDang { get; set; }
        [Required(ErrorMessage = "Không để trống!")]
        [Range(0, 100, ErrorMessage = "Giảm giá từ 0% đến 100% !")]
        public int GiamGia { get; set; }
        public Loai Loai { get; set; }
        public int DaMua { get; set; }
        public bool NoiBat { get; set; }
    }
}
