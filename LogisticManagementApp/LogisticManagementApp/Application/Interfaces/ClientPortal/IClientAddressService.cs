using LogisticManagementApp.Models.ClientPortal;

namespace LogisticManagementApp.Applicationn.Interfaces.ClientPortal
{
    public interface IClientAddressService
    {
        Task<ClientAddressesViewModel> GetAddressesAsync(string clientUserId);
        Task<ClientAddressFormViewModel?> GetAddressForEditAsync(string clientUserId, Guid addressId);
        Task CreateAddressAsync(string clientUserId, ClientAddressFormViewModel model);
        Task<bool> UpdateAddressAsync(string clientUserId, ClientAddressFormViewModel model);
        Task<bool> DeleteAddressAsync(string clientUserId, Guid addressId);
        Task<bool> SetDefaultAddressAsync(string clientUserId, Guid addressId);
    }
}
