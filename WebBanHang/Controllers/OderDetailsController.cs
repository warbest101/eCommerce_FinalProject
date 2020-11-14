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

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OderDetailsController : Controller
    {
        private readonly MyDBContext _context;
        private readonly string admin = "Admin";

        public OderDetailsController(MyDBContext context)
        {
            _context = context;
        }

        // GET: OderDetails
        public async Task<IActionResult> Index(int page = 1)
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }

            var query = _context.OderDetails.AsNoTracking().OrderBy(p => p.ID);
            var model = await PagingList.CreateAsync(query, 10, page);
            return View(model);
        }

        // GET: OderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oderDetail = await _context.OderDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (oderDetail == null)
            {
                return NotFound();
            }

            return View(oderDetail);
        }

        // GET: OderDetails/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View();
        }

        // POST: OderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OderID,Quantity,Gia,MaHH")] OderDetail oderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(oderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(oderDetail);
        }

        // GET: OderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oderDetail = await _context.OderDetails.FindAsync(id);
            if (oderDetail == null)
            {
                return NotFound();
            }
            return View(oderDetail);
        }

        // POST: OderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OderID,Quantity,Gia,MaHH")] OderDetail oderDetail)
        {
            if (id != oderDetail.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OderDetailExists(oderDetail.OderID))
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
            return View(oderDetail);
        }

        // GET: OderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oderDetail = await _context.OderDetails
                .FirstOrDefaultAsync(m => m.ID == id);
            if (oderDetail == null)
            {
                return NotFound();
            }

            return View(oderDetail);
        }

        // POST: OderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oderDetail = await _context.OderDetails.FindAsync(id);
            _context.OderDetails.Remove(oderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Oders", new { id = oderDetail.OderID });
        }

        private bool OderDetailExists(long id)
        {
            return _context.OderDetails.Any(e => e.OderID == id);
        }
    }
}
