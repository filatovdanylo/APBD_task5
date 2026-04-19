using System.ComponentModel.DataAnnotations;

namespace reservations_api.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        [Required]
        [MinLength(1)]
        public string OrganizerName { get; set; } = string.Empty;
        [Required]
        [MinLength(1)]
        public string Topic { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = new DateOnly();
        public TimeOnly StartTime { get; set; } = new TimeOnly();
        public TimeOnly EndTime { get; set;} = new TimeOnly();
        public string Status { get; set; } = string.Empty;

    }
}
