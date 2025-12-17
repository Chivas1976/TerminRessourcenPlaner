using System.ComponentModel.DataAnnotations;

namespace TerminRessourcenPlaner.Models
{
    public class Ressource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Typ { get; set; } = string.Empty; // z.B. Raum oder Person
    }
}