using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        
            [Key]
            [Display(Name = "Mã TK")]

            public string MaTK { get; set; }
          
            
            public string TenDangNhap { get; set; }
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        
    }
}

