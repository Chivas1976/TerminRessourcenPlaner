using System.ComponentModel.DataAnnotations;

namespace TerminRessourcenPlaner.Models
{
    public class Termin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titel { get; set; } = string.Empty;

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime Ende { get; set; }

        public string? Beschreibung { get; set; }
    }
}