using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.ViewModels
{
    public class OrderDetailViewModel
    {
        public int ID { get; set; }
        public string TenHH { get; set; }
        public string Loai { get; set; }
        public int Quantity { get; set; }
        public double Gia { get; set; }
        public int GiamGia { get; set; }
        public double ThanhTien { get; set; }
    }
}
