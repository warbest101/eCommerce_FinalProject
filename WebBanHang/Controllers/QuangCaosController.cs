using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class QuangCaosController : Controller
    {
        private readonly MyDBContext _context;
        private readonly string admin = "admin";

        public QuangCaosController(MyDBContext context)
        {
            _context = context;
        }

        // GET: QuangCaos
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View(await _context.QuangCao.ToListAsync());
        }

        // GET: QuangCaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quangCao = await _context.QuangCao
                .FirstOrDefaultAsync(m => m.ID == id);
            if (quangCao == null)
            {
                return NotFound();
            }

            return View(quangCao);
        }

        // GET: QuangCaos/Create
        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            return View();
        }

        // POST: QuangCaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Hinh,MaHH")] QuangCao quangCao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quangCao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quangCao);
        }

        // GET: QuangCaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quangCao = await _context.QuangCao.FindAsync(id);
            if (quangCao == null)
            {
                return NotFound();
            }
            return View(quangCao);
        }

        // POST: QuangCaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Hinh,MaHH")] QuangCao quangCao)
        {
            if (id != quangCao.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quangCao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuangCaoExists(quangCao.ID))
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
            return View(quangCao);
        }

        // GET: QuangCaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quangCao = await _context.QuangCao
                .FirstOrDefaultAsync(m => m.ID == id);
            if (quangCao == null)
            {
                return NotFound();
            }

            return View(quangCao);
        }

        // POST: QuangCaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quangCao = await _context.QuangCao.FindAsync(id);
            _context.QuangCao.Remove(quangCao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuangCaoExists(int id)
        {
            return _context.QuangCao.Any(e => e.ID == id);
        }
    }
}
