using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TerminRessourcenPlaner.Data;
using TerminRessourcenPlaner.Models;
using TerminRessourcenPlaner.ViewModels;

namespace TerminRessourcenPlaner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        private const string DashboardRessourcenTyp = "Stuhl";

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // Startseite (Landing Page)
        public IActionResult Index()
        {
            return View();
        }

        // Tagesübersicht / Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var heute = DateTime.Today;
            var morgen = heute.AddDays(1);
            var jetzt = DateTime.Now;

            var tagesStart = heute.AddHours(8);
            var tagesEnde = heute.AddHours(18);

            var ressourcenQuery = _db.Ressourcen.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(DashboardRessourcenTyp))
                ressourcenQuery = ressourcenQuery.Where(r => r.Typ == DashboardRessourcenTyp);

            var ressourcen = await ressourcenQuery.OrderBy(r => r.Name).ToListAsync();

            var termineHeute = await _db.Termine
                .AsNoTracking()
                .Where(t => t.Start >= heute && t.Start < morgen)
                .Include(t => t.TerminRessourcen).ThenInclude(tr => tr.Ressource)
                .OrderBy(t => t.Start)
                .ToListAsync();

            if (termineHeute.Any())
            {
                var minStart = termineHeute.Min(t => t.Start);
                var maxEnde = termineHeute.Max(t => t.Ende);
                if (minStart < tagesStart) tagesStart = minStart;
                if (maxEnde > tagesEnde) tagesEnde = maxEnde;
            }

            var naechsterTerminEntity = await _db.Termine
                .AsNoTracking()
                .Where(t => t.Start >= jetzt)
                .Include(t => t.TerminRessourcen).ThenInclude(tr => tr.Ressource)
                .OrderBy(t => t.Start)
                .FirstOrDefaultAsync();

            var belegtJetztRessourcenIds = termineHeute
                .Where(t => t.Start <= jetzt && t.Ende > jetzt)
                .SelectMany(t => t.TerminRessourcen.Select(tr => tr.RessourceId))
                .Distinct()
                .ToHashSet();

            var vm = new HomeDashboardViewModel
            {
                Datum = heute,
                TagesStart = tagesStart,
                TagesEnde = tagesEnde,

                RessourcenGesamtAnzahl = ressourcen.Count,
                TermineHeuteAnzahl = termineHeute.Count,

                RessourcenBelegtJetztAnzahl = belegtJetztRessourcenIds.Count,
                RessourcenFreiJetztAnzahl = Math.Max(0, ressourcen.Count - belegtJetztRessourcenIds.Count),

                NaechsterTermin = naechsterTerminEntity == null ? null : new TerminKurzInfo
                {
                    TerminId = naechsterTerminEntity.Id,
                    Titel = naechsterTerminEntity.Titel,
                    Start = naechsterTerminEntity.Start,
                    Ende = naechsterTerminEntity.Ende,
                    ZugewieseneRessourcenAnzahl = naechsterTerminEntity.TerminRessourcen.Count,
                    RessourcenNamen = naechsterTerminEntity.TerminRessourcen.Select(tr => tr.Ressource.Name).OrderBy(n => n).ToList()
                },

                HeutigeTermine = termineHeute.Select(t => new TerminKurzInfo
                {
                    TerminId = t.Id,
                    Titel = t.Titel,
                    Start = t.Start,
                    Ende = t.Ende,
                    ZugewieseneRessourcenAnzahl = t.TerminRessourcen.Count,
                    RessourcenNamen = t.TerminRessourcen.Select(tr => tr.Ressource.Name).OrderBy(n => n).ToList()
                }).ToList()
            };

            foreach (var r in ressourcen)
            {
                var buchungenHeute = termineHeute
                    .Where(t => t.TerminRessourcen.Any(tr => tr.RessourceId == r.Id))
                    .OrderBy(t => t.Start)
                    .Select(t => new TerminBlockInfo { TerminId = t.Id, Titel = t.Titel, Start = t.Start, Ende = t.Ende })
                    .ToList();

                var laufenderTermin = termineHeute
                    .Where(t => t.Start <= jetzt && t.Ende > jetzt)
                    .FirstOrDefault(t => t.TerminRessourcen.Any(tr => tr.RessourceId == r.Id));

                vm.RessourcenStatusHeute.Add(laufenderTermin == null
                    ? new RessourceStatusInfo { RessourceId = r.Id, Name = r.Name, Typ = r.Typ, IstGeradeBelegt = false, BuchungenHeute = buchungenHeute }
                    : new RessourceStatusInfo { RessourceId = r.Id, Name = r.Name, Typ = r.Typ, IstGeradeBelegt = true, BelegtVon = laufenderTermin.Start, BelegtBis = laufenderTermin.Ende, TerminTitel = laufenderTermin.Titel, TerminId = laufenderTermin.Id, BuchungenHeute = buchungenHeute });
            }

            return View(vm); // ✅ Suchen Views/Home/Dashboard.cshtml
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}