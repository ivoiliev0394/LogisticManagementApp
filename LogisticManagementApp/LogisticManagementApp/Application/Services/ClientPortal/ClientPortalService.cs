using LogisticManagementApp.Applicationn.Interfaces.ClientPortal;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.ClientPortal;
using Microsoft.EntityFrameworkCore;
using static LogisticManagementApp.Models.ClientPortal.ClientShipmentTrackingViewModel;

namespace LogisticManagementApp.Applicationn.Services.ClientPortal
{
    public class ClientPortalService : IClientPortalService
    {
        private readonly LogisticAppDbContext _db;

        public ClientPortalService(LogisticAppDbContext db)
        {
            _db = db;
        }

        public async Task<ClientDashboardViewModel> GetDashboardAsync(string userId)
        {
            var client = await GetClientProfileQuery(userId)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.EmailForContact,
                    x.PhoneNumber,
                    x.CreatedOnUtc,
                    Username = x.User != null ? x.User.UserName : null
                })
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("Client profile not found.");

            var addresses = await GetAddressesQuery(client.Id)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Country)
                .ThenBy(x => x.City)
                .Select(x => new ClientAddressItemViewModel
                {
                    Id = x.Id,
                    Country = x.Country,
                    City = x.City,
                    Street = x.Street,
                    PostalCode = x.PostalCode,
                    IsDefault = x.IsDefault
                })
                .ToListAsync();

            var orders = await GetOrdersBaseQuery(client.Id)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ClientDashboardOrderItemViewModel
                {
                    Id = x.Id,
                    OrderNo = x.OrderNo,
                    Status = x.Status.ToString(),
                    Priority = x.Priority.ToString(),
                    PickupAddress = x.PickupAddress != null
                        ? x.PickupAddress.Country + ", " + x.PickupAddress.City + ", " + x.PickupAddress.Street
                        : null,
                    DeliveryAddress = x.DeliveryAddress != null
                        ? x.DeliveryAddress.Country + ", " + x.DeliveryAddress.City + ", " + x.DeliveryAddress.Street
                        : null,
                    RequestedPickupDateUtc = x.RequestedPickupDateUtc
                })
                .ToListAsync();

            var shipmentEntities = await GetShipmentsBaseQuery(client.Id)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    OrderNo = x.Order != null ? x.Order.OrderNo : null,
                    x.Status,
                    x.CustomerReference,
                    ReceiverAddress = x.ReceiverAddress != null
                        ? x.ReceiverAddress.Country + ", " + x.ReceiverAddress.City + ", " + x.ReceiverAddress.Street
                        : null
                })
                .ToListAsync();

            var shipmentIds = shipmentEntities.Select(x => x.Id).ToList();

            var latestTracking = await _db.TrackingEvents
                .AsNoTracking()
                .Where(x => shipmentIds.Contains(x.ShipmentId))
                .GroupBy(x => x.ShipmentId)
                .Select(g => new
                {
                    ShipmentId = g.Key,
                    LastTrackingEventUtc = g.Max(x => x.EventTimeUtc),
                    LastTrackingEvent = g
                        .OrderByDescending(x => x.EventTimeUtc)
                        .Select(x => x.EventType.ToString())
                        .FirstOrDefault()
                })
                .ToDictionaryAsync(
                    x => x.ShipmentId,
                    x => new { x.LastTrackingEvent, x.LastTrackingEventUtc });

            var shipments = shipmentEntities
                .Select(x => new ClientDashboardShipmentItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.ShipmentNo,
                    OrderNo = x.OrderNo ?? "-",
                    Status = x.Status.ToString(),
                    ReceiverAddress = x.ReceiverAddress,
                    CustomerReference = x.CustomerReference,
                    LastTrackingEvent = latestTracking.TryGetValue(x.Id, out var te) ? te.LastTrackingEvent : null,
                    LastTrackingEventUtc = latestTracking.TryGetValue(x.Id, out te) ? te.LastTrackingEventUtc : null
                })
                .ToList();

            return new ClientDashboardViewModel
            {
                FullName = $"{client.FirstName} {client.LastName}".Trim(),
                Username = client.Username ?? string.Empty,
                Email = client.EmailForContact ?? string.Empty,
                PhoneNumber = client.PhoneNumber,
                CreatedOnUtc = client.CreatedOnUtc,

                AddressCount = addresses.Count,
                OrderCount = orders.Count,
                ActiveOrderCount = orders.Count(x =>
                    x.Status != OrderStatus.Completed.ToString() &&
                    x.Status != OrderStatus.Cancelled.ToString()),

                ShipmentCount = shipments.Count,
                ActiveShipmentCount = shipments.Count(x =>
                    x.Status != ShipmentStatus.Delivered.ToString() &&
                    x.Status != ShipmentStatus.Cancelled.ToString()),

                Addresses = addresses.Take(3).ToList(),
                RecentOrders = orders.Take(5).ToList(),
                RecentShipments = shipments.Take(5).ToList()
            };
        }

        //public async Task<ClientOrdersViewModel> GetOrdersAsync(string userId)
        //{
        //    var clientId = await GetClientProfileIdAsync(userId);

        //    var orders = await GetOrdersBaseQuery(clientId)
        //        .OrderByDescending(x => x.CreatedAtUtc)
        //        .Select(x => new ClientDashboardOrderItemViewModel
        //        {
        //            Id = x.Id,
        //            OrderNo = x.OrderNo,
        //            Status = x.Status.ToString(),
        //            Priority = x.Priority.ToString(),
        //            PickupAddress = x.PickupAddress != null
        //                ? x.PickupAddress.Country + ", " + x.PickupAddress.City + ", " + x.PickupAddress.Street
        //                : null,
        //            DeliveryAddress = x.DeliveryAddress != null
        //                ? x.DeliveryAddress.Country + ", " + x.DeliveryAddress.City + ", " + x.DeliveryAddress.Street
        //                : null,
        //            RequestedPickupDateUtc = x.RequestedPickupDateUtc
        //        })
        //        .ToListAsync();

        //    return new ClientOrdersViewModel
        //    {
        //        Orders = orders
        //    };
        //}
        public async Task<ClientOrdersViewModel> GetOrdersAsync(string clientUserId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var orders = await _db.Orders
                .AsNoTracking()
                .Where(o => o.ClientProfileId == clientProfileId && o.IsActive)
                .Include(o => o.PickupAddress)
                .Include(o => o.DeliveryAddress)
                .OrderByDescending(o => o.CreatedAtUtc) // ако това поле го нямаш, смени го
                .ToListAsync();

            var orderIds = orders.Select(o => o.Id).ToList();

            var shipments = await _db.Shipments
                .AsNoTracking()
                .Where(s => s.OrderId.HasValue && orderIds.Contains(s.OrderId.Value))
                .Select(s => new
                {
                    OrderId = s.OrderId!.Value,
                    ShipmentId = s.Id,
                    ShipmentNo = s.ShipmentNo
                })
                .ToListAsync();

            var shipmentsByOrderId = shipments
                .GroupBy(s => s.OrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.ShipmentId).First());

            var items = orders.Select(o =>
            {
                shipmentsByOrderId.TryGetValue(o.Id, out var shipment);

                return new ClientDashboardOrderItemViewModel
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    Status = o.Status.ToString(),
                    Priority = o.Priority.ToString(),
                    PickupAddress = o.PickupAddress != null
                        ? $"{o.PickupAddress.Street}, {o.PickupAddress.City}, {o.PickupAddress.Country}"
                        : null,
                    DeliveryAddress = o.DeliveryAddress != null
                        ? $"{o.DeliveryAddress.Street}, {o.DeliveryAddress.City}, {o.DeliveryAddress.Country}"
                        : null,
                    RequestedPickupDateUtc = o.RequestedPickupDateUtc,
                    ShipmentId = shipment?.ShipmentId,
                    ShipmentNo = shipment?.ShipmentNo
                };
            }).ToList();

            return new ClientOrdersViewModel
            {
                Orders = items
            };
        }

        public async Task<ClientShipmentsViewModel> GetShipmentsAsync(string userId)
        {
            var clientId = await GetClientProfileIdAsync(userId);

            var shipmentEntities = await GetShipmentsBaseQuery(clientId)
                 .OrderByDescending(x => x.CreatedAtUtc)
                 .Select(x => new
                 {
                     x.Id,
                     x.ShipmentNo,
                     OrderNo = x.Order != null ? x.Order.OrderNo : null,
                     x.Status,
                     x.CustomerReference,
                     ReceiverAddress = x.ReceiverAddress != null
                         ? x.ReceiverAddress.Country + ", " + x.ReceiverAddress.City + ", " + x.ReceiverAddress.Street
                         : null
                 })
                 .ToListAsync();

            var shipmentIds = shipmentEntities.Select(x => x.Id).ToList();

            var latestTracking = await _db.TrackingEvents
                .AsNoTracking()
                .Where(x => shipmentIds.Contains(x.ShipmentId))
                .GroupBy(x => x.ShipmentId)
                .Select(g => new
                {
                    ShipmentId = g.Key,
                    LastTrackingEventUtc = g.Max(x => x.EventTimeUtc),
                    LastTrackingEvent = g
                        .OrderByDescending(x => x.EventTimeUtc)
                        .Select(x => x.EventType.ToString())
                        .FirstOrDefault()
                })
                .ToDictionaryAsync(
                    x => x.ShipmentId,
                    x => new { x.LastTrackingEvent, x.LastTrackingEventUtc });

            var shipments = shipmentEntities
                .Select(x => new ClientDashboardShipmentItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.ShipmentNo,
                    OrderNo = x.OrderNo ?? "-",
                    Status = x.Status.ToString(),
                    ReceiverAddress = x.ReceiverAddress,
                    CustomerReference = x.CustomerReference,
                    LastTrackingEvent = latestTracking.TryGetValue(x.Id, out var te) ? te.LastTrackingEvent : null,
                    LastTrackingEventUtc = latestTracking.TryGetValue(x.Id, out te) ? te.LastTrackingEventUtc : null
                })
                .ToList();

            return new ClientShipmentsViewModel
            {
                Shipments = shipments
            };
        }

        public async Task<ClientAddressesViewModel> GetAddressesAsync(string userId)
        {
            var clientId = await GetClientProfileIdAsync(userId);

            var addresses = await GetAddressesQuery(clientId)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Country)
                .ThenBy(x => x.City)
                .Select(x => new ClientAddressItemViewModel
                {
                    Id = x.Id,
                    Country = x.Country,
                    City = x.City,
                    Street = x.Street,
                    PostalCode = x.PostalCode,
                    IsDefault = x.IsDefault
                })
                .ToListAsync();

            return new ClientAddressesViewModel
            {
                Addresses = addresses
            };
        }

        private IQueryable<Domain.Clients.ClientProfile> GetClientProfileQuery(string userId)
            => _db.ClientProfiles
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.UserId == userId);

        private IQueryable<Domain.Clients.ClientAddress> GetAddressesQuery(Guid clientProfileId)
            => _db.ClientAddresses
                .AsNoTracking()
                .Where(x => x.ClientProfileId == clientProfileId);

        private IQueryable<Domain.Orders.Order> GetOrdersBaseQuery(Guid clientProfileId)
            => _db.Orders
                .AsNoTracking()
                .Include(x => x.PickupAddress)
                .Include(x => x.DeliveryAddress)
                .Where(x => x.ClientProfileId == clientProfileId && x.IsActive);

        private IQueryable<Domain.Shipments.Shipment> GetShipmentsBaseQuery(Guid clientProfileId)
            => _db.Shipments
                .AsNoTracking()
                .Include(x => x.ReceiverAddress)
                .Include(x => x.Order)
                .Where(x => x.Order != null && x.Order.ClientProfileId == clientProfileId);

        private async Task<Guid> GetClientProfileIdAsync(string userId)
        {
            var clientId = await _db.ClientProfiles
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();

            return clientId ?? throw new InvalidOperationException("Client profile not found.");
        }

        public async Task<ClientShipmentTrackingViewModel?> GetShipmentTrackingAsync(string clientUserId, Guid shipmentId)
        {
            var clientProfileId = await GetClientProfileIdAsync(clientUserId);

            var shipment = await _db.Shipments
                .AsNoTracking()
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s =>
                    s.Id == shipmentId &&
                    s.Order != null &&
                    s.Order.ClientProfileId == clientProfileId);

            if (shipment == null)
            {
                return null;
            }

            var trackingEvents = await _db.TrackingEvents
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipment.Id)
                .Include(x => x.Location)
                    .ThenInclude(x => x.Address)
                .OrderByDescending(x => x.EventTimeUtc)
                .ToListAsync();

            var latestEvent = trackingEvents.FirstOrDefault();

            var model = new ClientShipmentTrackingViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                Status = shipment.Status.ToString(),
                Notes = shipment.Notes,
                LastUpdate = latestEvent?.EventTimeUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                Details = latestEvent?.Details,
                CurrentLocation = latestEvent?.Location != null
                    ? $"{latestEvent.Location.Name}, {latestEvent.Location.Address.City}, {latestEvent.Location.Address.Country}"
                    : null,
                TrackingEvents = trackingEvents
                    .Select(x => new ClientTrackingEventViewModel
                    {
                        EventType = x.EventType.ToString(),
                        EventTime = x.EventTimeUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                        Location = x.Location != null
                            ? $"{x.Location.Name}, {x.Location.Address.City}, {x.Location.Address.Country}"
                            : null,
                        Details = x.Details
                    })
                    .ToList()
            };

            return model;
        }
    }
}
