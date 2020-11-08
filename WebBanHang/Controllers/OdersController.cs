using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

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
        public async Task<IActionResult> Create([Bind("ID,Status,CustomerID,ShipName,ShipMobile,ShipAddress,ShipEmail,CreatedDate")] Oder oder)
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Status,CustomerID,ShipName,ShipMobile,ShipAddress,ShipEmail,CreatedDate")] Oder oder)
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

            return View(oder);
        }

        // POST: Oders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oder = await _context.Oders.FindAsync(id);
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
