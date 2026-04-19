using LogisticManagementApp.Applicationn.Services;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Tests.Helpers;

namespace LogisticManagementApp.Tests.Services;

public class HomeServiceTests
{
    [Fact]
    public async Task GetHomeDataAsync_ReturnsDistinctCountriesAndOnlyActiveCompanies()
    {
        await using var db = TestDbContextFactory.Create();

        db.Addresses.AddRange(
            new Address { Country = "Bulgaria", City = "Sofia", Street = "A" },
            new Address { Country = "Germany", City = "Berlin", Street = "B" },
            new Address { Country = "Bulgaria", City = "Plovdiv", Street = "C" },
            new Address { Country = "", City = "Varna", Street = "D" });

        db.Companies.AddRange(
            new Company { Name = "Zeta Logistics", CompanyType = CompanyType.Carrier, IsActive = true },
            new Company { Name = "Alpha Cargo", CompanyType = CompanyType.Carrier, IsActive = true },
            new Company { Name = "Inactive Co", CompanyType = CompanyType.Other, IsActive = false });

        await db.SaveChangesAsync();

        var service = new HomeService(db);

        var result = await service.GetHomeDataAsync();

        Assert.Equal(new[] { "Bulgaria", "Germany" }, result.SupportedCountries);
        Assert.Equal(new[] { "Alpha Cargo", "Zeta Logistics" }, result.CourierCompanies);
    }

    [Fact]
    public async Task TrackShipmentAsync_ReturnsBaseModel_WhenTrackingNumberIsEmpty()
    {
        await using var db = TestDbContextFactory.Create();
        db.Companies.Add(new Company { Name = "Active Co", CompanyType = CompanyType.Carrier, IsActive = true });
        await db.SaveChangesAsync();

        var service = new HomeService(db);

        var result = await service.TrackShipmentAsync("   ");

        Assert.True(string.IsNullOrEmpty(result.TrackingNumber));
        Assert.False(result.TrackingFound);
        Assert.Null(result.Status);
        Assert.Single(result.CourierCompanies);
    }

    [Fact]
    public async Task TrackShipmentAsync_ReturnsNotFound_WhenShipmentDoesNotExist()
    {
        await using var db = TestDbContextFactory.Create();
        var service = new HomeService(db);

        var result = await service.TrackShipmentAsync("SHP-2026-000999");

        Assert.False(result.TrackingFound);
        Assert.Equal("Няма намерена пратка", result.Status);
        Assert.Equal("SHP-2026-000999", result.TrackingNumber);
    }


    [Fact]
    public async Task TrackShipmentAsync_ReturnsTimelineWithoutLocation_WhenLatestEventHasNoLocation()
    {
        await using var db = TestDbContextFactory.Create();

        var company = new Company { Name = "Carrier", CompanyType = CompanyType.Carrier, IsActive = true };
        var shipment = new Shipment
        {
            ShipmentNo = "SHP-NO-LOC",
            CustomerCompany = company,
            Status = ShipmentStatus.Created,
            PrimaryMode = TransportMode.Road
        };
        var latestEvent = new TrackingEvent
        {
            Shipment = shipment,
            EventType = TrackingEventType.Created,
            EventTimeUtc = new DateTime(2026, 4, 11, 9, 30, 0, DateTimeKind.Utc),
            Details = "Created"
        };

        db.AddRange(company, shipment, latestEvent);
        await db.SaveChangesAsync();

        var service = new HomeService(db);
        var result = await service.TrackShipmentAsync("SHP-NO-LOC");

        Assert.True(result.TrackingFound);
        Assert.Null(result.CurrentLocation);
        Assert.Single(result.TrackingEvents);
        Assert.Null(result.TrackingEvents[0].Location);
    }

