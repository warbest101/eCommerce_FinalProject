using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.ViewModels
{
    public class ProductViewmodels
    {
        
        public int MaHH { get; set; }
        
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public string MoTa { get; set; }
      
        public double DonGia { get; set; }
      
        public int SoLuong { get; set; }
        public int MaLoai { get; set; }
        public DateTime NgayDang { get; set; }
      
        public Loai Loai { get; set; }
    }
}
