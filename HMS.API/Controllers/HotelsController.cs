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

        public HotelsController(
            IHotelService hotelService,
            IRoomService roomService,
            IReservationService reservationService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
            _reservationService = reservationService;
        }

        // ===============================
        // HOTELS
        // ===============================

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
            return Ok(hotel);
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

            return Ok("Hotel deleted");
        }

        // ===============================
        // ROOMS
        // ===============================

        [HttpGet("{hotelId}/rooms")]
        public async Task<IActionResult> GetRooms(Guid hotelId, double? minPrice, double? maxPrice)
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
            return Ok(room);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> UpdateRoom(Guid hotelId, Guid roomId, UpdateRoomDto dto)
        {
            var result = await _roomService.UpdateRoom(hotelId, roomId, dto);

            if (!result)
                return NotFound();

            return Ok();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> DeleteRoom(Guid hotelId, Guid roomId)
        {
            var result = await _roomService.DeleteRoom(hotelId, roomId);

            if (!result)
                return BadRequest("Room has active reservations");

            return Ok("Room deleted");
        }

        // ===============================
        // RESERVATIONS
        // ===============================

        [Authorize(Roles = "Guest")]
        [HttpPost("{hotelId}/reservations")]
        public async Task<IActionResult> CreateReservation(Guid hotelId, CreateReservationDto dto)
        {
            var reservation = await _reservationService.CreateReservation(hotelId, dto);
            return Ok(reservation);
        }

        [HttpPut("reservations/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(Guid reservationId, UpdateReservationDto dto)
        {
            var result = await _reservationService.UpdateReservation(reservationId, dto);

            if (!result)
                return BadRequest();

            return Ok();
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
    }
}