    [Fact]
    public async Task TrackShipmentAsync_ReturnsLatestEventAndOrderedTimeline_WhenShipmentExists()
    {
        await using var db = TestDbContextFactory.Create();

        var company = new Company { Name = "Carrier", CompanyType = CompanyType.Carrier, IsActive = true };
        var receiverAddress = new Address { Country = "Bulgaria", City = "Sofia", Street = "Receiver street" };
        var hubAddress = new Address { Country = "Germany", City = "Berlin", Street = "Hub street" };
        var location = new Location
        {
            Code = "BER-HUB",
            Name = "Berlin Hub",
            LocationType = LocationType.Warehouse,
            Address = hubAddress,
            IsActive = true
        };

        var shipment = new Shipment
        {
            ShipmentNo = "SHP-2026-000123",
            CustomerCompany = company,
            ReceiverAddress = receiverAddress,
            Status = ShipmentStatus.InTransit,
            PrimaryMode = TransportMode.Road
        };

        var olderEvent = new TrackingEvent
        {
            Shipment = shipment,
            EventType = TrackingEventType.Created,
            EventTimeUtc = new DateTime(2026, 4, 10, 8, 0, 0, DateTimeKind.Utc),
            Details = "Shipment created"
        };

        var latestEvent = new TrackingEvent
        {
            Shipment = shipment,
            EventType = TrackingEventType.Arrived,
            EventTimeUtc = new DateTime(2026, 4, 11, 9, 30, 0, DateTimeKind.Utc),
            Location = location,
            Details = "Arrived in Berlin hub"
        };

        db.AddRange(company, receiverAddress, hubAddress, location, shipment, olderEvent, latestEvent);
        await db.SaveChangesAsync();

        var service = new HomeService(db);

        var result = await service.TrackShipmentAsync(" SHP-2026-000123 ");

        Assert.True(result.TrackingFound);
        Assert.Equal("SHP-2026-000123", result.TrackingNumber);
        Assert.Equal("SHP-2026-000123", result.ShipmentNo);
        Assert.Equal(nameof(ShipmentStatus.InTransit), result.Status);
        Assert.Equal("Arrived in Berlin hub", result.Details);
        Assert.Equal("Berlin Hub, Berlin, Germany", result.CurrentLocation);
        Assert.Equal(2, result.TrackingEvents.Count);
        Assert.Equal(nameof(TrackingEventType.Arrived), result.TrackingEvents[0].EventType);
        Assert.Equal(nameof(TrackingEventType.Created), result.TrackingEvents[1].EventType);
    }


    [Fact]
    public async Task TrackShipmentAsync_ReturnsFoundWithoutTimeline_WhenShipmentHasNoEvents()
    {
        await using var db = TestDbContextFactory.Create();
        var company = new Company { Name = "Carrier", CompanyType = CompanyType.Carrier, IsActive = true };
        var shipment = new Shipment
        {
            ShipmentNo = "SHP-NO-EVENTS",
            CustomerCompany = company,
            Status = ShipmentStatus.Created,
            PrimaryMode = TransportMode.Road
        };

        db.AddRange(company, shipment);
        await db.SaveChangesAsync();

        var service = new HomeService(db);
        var result = await service.TrackShipmentAsync("SHP-NO-EVENTS");

        Assert.True(result.TrackingFound);
        Assert.Equal("Created", result.Status);
        Assert.Null(result.Details);
        Assert.Null(result.CurrentLocation);
        Assert.Empty(result.TrackingEvents);
    }

    [Fact]
    public async Task TrackShipmentAsync_ReturnsBaseCollections_WhenTrackingNumberMissingAndDatabaseHasCountries()
    {
        await using var db = TestDbContextFactory.Create();
        db.Addresses.AddRange(
            new Address { Country = "Bulgaria", City = "Sofia", Street = "A" },
            new Address { Country = "Romania", City = "Bucharest", Street = "B" });
        db.Companies.Add(new Company { Name = "Carrier", CompanyType = CompanyType.Carrier, IsActive = true });
        await db.SaveChangesAsync();

        var service = new HomeService(db);
        var result = await service.TrackShipmentAsync(null);

        Assert.Equal(new[] { "Bulgaria", "Romania" }, result.SupportedCountries);
        Assert.Single(result.CourierCompanies);
        Assert.False(result.TrackingFound);
    }

}
