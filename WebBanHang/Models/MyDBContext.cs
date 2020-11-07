using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Models
{
    public class MyDBContext : IdentityDbContext<AppUser>

    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {

        }

        public DbSet<Loai> loais { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<Oder> Oders { get; set; }
        public DbSet<OderDetail> OderDetails { get; set; }



        public DbSet<BaiViet> BaiViet { get; set; }

        public DbSet<QuangCao> QuangCao { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
