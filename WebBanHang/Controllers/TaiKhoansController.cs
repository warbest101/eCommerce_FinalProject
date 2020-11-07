using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class TaiKhoansController : Controller
    {
        private readonly MyDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly string admin = "admin";

        public TaiKhoansController(
            MyDBContext context, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: TaiKhoans
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            //return View(await _context.TaiKhoans.ToListAsync());
            return View(await _userManager.Users.ToListAsync());
        }

        // GET: TaiKhoans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTK == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
            */

            var result = await _userManager.FindByIdAsync(id);
            User _user = new User { Id = result.Id, UserName = result.UserName, Email = result.Email , Password = result.Password};
            if (_user == null)
            {
                return NotFound();
            }

            return View(_user);
        }

        // GET: TaiKhoans/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View();
        }

        // POST: TaiKhoans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,Password")] User user)
        {

            if (ModelState.IsValid)
            {
                var _user = new AppUser { UserName = user.UserName, Email = user.Email , Password = user.Password};
                var result = await _userManager.CreateAsync(_user, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            
            return View(user);
            /*
            TaiKhoan username = _context.TaiKhoans.SingleOrDefault(p => p.TenDangNhap == taiKhoan.TenDangNhap);
            if (username != null)
            {
                ViewBag.UsernameErr = "Username này đã tồn tại";
            }

            if (ViewBag.UsernameErr == null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(taiKhoan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(taiKhoan);
            */
        }

        // GET: TaiKhoans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
            */
            var result = await _userManager.FindByIdAsync(id);
            var _user = new User { Id = result.Id, UserName = result.UserName, Email = result.Email , Password = result.Password};
            if (result == null)
            {
                return NotFound();
            }


            return View(_user);
        }

        // POST: TaiKhoans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var _user = await _userManager.FindByIdAsync(id);
                
                IdentityResult validUserName = null;
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    UserValidator<AppUser> userValidator = new UserValidator<AppUser>();
                    validUserName = await userValidator.ValidateAsync(_userManager, _user);
                    if (validUserName.Succeeded)
                    {
                        _user.UserName = user.UserName;
                    }
                    else
                    {
                        foreach (IdentityError error in validUserName.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                   
                _user.Email = user.Email;

                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(user.Password))
                {
                    PasswordValidator<AppUser> passwordValidator = new PasswordValidator<AppUser>();
                    validPass = await passwordValidator.ValidateAsync(_userManager, _user, user.Password);
                    if (validPass.Succeeded)
                    {
                        _user.Password = user.Password;
                        PasswordHasher<AppUser> passwordHasher = new PasswordHasher<AppUser>();
                        _user.PasswordHash = passwordHasher.HashPassword(_user, user.Password);
                        
                    }
                    else
                    {
                        foreach (IdentityError error in validPass.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }

                if (validUserName != null && validPass != null && validUserName.Succeeded && validPass.Succeeded)
                {
                    var result = await _userManager.UpdateAsync(_user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }

            }
            return View(user);
            /*
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTK))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
            */

        }

        // GET: TaiKhoans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(m => m.MaTK == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
            */
            var result = await _userManager.FindByIdAsync(id);
            var _user = new User { Id = result.Id, UserName = result.UserName, Email = result.Email , Password = result.Password};
            if (result == null)
            {
                return NotFound();
            }

            return View(_user);
        }

        // POST: TaiKhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            /*
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            */

            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        private bool TaiKhoanExists(string id)
        {
            //return _context.TaiKhoans.Any(e => e.MaTK == id);
            return _userManager.Users.Any(e => e.Id == id);
        }
    }
}
