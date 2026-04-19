using Microsoft.AspNetCore.Mvc;
using reservations_api.Database;
using reservations_api.Models;

namespace reservations_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Room>> GetAll()
        {
            var rooms = DataStorage.Rooms.AsEnumerable();

            return Ok(rooms.ToList());
        }

        [HttpGet]
        public ActionResult<List<Room>> GetAllWithOptionalParameters
            (
                [FromQuery] int? minCapacity,
                [FromQuery] bool? hasProjector,
                [FromQuery] bool? activeOnly
            ) 
        {
            var rooms = DataStorage.Rooms.AsEnumerable();

            if (minCapacity.HasValue)
            {
                rooms = rooms.Where(r => r.Capacity >= minCapacity.Value).ToList();
            }
            if (hasProjector.HasValue)
            {
                rooms = rooms.Where(r => r.HasProjector == hasProjector).ToList();
            }
            if (activeOnly.HasValue)
            {
                rooms = rooms.Where(r => r.IsActive == activeOnly).ToList();
            }

            if (!rooms.Any())
            {
                return NotFound("Rooms with such filter parameters do not exist");
            }

            return Ok(rooms);
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult<Room> GetById(int id)
        {
            var room = DataStorage.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound($"Room with id {id} not found");
            }

            return Ok(room);
        }

        [HttpGet]
        [Route("building/{buildingCode}")]
        public ActionResult<List<Room>> GetRoomsByBuildingCode(string buildingCode)
        {
            var rooms = DataStorage.Rooms
                .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!rooms.Any())
            {
                return NotFound($"No rooms found in the building with code {buildingCode}");
            }

            return Ok(rooms);
        }

        [HttpPost]
        public ActionResult<Room> CreateRoom(Room room)
        {
            room.Id = DataStorage.NextRoomId;
            DataStorage.Rooms.Add(room);

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Room> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest("Id in URL does not match the id in the request body");
            }

            var databaseRoom = GetById(id).Value;

            if (databaseRoom == null)
            {
                return NotFound($"Room with id {id} does not exist");
            }

            databaseRoom.Name = room.Name;
            databaseRoom.BuildingCode = room.BuildingCode;
            databaseRoom.Floor = room.Floor;
            databaseRoom.Capacity = room.Capacity;
            databaseRoom.HasProjector = room.HasProjector;
            databaseRoom.IsActive = room.IsActive;

            return Ok(databaseRoom);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteRoom(int id)
        {
            var room = GetById(id).Value;

            if (room == null)
            {
                return NotFound($"Room with id {id} does not exist");
            }

            bool hasFutureReservations = DataStorage.Reservations
                .Exists(r => r.RoomId == room.Id 
                && r.Date >= DateOnly.FromDateTime(DateTime.Now) 
                && !r.Status.Equals("cancelled"));

            if (hasFutureReservations)
            {
                return Conflict($"Room with id {id} has future reservations. Cannot delete");
            }

            DataStorage.Rooms.Remove(room);

            return NoContent();
        }
    }
}
