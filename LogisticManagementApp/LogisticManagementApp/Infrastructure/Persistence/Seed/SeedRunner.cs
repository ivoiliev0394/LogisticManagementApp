using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Infrastructure.Persistence.Seed;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.CargoUnits;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Rail;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Road;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Billing;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Clients;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Companies;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Compliance;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Documents;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Identity;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Locations;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Operations;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Audit;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Notifications;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Preferences;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Orders;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Pricing;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Routes;
using LogisticManagementApp.Infrastructure.Persistence.Seed.Shipments;
using Microsoft.AspNetCore.Identity;
using System.Text;

public static class SeedRunner
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, LogisticAppDbContext db)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        async Task Run(string name, Func<Task> action)
        {
            try
            {
                Console.Write($"Seeding {name}... ");
                await action();
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine("✔️");
            }
            catch (Exception ex)
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine("❌ FAILED");
                Console.WriteLine($"Error in {name}: {ex.Message}");
                throw;
            }
        }

        // 1. CORE / IDENTITY BASE
        await Run("Roles", () => RoleSeeder.SeedAsync(roleManager));

        // 2. LOCATIONS
        await Run("Addresses", () => AddressSeeder.SeedAsync(db));
        await Run("Locations", () => LocationSeeder.SeedAsync(db));
        await Run("Warehouses", () => WarehouseSeeder.SeedAsync(db));
        await Run("Terminals", () => TerminalSeeder.SeedAsync(db));
        await Run("Docks", () => DockSeeder.SeedAsync(db));

        // 3. COMPANIES
        await Run("Companies", () => CompanySeeder.SeedAsync(db));
        await Run("Company Branches", () => CompanyBranchSeeder.SeedAsync(db));
        await Run("Company Contacts", () => CompanyContactSeeder.SeedAsync(db));
        await Run("Company Capabilities", () => CompanyCapabilitySeeder.SeedAsync(db));

        // 4. USERS
        await Run("Users", () => UserSeeder.SeedAsync(db, userManager));

        // 4.1 CLIENTS
        await Run("Client Profiles", () => ClientProfileSeeder.SeedAsync(db, userManager));
        await Run("Client Addresses", () => ClientAddressSeeder.SeedAsync(db));

        // 5. STATIC MASTER DATA / PRICING BASE
        await Run("Service Levels", () => ServiceLevelSeeder.SeedAsync(db));
        await Run("Geo Zones", () => GeoZoneSeeder.SeedAsync(db));
        await Run("Zone Rules", () => ZoneRuleSeeder.SeedAsync(db));
        await Run("Surcharges", () => SurchargeSeeder.SeedAsync(db));
        await Run("Tax Rates", () => TaxRateSeeder.SeedAsync(db));

        // 6. ASSETS - SEA
        await Run("Vessels", () => VesselSeeder.SeedAsync(db));
        await Run("Voyages", () => VoyageSeeder.SeedAsync(db));
        await Run("Voyage Stops", () => VoyageStopSeeder.SeedAsync(db));
        await Run("Vessel Positions", () => VesselPositionSeeder.SeedAsync(db));
        await Run("Vessel Crew Members", () => VesselCrewMemberSeeder.SeedAsync(db));
        await Run("Crew Assignments", () => CrewAssignmentSeeder.SeedAsync(db));

        // 7. ASSETS - ROAD
        await Run("Vehicles", () => VehicleSeeder.SeedAsync(db));
        await Run("Drivers", () => DriverSeeder.SeedAsync(db));
        await Run("Trips", () => TripSeeder.SeedAsync(db));
        await Run("Trip Stops", () => TripStopSeeder.SeedAsync(db));

        // 8. ASSETS - AIR
        await Run("Aircraft", () => AircraftSeeder.SeedAsync(db));
        await Run("Air Crew Members", () => AirCrewMemberSeeder.SeedAsync(db));
        await Run("Flights", () => FlightSeeder.SeedAsync(db));
        await Run("Flight Segments", () => FlightSegmentSeeder.SeedAsync(db));
        await Run("Air Crew Assignments", () => AirCrewAssignmentSeeder.SeedAsync(db));
        await Run("ULD", () => ULDSeeder.SeedAsync(db));

        // 9. ASSETS - RAIL
        await Run("Trains", () => TrainSeeder.SeedAsync(db));
        await Run("Rail Cars", () => RailCarSeeder.SeedAsync(db));
        await Run("Rail Services", () => RailServiceSeeder.SeedAsync(db));
        await Run("Rail Movements", () => RailMovementSeeder.SeedAsync(db));

        // 10. ASSETS - CARGO UNITS
        await Run("Containers", () => ContainerSeeder.SeedAsync(db));
        await Run("Container Seals", () => ContainerSealSeeder.SeedAsync(db));

        // 11. FILES
        await Run("File Resources", () => FileResourceSeeder.SeedAsync(db));

        // 12. ROUTES
        await Run("Routes", () => RouteSeeder.SeedAsync(db));
        await Run("Route Stops", () => RouteStopSeeder.SeedAsync(db));
        await Run("Route Plans", () => RoutePlanSeeder.SeedAsync(db));
        await Run("Route Plan Stops", () => RoutePlanStopSeeder.SeedAsync(db));

        // 13. ORDERS
        await Run("Orders", () => OrderSeeder.SeedAsync(db));
        await Run("Order Lines", () => OrderLineSeeder.SeedAsync(db));
        await Run("Order Attachments", () => OrderAttachmentSeeder.SeedAsync(db));
        await Run("Order Status History", () => OrderStatusHistorySeeder.SeedAsync(db));

        // 14. PRICING - COMMERCIAL
        await Run("Tariffs", () => TariffSeeder.SeedAsync(db));
        await Run("Tariff Rates", () => TariffRateSeeder.SeedAsync(db));
        await Run("Agreements", () => AgreementSeeder.SeedAsync(db));
        await Run("Discount Rules", () => DiscountRuleSeeder.SeedAsync(db));

        // 15. SHIPMENTS
        await Run("Shipments", () => ShipmentSeeder.SeedAsync(db));
        await Run("Packages", () => PackageSeeder.SeedAsync(db));
        await Run("Package Items", () => PackageItemSeeder.SeedAsync(db));
        await Run("Cargo Items", () => CargoItemSeeder.SeedAsync(db));
        await Run("Shipment Legs", () => ShipmentLegSeeder.SeedAsync(db));

        await Run("Shipment Parties", () => ShipmentPartySeeder.SeedAsync(db));
        await Run("Shipment References", () => ShipmentReferenceSeeder.SeedAsync(db));
        await Run("Shipment Tags", () => ShipmentTagSeeder.SeedAsync(db));
        await Run("Shipment Status History", () => ShipmentStatusHistorySeeder.SeedAsync(db));
        await Run("Tracking Events", () => TrackingEventSeeder.SeedAsync(db));

        await Run("Proof of Delivery", () => ProofOfDeliverySeeder.SeedAsync(db));
        await Run("Shipment Voyages", () => ShipmentVoyageSeeder.SeedAsync(db));
        await Run("Shipment Trips", () => ShipmentTripSeeder.SeedAsync(db));
        await Run("Shipment Containers", () => ShipmentContainerSeeder.SeedAsync(db));
        await Run("Shipment ULD", () => ShipmentULDSeeder.SeedAsync(db));

        // 16. DOCUMENTS
        await Run("Documents", () => DocumentSeeder.SeedAsync(db));
        await Run("Document Versions", () => DocumentVersionSeeder.SeedAsync(db));
        await Run("Document Templates", () => DocumentTemplateSeeder.SeedAsync(db));

        // 17. PRICING - QUOTES
        await Run("Pricing Quotes", () => PricingQuoteSeeder.SeedAsync(db));
        await Run("Pricing Quote Lines", () => PricingQuoteLineSeeder.SeedAsync(db));

        // 18. COMPLIANCE
        await Run("Compliance Checks", () => ComplianceCheckSeeder.SeedAsync(db));
        await Run("Dangerous Goods Declarations", () => DangerousGoodsDeclarationSeeder.SeedAsync(db));
        await Run("DG Documents", () => DGDocumentSeeder.SeedAsync(db));
        await Run("Temperature Requirements", () => TemperatureRequirementSeeder.SeedAsync(db));

        // 19. BILLING
        await Run("Charges", () => ChargeSeeder.SeedAsync(db));
        await Run("Invoices", () => InvoiceSeeder.SeedAsync(db));
        await Run("Invoice Lines", () => InvoiceLineSeeder.SeedAsync(db));
        await Run("Payments", () => PaymentSeeder.SeedAsync(db));
        await Run("Payment Allocations", () => PaymentAllocationSeeder.SeedAsync(db));
        await Run("Credit Notes", () => CreditNoteSeeder.SeedAsync(db));

        // 20. OPERATIONS - NOTIFICATIONS
        await Run("Notifications", () => NotificationSeeder.SeedAsync(db));
        await Run("Notification Subscriptions", () => NotificationSubscriptionSeeder.SeedAsync(db));

        // 21. OPERATIONS - BOOKINGS
        await Run("Bookings", () => BookingSeeder.SeedAsync(db));
        await Run("Booking Legs", () => BookingLegSeeder.SeedAsync(db));

        // 22. OPERATIONS - CONSOLIDATION
        await Run("Consolidations", () => ConsolidationSeeder.SeedAsync(db));
        await Run("Consolidation Shipments", () => ConsolidationShipmentSeeder.SeedAsync(db));

        // 23. OPERATIONS - PLANNING
        await Run("Resource Calendars", () => ResourceCalendarSeeder.SeedAsync(db));
        await Run("Resource Availability", () => ResourceAvailabilitySeeder.SeedAsync(db));
        await Run("Capacity Reservations", () => CapacityReservationSeeder.SeedAsync(db));
        await Run("Assignments", () => AssignmentSeeder.SeedAsync(db));
        await Run("Utilization Snapshots", () => UtilizationSnapshotSeeder.SeedAsync(db));

        // 24. OPERATIONS - PREFERENCES / AUDIT
        await Run("Saved Filters", () => SavedFilterSeeder.SeedAsync(db));
        await Run("Dashboard Configs", () => CompanyDashboardConfigSeeder.SeedAsync(db));
        await Run("Audit Logs", () => AuditLogSeeder.SeedAsync(db));

        // 25. FINAL LINKS
        await Run("Route Plan Shipments", () => RoutePlanShipmentSeeder.SeedAsync(db));
        await Run("Trip Shipments", () => TripShipmentSeeder.SeedAsync(db));
    }
}