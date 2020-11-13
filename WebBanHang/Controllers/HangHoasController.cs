using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HangHoasController : Controller
    {
        private readonly MyDBContext _context;
        private readonly string admin = "Admin";

        public HangHoasController(MyDBContext context)
        {
            _context = context;
        }

        // GET: HangHoas
        public async Task<IActionResult> Index(int page=1)
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            var query = _context.HangHoas.Include(h => h.Loai).AsNoTracking().OrderBy(p => p.MaHH);
            var model = await PagingList.CreateAsync(query, 10, page);
            return View(model);
        }
       


        // GET: HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.Loai)
                .FirstOrDefaultAsync(m => m.MaHH == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // GET: HangHoas/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            ViewData["MaLoai"] = new SelectList(_context.loais, "MaLoai", "TenLoai");
            return View();
        }

        // POST: HangHoas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHH,TenHH,Hinh,MoTa,DonGia,SoLuong,MaLoai,NgayDang,NoiBat")]  HangHoa hangHoa, IFormFile fHinh)
        {
          
            if (fHinh != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(),
                @"wwwroot\Hinh", fHinh.FileName);
                using (var file = new FileStream(path, FileMode.Create))
                {
                    await fHinh.CopyToAsync(file);
                }
                hangHoa.Hinh = fHinh.FileName;
            
            }
            hangHoa.NgayDang = DateTime.Now;
        
            if (ModelState.IsValid)
            {
               
                _context.Add(hangHoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLoai"] = new SelectList(_context.loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            return View(hangHoa);
        }

        //public IActionResult Edit(int? id)
        //{
        //    HangHoa lo = new HangHoa();
        //    if (id.HasValue && id.Value != 0)
        //    {
        //        lo = _context.HangHoas.SingleOrDefault(p => p.MaHH == id);
        //    }
        //    return PartialView("_Edit", lo);
        //}

       // GET: HangHoas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return NotFound();
            }
            ViewData["MaLoai"] = new SelectList(_context.loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            return View(hangHoa);
        }

        // POST: HangHoas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, HangHoa model,
IFormFile fHinh)
        {
            if (fHinh != null)
            {
                //upload file
                var path = Path.Combine(Directory.GetCurrentDirectory(),
               @"wwwroot\Hinh", fHinh.FileName);
                using (var file = new FileStream(path, FileMode.Create))
                {
                    await fHinh.CopyToAsync(file);
                }
                model.Hinh = fHinh.FileName;
            }
            if (id.HasValue && id.Value > 0)
            {
                _context.Update(model);
            }
            else
            {
                _context.Add(model);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: HangHoas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.Loai)
                .FirstOrDefaultAsync(m => m.MaHH == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // POST: HangHoas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            _context.HangHoas.Remove(hangHoa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHH == id);
        }
        public IActionResult Search()
        {
           
            return View();
        }
        [HttpPost]

        public IActionResult Search(string Keyword = "")
        {
            var data = _context.HangHoas.Where(p => p.TenHH.Contains(Keyword))
          .Include(p => p.Loai)
            .ToList();
            return PartialView("_SearchPartial", data);
        }
        public IActionResult JSONSearch(string Name = "", double? From = 0,
        double? To = double.MaxValue)
        {
            var data = _context.HangHoas.Where(p => p.TenHH.Contains(Name) &&
           p.DonGia >= From && p.DonGia <= To)
            .Select(p => new {
                TenHH = p.TenHH,
                DonGia = p.DonGia,
                Loai = p.Loai.TenLoai
            });
            return Json(data);
        }
        public IActionResult TimKiem()
        {
            return View();
        }
        
    }
}
