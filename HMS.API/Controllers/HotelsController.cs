using HMS.Application.DTOs;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IManagerService _managerService;
        private readonly IGuestService _guestService;

        public HotelsController(
            IHotelService hotelService,
            IRoomService roomService,
            IReservationService reservationService,
            IManagerService managerService,
            IGuestService guestService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
            _reservationService = reservationService;
            _managerService = managerService;
            _guestService = guestService;
        }

        // =========================
        // HOTELS
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetHotels([FromQuery] HotelFilterDto filter)
        {
            var hotels = await _hotelService.GetHotels(filter);
            return Ok(hotels);
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotel(Guid hotelId)
        {
            var hotel = await _hotelService.GetHotelById(hotelId);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateHotel(CreateHotelDto dto)
        {
            var hotel = await _hotelService.CreateHotel(dto);

            return CreatedAtAction(nameof(GetHotel),
                new { hotelId = hotel.Id }, hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{hotelId}")]
        public async Task<IActionResult> UpdateHotel(Guid hotelId, UpdateHotelDto dto)
        {
            var hotel = await _hotelService.UpdateHotel(hotelId, dto);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{hotelId}")]
        public async Task<IActionResult> DeleteHotel(Guid hotelId)
        {
            var result = await _hotelService.DeleteHotel(hotelId);

            if (!result)
                return BadRequest("Hotel cannot be deleted");

            return NoContent();
        }

        // =========================
        // ROOMS
        // =========================

        [HttpGet("{hotelId}/rooms")]
        public async Task<IActionResult> GetRooms(Guid hotelId, [FromQuery] double? minPrice, [FromQuery] double? maxPrice)
        {
            var rooms = await _roomService.GetRooms(hotelId, minPrice, maxPrice);
            return Ok(rooms);
        }

        [HttpGet("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> GetRoom(Guid hotelId, Guid roomId)
        {
            var room = await _roomService.GetRoom(hotelId, roomId);

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{hotelId}/rooms")]
        public async Task<IActionResult> CreateRoom(Guid hotelId, CreateRoomDto dto)
        {
            var room = await _roomService.CreateRoom(hotelId, dto);

            return CreatedAtAction(nameof(GetRoom),
                new { hotelId, roomId = room.Id }, room);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> UpdateRoom(Guid hotelId, Guid roomId, UpdateRoomDto dto)
        {
            var result = await _roomService.UpdateRoom(hotelId, roomId, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> DeleteRoom(Guid hotelId, Guid roomId)
        {
            var result = await _roomService.DeleteRoom(hotelId, roomId);

            if (!result)
                return BadRequest("Room has active reservations");

            return NoContent();
        }

        // =========================
        // RESERVATIONS
        // =========================

        [Authorize(Roles = "Guest")]
        [HttpPost("{hotelId}/reservations")]
        public async Task<IActionResult> CreateReservation(Guid hotelId, CreateReservationDto dto)
        {
            var reservation = await _reservationService.CreateReservation(hotelId, dto);

            return CreatedAtAction(nameof(GetReservations),
                new { hotelId }, reservation);
        }

        [Authorize(Roles = "Guest")]
        [HttpPut("reservations/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(Guid reservationId, UpdateReservationDto dto)
        {
            var result = await _reservationService.UpdateReservation(reservationId, dto);

            if (!result)
                return BadRequest();

            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{hotelId}/reservations")]
        public async Task<IActionResult> GetReservations(Guid hotelId)
        {
            var reservations = await _reservationService.GetReservations(hotelId);
            return Ok(reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("reservations/{reservationId}")]
        public async Task<IActionResult> DeleteReservation(Guid reservationId)
        {
            var result = await _reservationService.DeleteReservation(reservationId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] Guid? hotelId,
            [FromQuery] Guid? guestId,
            [FromQuery] Guid? roomId,
            [FromQuery] DateTime? date)
        {
            var result = await _reservationService
                .SearchReservations(hotelId, guestId, roomId, date);

            return Ok(result);
        }

        // =========================
        // MANAGERS
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpPost("{hotelId}/managers")]
        public async Task<IActionResult> CreateManager(Guid hotelId, CreateManagerDto dto)
        {
            var manager = await _managerService.CreateManager(hotelId, dto);

            return Ok(manager);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("managers/{managerId}")]
        public async Task<IActionResult> UpdateManager(Guid managerId, UpdateManagerDto dto)
        {
            var result = await _managerService.UpdateManager(managerId, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("managers/{managerId}")]
        public async Task<IActionResult> DeleteManager(Guid managerId)
        {
            var result = await _managerService.DeleteManager(managerId);

            if (!result)
                return BadRequest("Cannot delete manager");

            return NoContent();
        }

        // =========================
        // GUESTS
        // =========================

        [HttpPost("guests")]
        public async Task<IActionResult> CreateGuest(CreateGuestDto dto)
        {
            var guest = await _guestService.CreateGuest(dto);
            return Ok(guest);
        }

        [HttpGet("guests/{id}")]
        public async Task<IActionResult> GetGuest(Guid id)
        {
            var guest = await _guestService.GetGuest(id);

            if (guest == null)
                return NotFound();

            return Ok(guest);
        }

        [HttpPut("guests/{id}")]
        public async Task<IActionResult> UpdateGuest(Guid id, UpdateGuestDto dto)
        {
            var result = await _guestService.UpdateGuest(id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("guests/{id}")]
        public async Task<IActionResult> DeleteGuest(Guid id)
        {
            var result = await _guestService.DeleteGuest(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}