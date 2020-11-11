using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OdersController : Controller
    {
        private readonly MyDBContext _context;
        private readonly string admin = "Admin";

        public OdersController(MyDBContext context)
        {
            _context = context;
        }

        // GET: Oders
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View(await _context.Oders.ToListAsync());
        }

        // GET: Oders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oder = await _context.Oders
                .FirstOrDefaultAsync(m => m.ID == id);

            if (oder == null)
            {
                return NotFound();
            }

            var oderDetail = await _context.OderDetails.Where(m => m.OderID == oder.ID).ToListAsync();

            if(oderDetail == null)
            {
                return NotFound();
            }

            var subOrderDetail = (from A in _context.HangHoas
                                      join B in oderDetail on A.MaHH equals B.MaHH
                                      join C in _context.loais on A.MaLoai equals C.MaLoai
                                      select new OrderDetailViewModel
                                      {
                                          TenHH = A.TenHH,
                                          Loai = C.TenLoai,
                                          Quantity = B.Quantity,
                                          Gia = B.Gia,
                                      }   
                                 );

            ViewBag.oderDetail = await subOrderDetail.ToListAsync();

            return View(oder);
        }

        // GET: Oders/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View();
        }

        // POST: Oders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Status,CustomerID,ShipName,ShipMobile,ShipAddress,ShipEmail,CreatedDate,CheckOutType,Total")] Oder oder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oder);
        }

        // GET: Oders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oder = await _context.Oders.FindAsync(id);
            if (oder == null)
            {
                return NotFound();
            }
            return View(oder);
        }

        // POST: Oders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Status,CustomerID,ShipName,ShipMobile,ShipAddress,ShipEmail,CreatedDate,CheckOutType,Total")] Oder oder)
        {
            if (id != oder.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OderExists(oder.ID))
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
            return View(oder);
        }

        // GET: Oders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oder = await _context.Oders
                .FirstOrDefaultAsync(m => m.ID == id);

            if (oder == null)
            {
                return NotFound();
            }

            var oderDetail = await _context.OderDetails.Where(m => m.OderID == oder.ID).ToListAsync();

            if (oderDetail == null)
            {
                return NotFound();
            }

            var subOrderDetail = (from A in _context.HangHoas
                                  join B in oderDetail on A.MaHH equals B.MaHH
                                  join C in _context.loais on A.MaLoai equals C.MaLoai
                                  select new OrderDetailViewModel
                                  {
                                      TenHH = A.TenHH,
                                      Loai = C.TenLoai,
                                      Quantity = B.Quantity,
                                      Gia = B.Gia,
                                  }
                                 );

            ViewBag.oderDetail = await subOrderDetail.ToListAsync();
            return View(oder);
        }

        // POST: Oders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oder = await _context.Oders.FindAsync(id);
            var oderDetail = await _context.OderDetails.Where(m => m.OderID == oder.ID).ToListAsync();

            if(oderDetail != null)
            {
                foreach (var item in oderDetail)
                {
                    _context.OderDetails.Remove(item);
                }
            }

            _context.Oders.Remove(oder);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OderExists(int id)
        {
            return _context.Oders.Any(e => e.ID == id);
        }
    }
}
