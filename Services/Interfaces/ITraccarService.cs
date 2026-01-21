using Trasformazioni.Models.Entities;

namespace Trasformazioni.Services.Interfaces
{
    public interface ITraccarService
    {
        Task<List<TraccarDevice>> GetDevicesAsync();
        Task<TraccarDevice> GetDeviceByIdAsync(int deviceId);
        Task<TraccarDevice> GetDeviceByImeiAsync(string imei);
        Task<List<TraccarPosition>> GetPositionsAsync();
        Task<TraccarPosition> GetLatestPositionAsync(int deviceId);
        Task<List<TraccarPosition>> GetPositionHistoryAsync(int deviceId, DateTime from, DateTime to);
        Task<List<TraccarTrip>> GetTripsAsync(int deviceId, DateTime from, DateTime to);
        Task<List<TraccarEvent>> GetEventsAsync(int deviceId, DateTime from, DateTime to);
    }
}
