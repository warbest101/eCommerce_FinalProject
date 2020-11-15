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
using ReflectionIT.Mvc.Paging;
using WebBanHang.Models;
using WebBanHang.ViewModels;
using WebBanHang.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebBanHang.FriendlyUrl;

namespace WebBanHang.Controllers
{
    public class TrangChusController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDBContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public TrangChusController(
            MyDBContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender, 
            ISmsSender smsSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            var query = _context.HangHoas.AsNoTracking().OrderByDescending(p => p.NgayDang);
            var models = await PagingList.CreateAsync(query, 4, page);
            var model = _context.loais.ToList();
            ViewBag.model = model;
            var model2 = _context.HangHoas.Where(m => m.NoiBat == true);
            ViewBag.model2 = model2;
            var model3 = _context.HangHoas.AsNoTracking().OrderByDescending(p => p.DaMua);
            var model3s = await PagingList.CreateAsync(model3, 4, page);
            ViewBag.model3s = model3s;
           


            return View(models);
        }

        public IActionResult showbaiviet()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var loai = _context.BaiViet.ToList();


            return View(loai);
        }

        public IActionResult GioiThieu()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            return View();
        }

        public IActionResult DieuKhoan()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            return View();
        }

        public IActionResult ChinhSach()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;


            return View();
        }

        //[Route("lien-he")]
        public IActionResult Contact()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;


            return View();
        }

        //[Route("laptop/{tittle}")]
        //[HttpGet("laptop/{title}-{id}", Name = "show-sp")]
        public async Task<IActionResult> Showsp(int? maloai, string tenloai)
        {
            var model = await _context.loais.ToListAsync();
            ViewBag.model = model;

            if(maloai == null)
            {
                return NotFound();
            }

            var hangHoas = _context.HangHoas.Include(p => p.Loai)
                .Where(m => m.MaLoai == maloai).AsNoTracking().OrderBy(p => p.TenHH);

            var loai = await _context.loais.FirstOrDefaultAsync(m => m.MaLoai == maloai);

            ViewData["TenLoai"] = loai.TenLoai;

            if (loai == null) return NotFound();

            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(loai.TenLoai);

            if (!string.Equals(friendlyTitle, tenloai, StringComparison.Ordinal))
            {
                return RedirectToRoutePermanent("showsp", new { maloai, tenloai = friendlyTitle });
            }


            return View(hangHoas);

        }

        //[Route("laptop", Name = "show-sp-detail")]
        public async Task<IActionResult> Details(int? mahh, string tenhh, int page=1 )
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            if (mahh == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas.FirstOrDefaultAsync(m => m.MaHH == mahh);

            var loai = await _context.loais.FirstOrDefaultAsync(m => m.MaLoai == hangHoa.MaLoai);
            var loai2 =  _context.HangHoas
               .Where(m => m.MaLoai == hangHoa.MaLoai).AsNoTracking().OrderByDescending(p => p.NgayDang);
            var loai2s = await PagingList.CreateAsync(loai2, 6, page);
            ViewBag.loai2s = loai2s ;
            var loai3 = _context.HangHoas
               .Where(m => m.MaLoai == hangHoa.MaLoai).AsNoTracking().OrderBy(p => p.NgayDang);
            var loai3s = await PagingList.CreateAsync(loai3, 5, page);
            ViewBag.loai3s = loai3s;
            var loai4 = _context.BaiViet
              .Where(m => m.MaLoai == hangHoa.MaLoai).AsNoTracking().OrderBy(p => p.ID);
            var loai4s = await PagingList.CreateAsync(loai4, 3, page);
            ViewBag.loai4s = loai4s;

            if (hangHoa == null)
            {
                return NotFound();
            }

            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(hangHoa.TenHH);

            if (!string.Equals(friendlyTitle, tenhh, StringComparison.Ordinal))
            {
                return RedirectToRoutePermanent("showspdetail", new { mahh, tenhh = friendlyTitle});
            }

            return View(hangHoa);
        }
        

        // POST: Contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("ID,Name,Email,Phone,NoiDung")] Contact contact)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        //[Route("bai-viet/{id2}/{id}")]
        public async Task<IActionResult> XemBaiViet(int? id, string tieude,int page=1)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            if (id == null)
            {
                return NotFound();
            }


            var baiviet = await _context.BaiViet.FirstOrDefaultAsync(m => m.ID == id);

            if (baiviet == null)
            {
                return NotFound();
            }


            var loai3 = _context.HangHoas
             .Where(m => m.MaLoai == baiviet.MaLoai).AsNoTracking().OrderBy(p => p.NgayDang);
            var loai3s = await PagingList.CreateAsync(loai3, 5, page);
            ViewBag.loai3s = loai3s;


            var loai4 = _context.BaiViet
              .Where(m => m.MaLoai == baiviet.MaLoai).AsNoTracking().OrderBy(p => p.ID);
            var loai4s = await PagingList.CreateAsync(loai4, 3, page);
            ViewBag.loai4s = loai4s;

            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(baiviet.TieuDe);

            if (!string.Equals(friendlyTitle, tieude, StringComparison.Ordinal))
            {
                return RedirectToRoutePermanent("xembaiviet", new { id, tieude = friendlyTitle });
            }


            return View(baiviet);
        }

        public IActionResult Search(string query = "")
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            var data = _context.HangHoas.Where(p => p.TenHH.Contains(query))
          .Include(p => p.Loai)
            .ToList();
            return View(data);
        }

        public IActionResult JSONSearch(string Name = "", double? From = 0,
        double? To = double.MaxValue)
        {
            var data = _context.HangHoas.Where(p => p.TenHH.Contains(Name) &&
           p.DonGia >= From && p.DonGia <= To)
            .Select(p => new {
                MaHH = p.MaHH,
                TenHH = p.TenHH,
                DonGia = p.DonGia,
                Loai = p.Loai.TenLoai,
                GiamGia = p.GiamGia,
                Hinh = p.Hinh
            
            });
            return Json(data);
        }
        public IActionResult TimKiem()
        {

            var model = _context.loais.ToList();
            ViewBag.model = model;
            return View();
        }
        public List<ProductViewmodels> list(int id)
        {
            var model = from a in _context.HangHoas
                        join b in _context.loais
                        on a.MaLoai equals b.MaLoai
                        where a.MaLoai == id
                        select new ProductViewmodels()
                        {
                            MaHH = a.MaHH,
                            TenHH = a.TenHH,
                            Hinh = a.Hinh,
                            NgayDang = a.NgayDang,
                            MaLoai = b.MaLoai,
                            SoLuong = a.SoLuong,
                            DonGia = a.DonGia,
                        };
            return model.ToList();
                        
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {

            var model = _context.loais.ToList();
            ViewBag.model = model;
            ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"].ToString();
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Taikhoan()
        {
            return View(await _context.TaiKhoans.ToListAsync());
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var urlReturn = HttpContext.Request.Query["ReturnUrl"].ToString();
            ViewBag.ReturnUrl = urlReturn;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(urlReturn))
                    {
                        return Redirect(urlReturn);
                    }
                    else
                    {
                        return RedirectToAction("Index", "TrangChus");
                    }
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode));
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is invalid");
                }
            }
            return View(loginModel);
 
        }//end Login POST

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Create()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            return View();
        }

        // POST: TaiKhoans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,Password,PhoneNumber,Enable2FA")] User user)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            if (ModelState.IsValid)
            {


                var _user = new AppUser { UserName = user.UserName, Email = user.Email, Password = user.Password };
                var result = await _userManager.CreateAsync(_user, user.Password);

                if (result.Succeeded)
                {


                    await _signInManager.SignInAsync(_user, isPersistent: false);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }

            }
            
            return View(user);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(string provider = "Phone")
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, provider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound();
            }

            var message = "Your security code is: " + code;

            await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);


            return RedirectToAction(nameof(VerifyCode));
        }

        
        public ActionResult VerifyCode()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            var modelLoai = _context.loais.ToList();
            ViewBag.model = modelLoai;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync("Phone", model.Code, isPersistent:false, rememberClient:false);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "User account locked out.");
                return View(model.Code);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model.Code);
            }
        }


    }
}