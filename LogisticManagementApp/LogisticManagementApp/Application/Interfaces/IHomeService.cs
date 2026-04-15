using LogisticManagementApp.Models.Home;

namespace LogisticManagementApp.Applicationn.Interfaces
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeDataAsync();
        Task<HomeViewModel> TrackShipmentAsync(string? trackingNumber);
    }
}
