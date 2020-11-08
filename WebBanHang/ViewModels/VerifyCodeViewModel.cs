using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Code { get; set; }
    }
}
