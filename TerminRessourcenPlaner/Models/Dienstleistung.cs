using System.ComponentModel.DataAnnotations;

namespace TerminRessourcenPlaner.Models
{
    public class Dienstleistung
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Bezeichnung { get; set; } = string.Empty;

        [Required]
        [Range(5, 600)]
        public int DauerInMinuten { get; set; }

        // Optional, aber professionell (kannst du später nutzen)
        [Range(0, 9999)]
        public decimal? PreisInEuro { get; set; }

        // Optional: z.B. "Herren", "Damen", "Bart", "Waschen"
        [StringLength(50)]
        public string? Kategorie { get; set; }
    }
}