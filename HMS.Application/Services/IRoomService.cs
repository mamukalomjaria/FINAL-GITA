using HMS.Application.DTOs;

public interface IRoomService
{
    Task<List<RoomDto>> GetRooms(Guid hotelId, double? minPrice, double? maxPrice);
    Task<RoomDto?> GetRoom(Guid hotelId, Guid roomId);
    Task<RoomDto> CreateRoom(Guid hotelId, CreateRoomDto dto);
    Task<bool> UpdateRoom(Guid hotelId, Guid roomId, UpdateRoomDto dto);
    Task<bool> DeleteRoom(Guid hotelId, Guid roomId);
}