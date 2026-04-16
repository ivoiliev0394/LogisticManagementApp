using LogisticManagementApp.Applicationn.Interfaces.ClientPortal;
using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Infrastructure;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.ClientPortal;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.ClientPortal
{
    public class ClientAddressService : IClientAddressService
    {
        private readonly LogisticAppDbContext _db;

        public ClientAddressService(LogisticAppDbContext db)
        {
            _db = db;
        }

        private async Task<Guid> GetClientProfileIdAsync(string userId)
        {
            var clientId = await _db.ClientProfiles
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();

            return clientId ?? throw new InvalidOperationException("Client profile not found.");
        }

        public async Task<ClientAddressesViewModel> GetAddressesAsync(string clientUserId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var addresses = await _db.ClientAddresses
                .AsNoTracking()
                .Where(a => a.ClientProfileId == clientProfileId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.City)
                .ThenBy(a => a.Street)
                .Select(a => new ClientAddressItemViewModel
                {
                    Id = a.Id,
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();

            return new ClientAddressesViewModel
            {
                Addresses = addresses
            };
        }

        public async Task<ClientAddressFormViewModel?> GetAddressForEditAsync(string clientUserId, Guid addressId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            return await _db.ClientAddresses
                .AsNoTracking()
                .Where(a => a.Id == addressId && a.ClientProfileId == clientProfileId)
                .Select(a => new ClientAddressFormViewModel
                {
                    Id = a.Id,
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAddressAsync(string clientUserId, ClientAddressFormViewModel model)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            if (model.IsDefault)
            {
                var existingDefaults = await _db.ClientAddresses
                    .Where(a => a.ClientProfileId == clientProfileId && a.IsDefault)
                    .ToListAsync();

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                }
            }
            else
            {
                var hasAnyAddress = await _db.ClientAddresses
                    .AnyAsync(a => a.ClientProfileId == clientProfileId);

                if (!hasAnyAddress)
                {
                    model.IsDefault = true;
                }
            }

            var entity = new ClientAddress
            {
                Id = Guid.NewGuid(),
                ClientProfileId = clientProfileId,
                Country = model.Country.Trim(),
                City = model.City.Trim(),
                Street = model.Street.Trim(),
                PostalCode = string.IsNullOrWhiteSpace(model.PostalCode) ? null : model.PostalCode.Trim(),
                IsDefault = model.IsDefault
            };

            _db.ClientAddresses.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateAddressAsync(string clientUserId, ClientAddressFormViewModel model)
        {
            if (model.Id == null)
            {
                return false;
            }

            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var entity = await _db.ClientAddresses
                .FirstOrDefaultAsync(a => a.Id == model.Id.Value && a.ClientProfileId == clientProfileId);

            if (entity == null)
            {
                return false;
            }

            if (model.IsDefault)
            {
                var existingDefaults = await _db.ClientAddresses
                    .Where(a => a.ClientProfileId == clientProfileId && a.IsDefault && a.Id != entity.Id)
                    .ToListAsync();

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                }
            }
            else if (entity.IsDefault)
            {
                var anotherAddressExists = await _db.ClientAddresses
                    .AnyAsync(a => a.ClientProfileId == clientProfileId && a.Id != entity.Id);

                if (!anotherAddressExists)
                {
                    model.IsDefault = true;
                }
            }

            entity.Country = model.Country.Trim();
            entity.City = model.City.Trim();
            entity.Street = model.Street.Trim();
            entity.PostalCode = string.IsNullOrWhiteSpace(model.PostalCode) ? null : model.PostalCode.Trim();
            entity.IsDefault = model.IsDefault;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAddressAsync(string clientUserId, Guid addressId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var entity = await _db.ClientAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.ClientProfileId == clientProfileId);

            if (entity == null)
            {
                return false;
            }

            var wasDefault = entity.IsDefault;

            _db.ClientAddresses.Remove(entity);
            await _db.SaveChangesAsync();

            if (wasDefault)
            {
                var nextAddress = await _db.ClientAddresses
                    .Where(a => a.ClientProfileId == clientProfileId)
                    .OrderBy(a => a.City)
                    .ThenBy(a => a.Street)
                    .FirstOrDefaultAsync();

                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                    await _db.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(string clientUserId, Guid addressId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var target = await _db.ClientAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.ClientProfileId == clientProfileId);

            if (target == null)
            {
                return false;
            }

            var defaults = await _db.ClientAddresses
                .Where(a => a.ClientProfileId == clientProfileId && a.IsDefault)
                .ToListAsync();

            foreach (var addr in defaults)
            {
                addr.IsDefault = false;
            }

            target.IsDefault = true;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}