﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
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
        public async Task<IActionResult> Index(int page = 1)
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            var query = _context.Oders.AsNoTracking().OrderBy(p => p.ID);
            var model = await PagingList.CreateAsync(query, 10, page);
            return View(model);
        }

        // GET: Oders/Details/5
        public async Task<IActionResult> Details(long? id)
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

            var subOderDetail = await _context.OderDetails.Where(m => m.OderID == oder.ID).ToListAsync();

            if (subOderDetail != null)
            {

                var oderDetail = (from A in _context.HangHoas
                                  join B in subOderDetail on A.MaHH equals B.MaHH
                                  join C in _context.loais on A.MaLoai equals C.MaLoai
                                  select new OrderDetailViewModel
                                  {
                                      ID = B.ID,
                                      TenHH = A.TenHH,
                                      Loai = C.TenLoai,
                                      Quantity = B.Quantity,
                                      GiamGia = A.GiamGia,
                                      Gia = B.Gia,
                                      ThanhTien = Math.Round((B.Gia - B.Gia * A.GiamGia / 100) * B.Quantity, 0)
                                  }
                                     );


                ViewBag.oderDetail = await oderDetail.ToListAsync();
            }

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
                oder.ID = DateTime.Now.Ticks;
                _context.Add(oder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oder);
        }

        // GET: Oders/Edit/5
        public async Task<IActionResult> Edit(long? id)
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
        public async Task<IActionResult> Edit(long id, [Bind("ID,Status,CustomerID,ShipName,ShipMobile,ShipAddress,ShipEmail,CreatedDate,CheckOutType,Total")] Oder oder)
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
        public async Task<IActionResult> Delete(long? id)
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
                                      GiamGia = A.GiamGia,
                                      ThanhTien = (B.Gia - B.Gia * A.GiamGia / 100) * B.Quantity
                                  }
                                 );

            ViewBag.oderDetail = await subOrderDetail.ToListAsync();
            return View(oder);
        }

        // POST: Oders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
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

        private bool OderExists(long id)
        {
            return _context.Oders.Any(e => e.ID == id);
        }
    }
}
