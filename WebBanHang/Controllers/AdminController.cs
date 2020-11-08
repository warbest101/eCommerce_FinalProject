using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MyDBContext _context;

        private readonly string admin = "Admin";

        public AdminController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            if (User.Identity.Name == admin)
            {
                return View();
            }
            else return RedirectToAction("ManageAccount", "ManageAccount");

        }

        public IActionResult CheckAdmin(string TenDangNhap)
        {
            if (TenDangNhap == admin)
            {
                return Json("không được đăng ký tên admin");
            }
            else return Json(true);
        }



        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"].ToString();
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Taikhoan()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View(await _context.TaiKhoans.ToListAsync());
        }
    }
}