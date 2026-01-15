namespace TerminRessourcenPlaner.Models
{
    public class TerminRessource
    {
        public int TerminId { get; set; }
        public Termin Termin { get; set; } = null!;

        public int RessourceId { get; set; }
        public Ressource Ressource { get; set; } = null!;
    }
}