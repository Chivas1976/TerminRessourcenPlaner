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
    public class RessourcenController : Controller
    {
        private readonly AppDbContext _context;

        public RessourcenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ressourcen
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ressourcen.ToListAsync());
        }

        // GET: Ressourcen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ressource = await _context.Ressourcen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ressource == null)
            {
                return NotFound();
            }

            return View(ressource);
        }

        // GET: Ressourcen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ressourcen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Typ")] Ressource ressource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ressource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ressource);
        }

        // GET: Ressourcen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ressource = await _context.Ressourcen.FindAsync(id);
            if (ressource == null)
            {
                return NotFound();
            }
            return View(ressource);
        }

        // POST: Ressourcen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Typ")] Ressource ressource)
        {
            if (id != ressource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ressource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RessourceExists(ressource.Id))
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
            return View(ressource);
        }

        // GET: Ressourcen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ressource = await _context.Ressourcen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ressource == null)
            {
                return NotFound();
            }

            return View(ressource);
        }

        // POST: Ressourcen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ressource = await _context.Ressourcen.FindAsync(id);
            if (ressource != null)
            {
                _context.Ressourcen.Remove(ressource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RessourceExists(int id)
        {
            return _context.Ressourcen.Any(e => e.Id == id);
        }
    }
}
