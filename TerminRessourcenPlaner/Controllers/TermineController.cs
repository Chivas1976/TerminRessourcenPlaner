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
    public class TermineController : Controller
    {
        private readonly AppDbContext _context;

        public TermineController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Termine
        public async Task<IActionResult> Index()
        {
            var termine = await _context.Termine
                .AsNoTracking()
                .Include(t => t.Dienstleistung)
                .Include(t => t.TerminRessourcen)
                    .ThenInclude(tr => tr.Ressource)
                .OrderBy(t => t.Start)
                .ToListAsync();

            return View(termine);
        }

        // GET: Termine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var termin = await _context.Termine
                .AsNoTracking()
                .Include(t => t.Dienstleistung)
                .Include(t => t.TerminRessourcen)
                    .ThenInclude(tr => tr.Ressource)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (termin == null) return NotFound();

            return View(termin);
        }

        // GET: Termine/Create
        public IActionResult Create()
        {
            // Ressourcen (Checkboxen) – aktuell: Mitarbeiter
            ViewBag.Ressourcen = _context.Ressourcen
                .Where(r => r.Typ == "Mitarbeiter")
                .OrderBy(r => r.Name)
                .ToList();

            // Dienstleistung (Dropdown)
            ViewData["DienstleistungId"] = new SelectList(
                _context.Dienstleistungen.OrderBy(d => d.Bezeichnung),
                "Id",
                "Bezeichnung"
            );
            ViewBag.DienstleistungenAnzahl = _context.Dienstleistungen.Count();

            return View();
        }

        // POST: Termine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Titel,Kundenname,DienstleistungId,Start,Beschreibung")] Termin termin,
            int[] selectedRessourcen)
        {
            // 1) Dienstleistung laden und Ende automatisch berechnen
            var dienstleistung = await _context.Dienstleistungen
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == termin.DienstleistungId);

            if (dienstleistung == null)
            {
                ModelState.AddModelError("DienstleistungId", "Bitte wählen Sie eine gültige Dienstleistung aus.");
            }
            else
            {
                termin.Ende = termin.Start.AddMinutes(dienstleistung.DauerInMinuten);
            }

            // 2) Minimale Plausibilitätsprüfung
            if (dienstleistung != null && termin.Start >= termin.Ende)
            {
                ModelState.AddModelError(string.Empty, "Der Terminzeitraum ist ungültig.");
            }

            // 3) Konfliktprüfung (Doppelbelegung) – bevor gespeichert wird
            if (ModelState.IsValid && selectedRessourcen != null && selectedRessourcen.Any())
            {
                var konfliktVorhanden = await _context.TerminRessourcen
                    .Include(tr => tr.Termin)
                    .AnyAsync(tr =>
                        selectedRessourcen.Contains(tr.RessourceId) &&
                        tr.Termin.Start < termin.Ende &&
                        termin.Start < tr.Termin.Ende
                    );

                if (konfliktVorhanden)
                {
                    ModelState.AddModelError(string.Empty,
                        "Konflikt: Mindestens eine ausgewählte Ressource ist in diesem Zeitraum bereits belegt.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(termin);
                await _context.SaveChangesAsync();

                // n:m Verknüpfung speichern
                if (selectedRessourcen != null && selectedRessourcen.Any())
                {
                    foreach (var ressourcenId in selectedRessourcen)
                    {
                        _context.TerminRessourcen.Add(new TerminRessource
                        {
                            TerminId = termin.Id,
                            RessourceId = ressourcenId
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Termin wurde erfolgreich gespeichert.";
                return RedirectToAction(nameof(Index));
            }

            // Bei Fehlern: Dropdown + Ressourcen neu füllen
            ViewBag.Ressourcen = _context.Ressourcen
                .Where(r => r.Typ == "Mitarbeiter")
                .OrderBy(r => r.Name)
                .ToList();

            ViewBag.AusgewaehlteRessourcenIds = selectedRessourcen?.ToList() ?? new List<int>();

            ViewData["DienstleistungId"] = new SelectList(
                _context.Dienstleistungen.OrderBy(d => d.Bezeichnung),
                "Id",
                "Bezeichnung",
                termin.DienstleistungId
            );

            return View(termin);
        }

        // GET: Termine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var termin = await _context.Termine
                .Include(t => t.TerminRessourcen)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (termin == null) return NotFound();

            ViewBag.Ressourcen = await _context.Ressourcen
                .Where(r => r.Typ == "Mitarbeiter")
                .OrderBy(r => r.Name)
                .ToListAsync();

            ViewBag.AusgewaehlteRessourcenIds = termin.TerminRessourcen
                .Select(tr => tr.RessourceId)
                .ToList();

            ViewData["DienstleistungId"] = new SelectList(
                _context.Dienstleistungen.OrderBy(d => d.Bezeichnung),
                "Id",
                "Bezeichnung",
                termin.DienstleistungId
            );

            return View(termin);
        }

        // POST: Termine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Titel,Kundenname,DienstleistungId,Start,Beschreibung")] Termin termin,
            int[] selectedRessourcen)
        {
            if (id != termin.Id)
                return NotFound();

            // Bestehenden Termin inkl. Zuordnungen laden (wichtig für Update)
            var terminDb = await _context.Termine
                .Include(t => t.TerminRessourcen)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (terminDb == null)
                return NotFound();

            // Dienstleistung laden und Ende automatisch berechnen
            var dienstleistung = await _context.Dienstleistungen
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == termin.DienstleistungId);

            if (dienstleistung == null)
            {
                ModelState.AddModelError("DienstleistungId", "Bitte wählen Sie eine gültige Dienstleistung aus.");
            }
            else
            {
                termin.Ende = termin.Start.AddMinutes(dienstleistung.DauerInMinuten);
            }

            // Plausibilitätsprüfung
            if (dienstleistung != null && termin.Start >= termin.Ende)
            {
                ModelState.AddModelError(string.Empty, "Der Terminzeitraum ist ungültig.");
            }

            // Konfliktprüfung (Doppelbelegung) – aktueller Termin wird ausgeschlossen
            if (ModelState.IsValid && selectedRessourcen != null && selectedRessourcen.Any())
            {
                var konfliktVorhanden = await _context.TerminRessourcen
                    .Include(tr => tr.Termin)
                    .AnyAsync(tr =>
                        tr.TerminId != termin.Id &&
                        selectedRessourcen.Contains(tr.RessourceId) &&
                        tr.Termin.Start < termin.Ende &&
                        termin.Start < tr.Termin.Ende
                    );

                if (konfliktVorhanden)
                {
                    ModelState.AddModelError(string.Empty,
                        "Konflikt: Mindestens eine ausgewählte Ressource ist in diesem Zeitraum bereits belegt.");
                }
            }

            // Wenn Fehler: View neu befüllen und zurück
            if (!ModelState.IsValid)
            {
                ViewBag.Ressourcen = await _context.Ressourcen
                    .Where(r => r.Typ == "Mitarbeiter")
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                ViewBag.AusgewaehlteRessourcenIds = selectedRessourcen?.ToList() ?? new List<int>();

                ViewData["DienstleistungId"] = new SelectList(
                    _context.Dienstleistungen.OrderBy(d => d.Bezeichnung),
                    "Id",
                    "Bezeichnung",
                    termin.DienstleistungId
                );

                return View(termin);
            }

            // TerminDB aktualisieren (sauber, ohne Nebenwirkungen)
            terminDb.Titel = termin.Titel;
            terminDb.Kundenname = termin.Kundenname;
            terminDb.DienstleistungId = termin.DienstleistungId;
            terminDb.Start = termin.Start;
            terminDb.Ende = termin.Ende;
            terminDb.Beschreibung = termin.Beschreibung;

            // Alte Zuordnungen löschen
            _context.TerminRessourcen.RemoveRange(terminDb.TerminRessourcen);

            // Neue Zuordnungen setzen
            if (selectedRessourcen != null && selectedRessourcen.Any())
            {
                foreach (var ressourcenId in selectedRessourcen)
                {
                    _context.TerminRessourcen.Add(new TerminRessource
                    {
                        TerminId = terminDb.Id,
                        RessourceId = ressourcenId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Termin wurde erfolgreich aktualisiert.";
            return RedirectToAction(nameof(Index));
        }
        // GET: Termine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var termin = await _context.Termine
                .AsNoTracking()
                .Include(t => t.TerminRessourcen)
                    .ThenInclude(tr => tr.Ressource)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (termin == null) return NotFound();

            return View(termin);
        }

        // POST: Termine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // n:m Zuordnungen zuerst löschen
            var zuordnungen = _context.TerminRessourcen.Where(tr => tr.TerminId == id);
            _context.TerminRessourcen.RemoveRange(zuordnungen);

            var termin = await _context.Termine.FindAsync(id);
            if (termin != null)
            {
                _context.Termine.Remove(termin);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Termin wurde erfolgreich gelöscht.";
            return RedirectToAction(nameof(Index));
        }
    }
}
