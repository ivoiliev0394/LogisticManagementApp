using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Domain.Assets.CargoUnits;
using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Compliance;
using LogisticManagementApp.Domain.Documents;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Operations;
using LogisticManagementApp.Domain.Operations.Audit;
using LogisticManagementApp.Domain.Operations.Notifications;
using LogisticManagementApp.Domain.Operations.Planning;
using LogisticManagementApp.Domain.Operations.Preferences;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Domain.Pricing;
using LogisticManagementApp.Domain.Routes;
using LogisticManagementApp.Domain.Security;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence
{
    public class LogisticAppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public LogisticAppDbContext(DbContextOptions<LogisticAppDbContext> options)
            : base(options)
        {
        }

        // Companies
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<CompanyBranch> CompanyBranches { get; set; }
        public DbSet<CompanyCapability> CompanyCapabilities { get; set; }

        // Clients
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<ClientAddress> ClientAddresses { get; set; }

        // Locations
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Dock> Docks { get; set; }

        // Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<OrderAttachment> OrderAttachments { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

        // Shipments
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentParty> ShipmentParties { get; set; }
        public DbSet<ShipmentLeg> ShipmentLegs { get; set; }
        public DbSet<LegStatusHistory> LegStatusHistories { get; set; }
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; set; }
        public DbSet<TrackingEvent> TrackingEvents { get; set; }
        public DbSet<ProofOfDelivery> ProofOfDeliveries { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<CargoItem> CargoItems { get; set; }
        public DbSet<ShipmentReference> ShipmentReferences { get; set; }
        public DbSet<ShipmentTag> ShipmentTags { get; set; }
        public DbSet<ShipmentVoyage> ShipmentVoyages { get; set; }
        public DbSet<ShipmentTrip> ShipmentTrips { get; set; }
        public DbSet<ShipmentContainer> ShipmentContainers { get; set; }
        public DbSet<ShipmentULD> ShipmentULDs { get; set; }

        // Pricing
        public DbSet<ServiceLevel> ServiceLevels { get; set; }
        public DbSet<GeoZone> GeoZones { get; set; }
        public DbSet<ZoneRule> ZoneRules { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<TariffRate> TariffRates { get; set; }
        public DbSet<Surcharge> Surcharges { get; set; }
        public DbSet<TariffSurcharge> TariffSurcharges { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<DiscountRule> DiscountRules { get; set; }
        public DbSet<PricingQuote> PricingQuotes { get; set; }
        public DbSet<PricingQuoteLine> PricingQuoteLines { get; set; }

        // Billing
        public DbSet<Charge> Charges { get; set; }
        public DbSet<ChargeRuleApplied> ChargeRulesApplied { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentAllocation> PaymentAllocations { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }

        // Documents
        public DbSet<FileResource> FileResources { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; }

        // Compliance
        public DbSet<DangerousGoodsDeclaration> DangerousGoodsDeclarations { get; set; }
        public DbSet<TemperatureRequirement> TemperatureRequirements { get; set; }
        public DbSet<ComplianceCheck> ComplianceChecks { get; set; }
        public DbSet<DGDocument> DGDocuments { get; set; }

        // Assets - Sea
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<VesselPosition> VesselPositions { get; set; }
        public DbSet<Voyage> Voyages { get; set; }
        public DbSet<VoyageStop> VoyageStops { get; set; }
        public DbSet<VesselCrewMember> VesselCrewMembers { get; set; }
        public DbSet<CrewAssignment> CrewAssignments { get; set; }

        // Assets - Road
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripStop> TripStops { get; set; }
        public DbSet<TripShipment> TripShipments { get; set; }

        // Assets - Air
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightSegment> FlightSegments { get; set; }
        public DbSet<AirCrewMember> AirCrewMembers { get; set; }
        public DbSet<AirCrewAssignment> AirCrewAssignments { get; set; }
        public DbSet<ULD> ULDs { get; set; }

        // Assets - Rail
        public DbSet<Train> Trains { get; set; }
        public DbSet<RailCar> RailCars { get; set; }
        public DbSet<RailService> RailServices { get; set; }
        public DbSet<RailMovement> RailMovements { get; set; }

        // Assets - Cargo Units
        public DbSet<Container> Containers { get; set; }
        public DbSet<ContainerSeal> ContainerSeals { get; set; }

        // Operations
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SavedFilter> SavedFilters { get; set; }
        public DbSet<CompanyDashboardConfig> CompanyDashboardConfigs { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingLeg> BookingLegs { get; set; }
        public DbSet<Consolidation> Consolidations { get; set; }
        public DbSet<ConsolidationShipment> ConsolidationShipments { get; set; }
        public DbSet<ResourceCalendar> ResourceCalendars { get; set; }
        public DbSet<ResourceAvailability> ResourceAvailabilities { get; set; }
        public DbSet<CapacityReservation> CapacityReservations { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<UtilizationSnapshot> UtilizationSnapshots { get; set; }

        // Routes
        public DbSet<Domain.Routes.Route> Routes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<RoutePlan> RoutePlans { get; set; }
        public DbSet<RoutePlanStop> RoutePlanStops { get; set; }
        public DbSet<RoutePlanShipment> RoutePlanShipments { get; set; }


        // Identity
        public DbSet<UserSession> UserSessions { get; set; }


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticAppDbContext).Assembly);

            ConfigureIdentityClone(modelBuilder);

            ConfigureDecimalPrecision(modelBuilder);

            modelBuilder.ApplySoftDeleteQueryFilter();
        }

        private static void ConfigureIdentityClone(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
            });

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Company)
                .WithOne(c => c.User)
                .HasForeignKey<ApplicationUser>(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.ClientProfile)
                .WithOne(c => c.User)
                .HasForeignKey<ClientProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");
        }

        public override int SaveChanges()
        {
            ApplyAuditAndSoftDeleteRules();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditAndSoftDeleteRules();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditAndSoftDeleteRules()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedAtUtc = null;
                    entry.Entity.IsDeleted = false;
                    entry.Entity.DeletedAtUtc = null;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }
            }
        }
        private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var decimalProperties = entityType
                    .GetProperties()
                    .Where(p =>
                        p.ClrType == typeof(decimal) ||
                        p.ClrType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    property.SetPrecision(18);
                    property.SetScale(4);
                }
            }
        }
    }
}
