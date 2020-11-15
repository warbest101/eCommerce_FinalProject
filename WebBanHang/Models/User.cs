using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Phone number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Two Factor Authentication")]
        public bool Enable2FA { get; set; }
    }
}
