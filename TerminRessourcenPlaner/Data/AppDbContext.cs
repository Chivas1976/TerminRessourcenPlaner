using Microsoft.EntityFrameworkCore;
using TerminRessourcenPlaner.Models;

namespace TerminRessourcenPlaner.Data
{
    /// <summary>
    /// Zentrale Datenbank-Kontextklasse der Anwendung.
    /// Sie stellt die Verbindung zwischen den Modellen und der Datenbank her.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tabellen der Datenbank
        public DbSet<Termin> Termine { get; set; }
        public DbSet<Ressource> Ressourcen { get; set; }
        public DbSet<TerminRessource> TerminRessourcen { get; set; }

        /// <summary>
        /// Konfiguration des Datenmodells (Beziehungen, Schlüssel)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Zusammengesetzter Primärschlüssel für die n:m-Verknüpfung
            modelBuilder.Entity<TerminRessource>()
                .HasKey(tr => new { tr.TerminId, tr.RessourceId });

            modelBuilder.Entity<TerminRessource>()
                .HasOne(tr => tr.Termin)
                .WithMany()
                .HasForeignKey(tr => tr.TerminId);

            modelBuilder.Entity<TerminRessource>()
                .HasOne(tr => tr.Ressource)
                .WithMany()
                .HasForeignKey(tr => tr.RessourceId);
        }
    }
}