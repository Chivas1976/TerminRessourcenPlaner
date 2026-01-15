using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TerminRessourcenPlaner.Data;
using TerminRessourcenPlaner.Models;

namespace TerminRessourcenPlaner.Controllers
{
    public class DienstleistungsController : Controller
    {
        private readonly AppDbContext _context;

        public DienstleistungsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dienstleistungs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dienstleistung.ToListAsync());
        }

        // GET: Dienstleistungs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dienstleistung = await _context.Dienstleistung
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dienstleistung == null)
            {
                return NotFound();
            }

            return View(dienstleistung);
        }

        // GET: Dienstleistungs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dienstleistungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bezeichnung,DauerInMinuten,PreisInEuro,Kategorie")] Dienstleistung dienstleistung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dienstleistung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dienstleistung);
        }

        // GET: Dienstleistungs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dienstleistung = await _context.Dienstleistung.FindAsync(id);
            if (dienstleistung == null)
            {
                return NotFound();
            }
            return View(dienstleistung);
        }

        // POST: Dienstleistungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Bezeichnung,DauerInMinuten,PreisInEuro,Kategorie")] Dienstleistung dienstleistung)
        {
            if (id != dienstleistung.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dienstleistung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DienstleistungExists(dienstleistung.Id))
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
            return View(dienstleistung);
        }

        // GET: Dienstleistungs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dienstleistung = await _context.Dienstleistung
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dienstleistung == null)
            {
                return NotFound();
            }

            return View(dienstleistung);
        }

        // POST: Dienstleistungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dienstleistung = await _context.Dienstleistung.FindAsync(id);
            if (dienstleistung != null)
            {
                _context.Dienstleistung.Remove(dienstleistung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DienstleistungExists(int id)
        {
            return _context.Dienstleistung.Any(e => e.Id == id);
        }
    }
}
