using System;
using System.Collections.Generic;
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
        public string Kundenname { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Bitte wählen Sie eine Dienstleistung aus.")]
        public int DienstleistungId { get; set; }

        public Dienstleistung? Dienstleistung { get; set; } = null!;

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime Ende { get; set; }

        public string? Beschreibung { get; set; }

        public ICollection<TerminRessource> TerminRessourcen { get; set; } = new List<TerminRessource>();
    }
}