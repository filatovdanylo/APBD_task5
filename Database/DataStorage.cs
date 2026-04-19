using reservations_api.Models;

namespace reservations_api.Database
{
    public static class DataStorage
    {
        public static List<Room> Rooms { get; } = new()
   {
       new Room { Id = 1, Name = "Lecture Hall 101", BuildingCode = "A", Floor = 1, Capacity = 120, HasProjector = true,  IsActive = true },
       new Room { Id = 2, Name = "Lab 202",          BuildingCode = "A", Floor = 2, Capacity = 30,  HasProjector = true,  IsActive = true },
       new Room { Id = 3, Name = "Seminar Room 305", BuildingCode = "B", Floor = 3, Capacity = 20,  HasProjector = false, IsActive = true },
       new Room { Id = 4, Name = "Conference Room 1", BuildingCode = "B", Floor = 1, Capacity = 15, HasProjector = true,  IsActive = false },
       new Room { Id = 5, Name = "Workshop Room 110", BuildingCode = "C", Floor = 1, Capacity = 40, HasProjector = true,  IsActive = true }
   };
        public static List<Reservation> Reservations { get; } = new()
   {
       new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Nowak",      Topic = "Algorithms Lecture",      Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(8, 0),  EndTime = new TimeOnly(10, 0), Status = "confirmed" },
       new Reservation { Id = 2, RoomId = 2, OrganizerName = "Anna Kowalska",  Topic = "C# Workshop",            Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), Status = "planned" },
       new Reservation { Id = 3, RoomId = 3, OrganizerName = "Piotr Wisniewski", Topic = "Project Consultation", Date = new DateOnly(2026, 5, 11), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 30), Status = "confirmed" },
       new Reservation { Id = 4, RoomId = 1, OrganizerName = "Maria Zielinska", Topic = "Data Structures Exam",  Date = new DateOnly(2026, 5, 12), StartTime = new TimeOnly(9, 0),  EndTime = new TimeOnly(11, 0), Status = "planned" },
       new Reservation { Id = 5, RoomId = 5, OrganizerName = "Tomasz Lewandowski", Topic = "Docker Training",    Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(16, 0), Status = "cancelled" }
   };
        public static int NextRoomId => Rooms.Count > 0 ? Rooms.Max(r => r.Id) + 1 : 1;
        public static int NextReservationId => Reservations.Count > 0 ? Reservations.Max(r => r.Id) + 1 : 1;
    }
}
