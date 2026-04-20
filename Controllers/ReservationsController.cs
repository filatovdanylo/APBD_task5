using Microsoft.AspNetCore.Mvc;
using reservations_api.Database;
using reservations_api.Models;

namespace reservations_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {

        [HttpGet]
        public ActionResult<List<Reservation>> GetAllWithOptionalParameters
            (
                [FromQuery] DateOnly? date,
                [FromQuery] string? status,
                [FromQuery] int? roomId,
                [FromQuery] string? topic
            )
        {
            var reservations = DataStorage.Reservations.AsEnumerable();

            if (date.HasValue)
            {
                reservations = reservations.Where(r => r.Date == date).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations
                    .Where(r => r.Status == status).ToList();
            }
            if (roomId.HasValue)
            {
                reservations = reservations
                    .Where(r => r.RoomId == roomId).ToList();
            }
            if (!string.IsNullOrEmpty(topic))
            {
                reservations = reservations
                    .Where(r => r.Topic == topic).ToList();
            }

            if (!reservations.Any())
            {
                return NotFound("Reservations with such filter parameters do not exist");
            }

            return Ok(reservations);
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = DataStorage.Reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} does not exist");
            }

            return Ok(reservation);
        }

        [HttpPost]
        public ActionResult<Reservation> CreateReservation(Reservation reservation)
        {
            var error = Validate(reservation);

            if (error != null)
            {
                return error;
            }

            int newId = DataStorage.NextReservationId;

            reservation.Id = newId;

            DataStorage.Reservations.Add(reservation);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Reservation> UpdateReservation(int id, Reservation reservation)
        {

            if (id != reservation.Id)
            {
                return BadRequest("Id in URL does not match the id in the request body");
            }

            var databaseReservation = DataStorage.Reservations.FirstOrDefault(r => r.Id == id);

            if (databaseReservation == null)
            {
                return NotFound($"Reservation with id {id} does not exist");
            }

            var error = Validate(reservation);

            if (error != null)
            {
                return error;
            }
                

            databaseReservation.RoomId = reservation.RoomId;
            databaseReservation.OrganizerName = reservation.OrganizerName;
            databaseReservation.Topic = reservation.Topic;
            databaseReservation.Date = reservation.Date;
            databaseReservation.StartTime = reservation.StartTime;
            databaseReservation.EndTime = reservation.EndTime;
            databaseReservation.Status = reservation.Status;

            return Ok(databaseReservation);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var reservation = DataStorage.Reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound($"Reservation with id {id} does not exist");
            }

            DataStorage.Reservations.Remove(reservation);

            return NoContent();
        }

        private ActionResult? Validate(Reservation reservation)
        {
            int roomId = reservation.RoomId;
            var room = DataStorage.Rooms.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
                return NotFound($"Room with id {roomId} does not exist. Cannot reserve it");

            if (reservation.StartTime > reservation.EndTime)
                return BadRequest("Starting time of reservation should be smaller than end time");

            if (!room.IsActive)
                return BadRequest($"Cannot reserve inactive room with id {roomId}");

            bool isOccupied = DataStorage.Reservations
                .Exists(r => r.Id != reservation.Id && r.RoomId == roomId 
                && r.Date == reservation.Date
                && r.EndTime > reservation.StartTime && r.StartTime < reservation.EndTime
                && !r.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase));

            if (isOccupied)
                return Conflict($"Two reservations for the same room with id {roomId} cannot overlap on the same day");

            return null;
        }
    }
}
