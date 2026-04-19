using System.ComponentModel.DataAnnotations;

namespace reservations_api.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MinLength(1)]
        public string BuildingCode {  get; set; } = string.Empty;
        public int Floor { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Capacity must be a positive integer.")]
        public int Capacity { get; set; }
        public bool HasProjector { get; set; }
        public bool IsActive { get; set; }
    }
}
