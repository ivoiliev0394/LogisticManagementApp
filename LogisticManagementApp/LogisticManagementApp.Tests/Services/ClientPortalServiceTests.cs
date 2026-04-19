using LogisticManagementApp.Applicationn.Services.ClientPortal;
using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Tests.Helpers;

namespace LogisticManagementApp.Tests.Services;

public class ClientPortalServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ReturnsCounts_AndLimitsRecentCollections()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, company) = await SeedClientAsync(db, "user-1");

        db.ClientAddresses.AddRange(
            MakeAddress(profile.Id, "Bulgaria", "Varna", true),
            MakeAddress(profile.Id, "Bulgaria", "Sofia", false),
            MakeAddress(profile.Id, "Germany", "Berlin", false),
            MakeAddress((await SeedClientAsync(db, "user-foreign")).profile.Id, "Romania", "Bucharest", true));

        var pickup = new Address { Country = "Bulgaria", City = "Sofia", Street = "Pickup" };
        var delivery = new Address { Country = "Germany", City = "Berlin", Street = "Delivery" };
        db.AddRange(pickup, delivery);

        var orders = Enumerable.Range(1, 6).Select(i => new Order
        {
            OrderNo = $"ORD-2026-00000{i}",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            ClientProfileId = profile.Id,
            PickupAddress = pickup,
            DeliveryAddress = delivery,
            Status = i == 6 ? OrderStatus.Completed : OrderStatus.Confirmed,
            Priority = i % 2 == 0 ? OrderPriority.High : OrderPriority.Normal,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-i),
            RequestedPickupDateUtc = DateTime.UtcNow.AddDays(i)
        }).ToList();

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();

        var shipment1 = new Shipment { ShipmentNo = "SHP-1", CustomerCompanyId = company.Id, CustomerCompany = company, OrderId = orders[0].Id, Order = orders[0], ReceiverAddress = delivery, Status = ShipmentStatus.InTransit, CreatedAtUtc = DateTime.UtcNow.AddHours(-1) };
        var shipment2 = new Shipment { ShipmentNo = "SHP-2", CustomerCompanyId = company.Id, CustomerCompany = company, OrderId = orders[1].Id, Order = orders[1], ReceiverAddress = delivery, Status = ShipmentStatus.Delivered, CreatedAtUtc = DateTime.UtcNow.AddHours(-2) };
        var shipment3 = new Shipment { ShipmentNo = "SHP-3", CustomerCompanyId = company.Id, CustomerCompany = company, OrderId = orders[2].Id, Order = orders[2], ReceiverAddress = delivery, Status = ShipmentStatus.Created, CreatedAtUtc = DateTime.UtcNow.AddHours(-3) };
        db.Shipments.AddRange(shipment1, shipment2, shipment3);

        db.TrackingEvents.AddRange(
            new TrackingEvent { Shipment = shipment1, EventType = TrackingEventType.InTransit, EventTimeUtc = DateTime.UtcNow.AddMinutes(-30), Details = "On the way" },
            new TrackingEvent { Shipment = shipment2, EventType = TrackingEventType.Delivered, EventTimeUtc = DateTime.UtcNow.AddMinutes(-60), Details = "Delivered" });

        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetDashboardAsync("user-1");

        Assert.Equal("Demo User", result.FullName);
        Assert.Equal(3, result.AddressCount);
        Assert.Equal(6, result.OrderCount);
        Assert.Equal(5, result.ActiveOrderCount);
        Assert.Equal(3, result.ShipmentCount);
        Assert.Equal(2, result.ActiveShipmentCount);
        Assert.Equal(3, result.Addresses.Count);
        Assert.Equal(5, result.RecentOrders.Count);
        Assert.Equal(3, result.RecentShipments.Count);
        Assert.Equal("InTransit", result.RecentShipments[0].LastTrackingEvent);
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsOnlyActiveOrders_AndMapsLatestShipmentPerOrder()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, company) = await SeedClientAsync(db, "user-1");
        var pickup = new Address { Country = "Bulgaria", City = "Sofia", Street = "Pickup" };
        var delivery = new Address { Country = "Greece", City = "Athens", Street = "Delivery" };
        db.AddRange(pickup, delivery);

        var order = new Order
        {
            OrderNo = "ORD-2026-000001",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            ClientProfileId = profile.Id,
            PickupAddress = pickup,
            DeliveryAddress = delivery,
            Status = OrderStatus.Confirmed,
            Priority = OrderPriority.High,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };
        var inactiveOrder = new Order
        {
            OrderNo = "ORD-2026-000002",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            ClientProfileId = profile.Id,
            Status = OrderStatus.Draft,
            Priority = OrderPriority.Normal,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            IsActive = false
        };
        db.Orders.AddRange(order, inactiveOrder);
        await db.SaveChangesAsync();

        db.Shipments.AddRange(
            new Shipment { Id = new Guid("00000000-0000-0000-0000-000000000001"), ShipmentNo = "SHP-OLD", CustomerCompanyId = company.Id, CustomerCompany = company, OrderId = order.Id, Order = order },
            new Shipment { Id = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), ShipmentNo = "SHP-NEW", CustomerCompanyId = company.Id, CustomerCompany = company, OrderId = order.Id, Order = order });
        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetOrdersAsync("user-1");

        var single = Assert.Single(result.Orders);
        Assert.Equal("ORD-2026-000001", single.OrderNo);
        Assert.Equal("SHP-NEW", single.ShipmentNo);
        Assert.Contains("Pickup", single.PickupAddress);
        Assert.Contains("Delivery", single.DeliveryAddress);
    }


    [Fact]
    public async Task GetAddressesAsync_ReturnsOnlyCurrentClientAddresses_DefaultFirst()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, _) = await SeedClientAsync(db, "user-1");
        var (otherProfile, _) = await SeedClientAsync(db, "user-2");

        db.ClientAddresses.AddRange(
            MakeAddress(profile.Id, "Bulgaria", "Sofia", false),
            MakeAddress(profile.Id, "Bulgaria", "Varna", true),
            MakeAddress(otherProfile.Id, "Romania", "Bucharest", true));
        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetAddressesAsync("user-1");

        Assert.Equal(2, result.Addresses.Count);
        Assert.True(result.Addresses[0].IsDefault);
        Assert.Equal("Varna", result.Addresses[0].City);
        Assert.DoesNotContain(result.Addresses, x => x.City == "Bucharest");
    }

    [Fact]
    public async Task GetShipmentTrackingAsync_ReturnsNull_WhenShipmentDoesNotBelongToClient()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, company) = await SeedClientAsync(db, "user-1");
        var otherProfile = await SeedClientAsync(db, "user-2");
        var order = new Order
        {
            OrderNo = "ORD-OWNERSHIP",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            ClientProfileId = otherProfile.profile.Id,
            Status = OrderStatus.Confirmed,
            Priority = OrderPriority.Normal
        };
        var shipment = new Shipment { ShipmentNo = "SHP-FOREIGN", CustomerCompanyId = company.Id, CustomerCompany = company, Order = order, Status = ShipmentStatus.Created };
        db.AddRange(order, shipment);
        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetShipmentTrackingAsync("user-1", shipment.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetShipmentTrackingAsync_ReturnsLatestEventAndTimeline_ForOwnedShipment()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, company) = await SeedClientAsync(db, "user-1");
        var address = new Address { Country = "Germany", City = "Berlin", Street = "Hub" };
        var location = new Location { Code = "BER", Name = "Berlin Hub", LocationType = LocationType.Warehouse, Address = address, IsActive = true };
        var order = new Order { OrderNo = "ORD-2026-TRACK", CustomerCompanyId = company.Id, CustomerCompany = company, ClientProfileId = profile.Id, Status = OrderStatus.Confirmed, Priority = OrderPriority.Normal };
        var shipment = new Shipment { ShipmentNo = "SHP-2026-TRACK", CustomerCompanyId = company.Id, CustomerCompany = company, Order = order, Status = ShipmentStatus.InTransit, Notes = "Handle carefully" };
        var older = new TrackingEvent { Shipment = shipment, EventType = TrackingEventType.Created, EventTimeUtc = new DateTime(2026, 4, 10, 8, 0, 0, DateTimeKind.Utc), Details = "Created" };
        var latest = new TrackingEvent { Shipment = shipment, EventType = TrackingEventType.Arrived, EventTimeUtc = new DateTime(2026, 4, 11, 9, 30, 0, DateTimeKind.Utc), Location = location, Details = "Arrived at hub" };

        db.AddRange(address, location, order, shipment, older, latest);
        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetShipmentTrackingAsync("user-1", shipment.Id);

        Assert.NotNull(result);
        Assert.Equal("SHP-2026-TRACK", result!.ShipmentNo);
        Assert.Equal("InTransit", result.Status);
        Assert.Equal("Arrived at hub", result.Details);
        Assert.Equal("Berlin Hub, Berlin, Germany", result.CurrentLocation);
        Assert.Equal(2, result.TrackingEvents.Count);
        Assert.Equal("Arrived", result.TrackingEvents[0].EventType);
    }


    [Fact]
    public async Task GetDashboardAsync_Throws_WhenClientProfileIsMissing()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new ClientPortalService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetDashboardAsync("missing-user"));
    }

    [Fact]
    public async Task GetShipmentsAsync_ReturnsOnlyOwnedShipments_AndMapsLatestTracking()
    {
        await using var db = TestDbContextFactory.Create();
        var (profile, company) = await SeedClientAsync(db, "user-1");
        var other = await SeedClientAsync(db, "user-2");
        var receiver = new Address { Country = "Bulgaria", City = "Sofia", Street = "Receiver" };
        var order = new Order { OrderNo = "ORD-SHIP", CustomerCompanyId = company.Id, CustomerCompany = company, ClientProfileId = profile.Id, Status = OrderStatus.Confirmed, Priority = OrderPriority.Normal, IsActive = true };
        var ownedShipment = new Shipment { ShipmentNo = "SHP-OWN", CustomerCompanyId = company.Id, CustomerCompany = company, Order = order, ReceiverAddress = receiver, Status = ShipmentStatus.InTransit, CustomerReference = "REF", CreatedAtUtc = DateTime.UtcNow };
        var foreignOrder = new Order { OrderNo = "ORD-FOR", CustomerCompanyId = other.company.Id, CustomerCompany = other.company, ClientProfileId = other.profile.Id, Status = OrderStatus.Confirmed, Priority = OrderPriority.Normal, IsActive = true };
        var foreignShipment = new Shipment { ShipmentNo = "SHP-FOR", CustomerCompanyId = other.company.Id, CustomerCompany = other.company, Order = foreignOrder, Status = ShipmentStatus.Created, CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10) };
        db.AddRange(receiver, order, ownedShipment, foreignOrder, foreignShipment,
            new TrackingEvent { Shipment = ownedShipment, EventType = TrackingEventType.Departed, EventTimeUtc = DateTime.UtcNow.AddMinutes(-5), Details = "Departed" });
        await db.SaveChangesAsync();

        var service = new ClientPortalService(db);
        var result = await service.GetShipmentsAsync("user-1");

        var single = Assert.Single(result.Shipments);
        Assert.Equal("SHP-OWN", single.ShipmentNo);
        Assert.Equal("Departed", single.LastTrackingEvent);
        Assert.DoesNotContain(result.Shipments, x => x.ShipmentNo == "SHP-FOR");
    }

    [Fact]
    public async Task GetShipmentTrackingAsync_Throws_WhenClientProfileMissing()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new ClientPortalService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetShipmentTrackingAsync("missing-user", Guid.NewGuid()));
    }

    private static ClientAddress MakeAddress(Guid clientProfileId, string country, string city, bool isDefault) => new()
    {
        ClientProfileId = clientProfileId,
        Country = country,
        City = city,
        Street = city + " street",
        IsDefault = isDefault
    };

    private static async Task<(ClientProfile profile, Company company)> SeedClientAsync(LogisticManagementApp.Infrastructure.Persistence.LogisticAppDbContext db, string userId)
    {
        var user = new ApplicationUser { Id = userId, UserName = userId, Email = userId + "@example.com" };
        var company = new Company { Name = "Demo Carrier", CompanyType = CompanyType.Carrier, IsActive = true };
        var profile = new ClientProfile
        {
            UserId = userId,
            User = user,
            FirstName = "Demo",
            LastName = "User",
            EmailForContact = user.Email,
            CreatedOnUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Companies.Add(company);
        db.ClientProfiles.Add(profile);
        await db.SaveChangesAsync();
        return (profile, company);
    }
}
