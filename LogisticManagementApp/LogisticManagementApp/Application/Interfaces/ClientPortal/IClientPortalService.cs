using LogisticManagementApp.Models.ClientPortal;

namespace LogisticManagementApp.Applicationn.Interfaces.ClientPortal
{
    public interface IClientPortalService
    {
        Task<ClientDashboardViewModel> GetDashboardAsync(string userId);
        Task<ClientOrdersViewModel> GetOrdersAsync(string clientUserId);
        Task<ClientShipmentsViewModel> GetShipmentsAsync(string userId);
        Task<ClientAddressesViewModel> GetAddressesAsync(string userId);

        Task<ClientShipmentTrackingViewModel?> GetShipmentTrackingAsync(string clientUserId, Guid shipmentId);
    }
}
