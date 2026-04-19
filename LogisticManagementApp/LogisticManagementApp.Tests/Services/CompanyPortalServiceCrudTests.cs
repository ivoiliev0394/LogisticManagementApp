using LogisticManagementApp.Applicationn.Services;
using LogisticManagementApp.Applicationn.Services.CompanyPortal;
using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Domain.Enums.Locations;
using LogisticManagementApp.Domain.Enums.Routes;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Locations;
using LogisticManagementApp.Domain.Routes;
using LogisticManagementApp.Models.CompanyPortal;
using LogisticManagementApp.Models.CompanyPortal.Assets.Road;
using LogisticManagementApp.Models.CompanyPortal.Branches;
using LogisticManagementApp.Models.CompanyPortal.Capabilities;
using LogisticManagementApp.Models.CompanyPortal.Contacts;
using LogisticManagementApp.Models.CompanyPortal.Routes;
using LogisticManagementApp.Models.CompanyPortal.Shipments;
using LogisticManagementApp.Models.CompanyPortal.Orders;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Tests.Helpers;
using Moq;
using Microsoft.EntityFrameworkCore;
using LogisticManagementApp.Domain.Enums.Orders;

namespace LogisticManagementApp.Tests.Services;

public class CompanyPortalServiceCrudTests
{

    [Fact]
    public async Task GetSharedLocationsHomeAsync_ReturnsAllNavigationCards()
    {
        await using var db = TestDbContextFactory.Create();
        var service = CreateService(db);

        var model = await service.GetSharedLocationsHomeAsync();

        Assert.Equal(5, model.Cards.Count());
        Assert.Contains(model.Cards, x => x.Title == "Addresses");
        Assert.Contains(model.Cards, x => x.Title == "Docks");
    }

    [Fact]
    public async Task CompanyProfile_Flow_ReadsAndUpdatesCompany()
    {
        await using var db = TestDbContextFactory.Create();
        var company = new Company { Name = "Alpha Logistics", CompanyType = CompanyType.Carrier, TaxNumber = "BG123", Website = "old", Notes = "old notes" };
        db.Companies.Add(company);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var before = await service.GetMyCompanyAsync(company.Id);
        var updated = await service.UpdateMyCompanyAsync(company.Id, new EditCompanyProfileViewModel
        {
            Name = "Beta Logistics",
            TaxNumber = "BG999",
            CompanyType = CompanyType.Forwarder,
            Website = "https://beta.test",
            Notes = "updated"
        });
        var after = await service.GetMyCompanyAsync(company.Id);

        Assert.NotNull(before);
        Assert.True(updated);
        Assert.Equal("Beta Logistics", after!.Name);
        Assert.Equal("Forwarder", after.CompanyType);
        Assert.Equal("https://beta.test", after.Website);
    }

    [Fact]
    public async Task GetAddressOptionsAsync_ReturnsPostalCodeThenCityThenStreetOrder()
    {
        await using var db = TestDbContextFactory.Create();
        db.Addresses.AddRange(
            new Address { Country = "BG", City = "Sofia", Street = "B", PostalCode = "2000" },
            new Address { Country = "BG", City = "Plovdiv", Street = "A", PostalCode = "1000" },
            new Address { Country = "BG", City = "Sofia", Street = "A", PostalCode = "1000" });
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var result = (await service.GetAddressOptionsAsync()).ToList();

        Assert.Equal(3, result.Count);
        Assert.EndsWith("A, Plovdiv, 1000", result[0].Text);
        Assert.EndsWith("A, Sofia, 1000", result[1].Text);
        Assert.EndsWith("B, Sofia, 2000", result[2].Text);
    }

    [Fact]
    public async Task VehicleCrud_UsesCompanyOwnership_AndSoftDelete()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var otherCompany = await SeedCompanyAsync(db, "Carrier B");
        var service = CreateService(db);

        var id = await service.CreateVehicleAsync(company.Id, new VehicleCreateViewModel
        {
            RegistrationNumber = " CA1234TX ",
            VehicleType = VehicleType.Truck,
            Brand = " Volvo ",
            Model = " FH ",
            Status = AssetStatus.Available,
            Notes = " note "
        });

        Assert.NotNull(id);
        var list = (await service.GetVehiclesAsync(company.Id)).ToList();
        var vehicle = Assert.Single(list);
        Assert.Equal("CA1234TX", vehicle.RegistrationNumber);
        Assert.Equal("Volvo", vehicle.Brand);

        var denied = await service.UpdateVehicleAsync(otherCompany.Id, new VehicleEditViewModel { Id = id!.Value, RegistrationNumber = "X", VehicleType = VehicleType.Van, Status = AssetStatus.Maintenance });
        Assert.False(denied);

        var updated = await service.UpdateVehicleAsync(company.Id, new VehicleEditViewModel
        {
            Id = id.Value,
            RegistrationNumber = "CA9999TX",
            VehicleType = VehicleType.Van,
            Brand = " MAN ",
            Model = " TGE ",
            Status = AssetStatus.Maintenance,
            Notes = " updated "
        });
        Assert.True(updated);

        var deleted = await service.DeleteVehicleAsync(company.Id, id.Value);
        Assert.True(deleted);
        Assert.Empty(await service.GetVehiclesAsync(company.Id));
        Assert.True((await db.Vehicles.IgnoreQueryFilters().SingleAsync(x => x.Id == id.Value)).IsDeleted);
    }

    [Fact]
    public async Task DriverCrud_CreatesUpdatesAndDeletesDriver()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var service = CreateService(db);

        var id = await service.CreateDriverAsync(company.Id, new DriverCreateViewModel
        {
            FullName = " Ivan Petrov ",
            LicenseCategory = DriverLicenseCategory.CE,
            LicenseNumber = " LIC-123 ",
            Phone = " 0888 123 456 ",
            Status = AssetStatus.Available,
            Notes = " note "
        });

        var edit = await service.GetDriverForEditAsync(company.Id, id!.Value);
        Assert.NotNull(edit);
        Assert.Equal("Ivan Petrov", edit!.FullName);

        var updated = await service.UpdateDriverAsync(company.Id, new DriverEditViewModel
        {
            Id = id.Value,
            FullName = " Maria Ivanova ",
            LicenseCategory = DriverLicenseCategory.C,
            LicenseNumber = " LIC-999 ",
            Phone = " 0899 ",
            Status = AssetStatus.Assigned,
            Notes = " updated "
        });
        Assert.True(updated);

        Assert.True(await service.DeleteDriverAsync(company.Id, id.Value));
        Assert.Empty(await service.GetDriversAsync(company.Id));
    }

    [Fact]
    public async Task SharedLocations_ReadModels_ReturnActiveSeededEntries()
    {
        await using var db = TestDbContextFactory.Create();

        var activeAddress = new Address
        {
            Country = "Testland",
            City = "AlphaCity",
            Street = "Main"
        };

        var deletedAddress = new Address
        {
            Country = "Testland",
            City = "DeletedCity",
            Street = "Deleted",
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        db.AddRange(activeAddress, deletedAddress);
        await db.SaveChangesAsync();

        var activeLocation = new Location
        {
            Code = "SOF",
            Name = "Sofia Hub",
            LocationType = LocationType.Warehouse,
            AddressId = activeAddress.Id,
            IsActive = true
        };

        var deletedLocation = new Location
        {
            Code = "DEL",
            Name = "Deleted Hub",
            LocationType = LocationType.Warehouse,
            AddressId = deletedAddress.Id,
            IsActive = false,
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        db.AddRange(activeLocation, deletedLocation);
        await db.SaveChangesAsync();

        var activeWarehouse = new Warehouse
        {
            LocationId = activeLocation.Id
        };

        var deletedWarehouse = new Warehouse
        {
            LocationId = deletedLocation.Id,
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        var activeTerminal = new Terminal
        {
            LocationId = activeLocation.Id,
            TerminalCode = "T1",
            IsActive = true
        };

        var deletedTerminal = new Terminal
        {
            LocationId = deletedLocation.Id,
            TerminalCode = "TD",
            IsActive = false,
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        var activeDock = new Dock
        {
            Code = "D1",
            LocationId = activeLocation.Id,
            IsActive = true
        };

        var deletedDock = new Dock
        {
            Code = "DD",
            LocationId = deletedLocation.Id,
            IsActive = false,
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };

        db.AddRange(
            activeWarehouse, deletedWarehouse,
            activeTerminal, deletedTerminal,
            activeDock, deletedDock);

        await db.SaveChangesAsync();

        var service = CreateService(db);

        var addresses = (await service.GetAddressesAsync()).ToList();
        Assert.Contains(addresses, x => x.City == "AlphaCity");

        var locations = (await service.GetLocationsAsync()).ToList();
        Assert.Contains(locations, x => x.Code == "SOF");

        var warehouses = (await service.GetWarehousesAsync()).ToList();
        Assert.Contains(warehouses, x => x.LocationDisplay.Contains("Sofia Hub"));

        var terminals = (await service.GetTerminalsAsync()).ToList();
        Assert.Contains(terminals, x => x.TerminalCode == "T1");

        var docks = (await service.GetDocksAsync()).ToList();
        Assert.Contains(docks, x => x.Code == "D1");
        Assert.StartsWith("Location:", docks.Single(x => x.Code == "D1").ParentDisplay);
    }

    [Fact]
    public async Task RouteCrud_AndStops_RespectOwnershipAndLookupValidation()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var otherCompany = await SeedCompanyAsync(db, "Carrier B");
        var address = new Address { Country = "BG", City = "Sofia", Street = "Main" };
        var location = new Location { Code = "SOF", Name = "Sofia", Address = address, LocationType = LocationType.Warehouse, IsActive = true };
        db.AddRange(address, location);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        var routeId = await service.CreateRouteAsync(company.Id, new RouteCreateViewModel
        {
            RouteCode = " R-001 ",
            Name = " Main Route ",
            TransportMode = TransportMode.Road,
            Notes = " note "
        });

        Assert.NotNull(routeId);
        Assert.Null(await service.CreateRouteStopAsync(company.Id, new RouteStopCreateViewModel
        {
            RouteId = routeId!.Value,
            LocationId = Guid.NewGuid(),
            SequenceNo = 1,
            StopType = RouteStopType.Pickup
        }));

        var stopId = await service.CreateRouteStopAsync(company.Id, new RouteStopCreateViewModel
        {
            RouteId = routeId.Value,
            LocationId = location.Id,
            SequenceNo = 1,
            StopType = RouteStopType.Pickup,
            Notes = " stop "
        });
        Assert.NotNull(stopId);

        Assert.False(await service.UpdateRouteAsync(otherCompany.Id, new RouteEditViewModel { Id = routeId.Value, RouteCode = "X", Name = "Denied", TransportMode = TransportMode.Road }));
        Assert.True(await service.UpdateRouteAsync(company.Id, new RouteEditViewModel { Id = routeId.Value, RouteCode = "R-002", Name = "Updated", TransportMode = TransportMode.Sea, Notes = " updated " }));
        Assert.True(await service.UpdateRouteStopAsync(company.Id, new RouteStopEditViewModel { Id = stopId.Value, RouteId = routeId.Value, LocationId = location.Id, SequenceNo = 2, StopType = RouteStopType.Delivery, Notes = " changed " }));

        var stops = (await service.GetRouteStopsAsync(company.Id)).ToList();
        Assert.Single(stops);
        Assert.Equal(2, stops[0].SequenceNo);
        Assert.Equal("Delivery", stops[0].StopType);

        Assert.True(await service.DeleteRouteStopAsync(company.Id, stopId.Value));
        Assert.True(await service.DeleteRouteAsync(company.Id, routeId.Value));
        Assert.Empty(await service.GetRoutesAsync(company.Id));
    }

    [Fact]
    public async Task RoutePlanCrud_AndPlanStop_ValidateOwnershipForForeignRoute()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var otherCompany = await SeedCompanyAsync(db, "Carrier B");
        var address = new Address { Country = "BG", City = "Sofia", Street = "Main" };
        var location = new Location { Code = "SOF", Name = "Sofia", Address = address, LocationType = LocationType.Warehouse, IsActive = true };
        var route = new Route { CompanyId = company.Id, RouteCode = "R-1", Name = "Main", TransportMode = TransportMode.Road };
        var foreignRoute = new Route { CompanyId = otherCompany.Id, RouteCode = "R-2", Name = "Foreign", TransportMode = TransportMode.Road };
        db.AddRange(address, location, route, foreignRoute);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        Assert.Null(await service.CreateRoutePlanAsync(company.Id, new RoutePlanCreateViewModel { RouteId = foreignRoute.Id, PlanDateUtc = DateTime.UtcNow }));

        var planId = await service.CreateRoutePlanAsync(company.Id, new RoutePlanCreateViewModel
        {
            RouteId = route.Id,
            PlanDateUtc = new DateTime(2026, 4, 18),
            Status = RoutePlanStatus.Planned,
            PlanReference = " PLAN-1 ",
            Notes = " note "
        });
        Assert.NotNull(planId);

        var planStopId = await service.CreateRoutePlanStopAsync(company.Id, new RoutePlanStopCreateViewModel
        {
            RoutePlanId = planId!.Value,
            LocationId = location.Id,
            SequenceNo = 1,
            StopType = RouteStopType.Pickup,
            PlannedArrivalUtc = new DateTime(2026, 4, 18, 8, 0, 0, DateTimeKind.Utc),
            Notes = " plan stop "
        });
        Assert.NotNull(planStopId);

        var edit = await service.GetRoutePlanForEditAsync(company.Id, planId.Value);
        Assert.NotNull(edit);
        Assert.Equal(RoutePlanStatus.Planned, edit!.Status);

        Assert.True(await service.UpdateRoutePlanAsync(company.Id, new RoutePlanEditViewModel { Id = planId.Value, RouteId = route.Id, PlanDateUtc = edit.PlanDateUtc.AddDays(1), Status = RoutePlanStatus.Completed, PlanReference = "PLAN-2", Notes = "updated" }));
        Assert.True(await service.DeleteRoutePlanStopAsync(company.Id, planStopId.Value));
        Assert.True(await service.DeleteRoutePlanAsync(company.Id, planId.Value));
    }


    [Fact]
    public async Task BranchCrud_KeepsSingleHeadOffice_AndPromotesNextOnDelete()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var address = new Address { Country = "BG", City = "Sofia", Street = "Main", PostalCode = "1000" };
        db.Add(address);
        await db.SaveChangesAsync();
        var service = CreateService(db);

        await service.CreateBranchAsync(company.Id, new CompanyBranchCreateViewModel
        {
            Name = "Head Office",
            AddressId = address.Id,
            BranchCode = "HQ",
            IsHeadOffice = false,
            IsActive = true
        });

        await service.CreateBranchAsync(company.Id, new CompanyBranchCreateViewModel
        {
            Name = "Branch Office",
            AddressId = address.Id,
            BranchCode = "BR1",
            IsHeadOffice = true,
            IsActive = true
        });

        var branches = (await service.GetMyBranchesAsync(company.Id)).ToList();
        Assert.Equal(2, branches.Count);
        Assert.Equal("Branch Office", branches[0].Name);
        Assert.True(branches[0].IsHeadOffice);
        Assert.False(branches[1].IsHeadOffice);

        var firstBranch = await service.GetBranchForEditAsync(company.Id, branches[1].Id);
        Assert.NotNull(firstBranch);

        Assert.True(await service.DeleteBranchAsync(company.Id, branches[0].Id));
        var remaining = Assert.Single(await service.GetMyBranchesAsync(company.Id));
        Assert.True(remaining.IsHeadOffice);
        Assert.Equal("Head Office", remaining.Name);
    }

    [Fact]
    public async Task ContactCrud_KeepsSinglePrimary_AndPromotesAlphabeticallyOnDelete()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var service = CreateService(db);

        await service.CreateContactAsync(company.Id, new CompanyContactCreateViewModel
        {
            FullName = "Zara Contact",
            Email = "z@test.com",
            Phone = "111",
            RoleTitle = "Sales",
            IsPrimary = false
        });

        await service.CreateContactAsync(company.Id, new CompanyContactCreateViewModel
        {
            FullName = "Anna Contact",
            Email = "a@test.com",
            Phone = "222",
            RoleTitle = "Ops",
            IsPrimary = true
        });

        var contacts = (await service.GetMyContactsAsync(company.Id)).ToList();
        Assert.Equal(2, contacts.Count);
        Assert.True(contacts[0].IsPrimary);
        Assert.Equal("Anna Contact", contacts[0].FullName);

        var edit = await service.GetContactForEditAsync(company.Id, contacts[1].Id);
        Assert.NotNull(edit);
        Assert.True(await service.UpdateContactAsync(company.Id, new CompanyContactEditViewModel
        {
            Id = contacts[1].Id,
            FullName = "Zara Contact",
            Email = "z@test.com",
            Phone = "333",
            RoleTitle = "Ops Lead",
            IsPrimary = true
        }));

        var refreshed = (await service.GetMyContactsAsync(company.Id)).ToList();
        Assert.True(refreshed.Single(x => x.FullName == "Zara Contact").IsPrimary);
        Assert.False(refreshed.Single(x => x.FullName == "Anna Contact").IsPrimary);

        var primaryId = refreshed.Single(x => x.FullName == "Zara Contact").Id;
        Assert.True(await service.DeleteContactAsync(company.Id, primaryId));
        var remaining = Assert.Single(await service.GetMyContactsAsync(company.Id));
        Assert.True(remaining.IsPrimary);
        Assert.Equal("Anna Contact", remaining.FullName);
    }

    [Fact]
    public async Task CapabilityCrud_RejectsInvalidDatesAndDuplicateTypes()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var service = CreateService(db);

        var invalid = await service.CreateCapabilityAsync(company.Id, new CompanyCapabilityCreateViewModel
        {
            CapabilityType = CompanyCapabilityType.CustomsBroker,
            ValidFromUtc = new DateTime(2026, 4, 20),
            ValidToUtc = new DateTime(2026, 4, 18)
        });
        Assert.False(invalid);

        var created = await service.CreateCapabilityAsync(company.Id, new CompanyCapabilityCreateViewModel
        {
            CapabilityType = CompanyCapabilityType.CustomsBroker,
            IsEnabled = true,
            ValidFromUtc = new DateTime(2026, 4, 18),
            ValidToUtc = new DateTime(2026, 4, 20),
            Notes = "ok"
        });
        Assert.True(created);
        Assert.False(await service.CreateCapabilityAsync(company.Id, new CompanyCapabilityCreateViewModel
        {
            CapabilityType = CompanyCapabilityType.CustomsBroker,
            IsEnabled = true
        }));

        var capability = Assert.Single(await service.GetMyCapabilitiesAsync(company.Id));
        var edit = await service.GetCapabilityForEditAsync(company.Id, capability.Id);
        Assert.NotNull(edit);
        Assert.True(await service.UpdateCapabilityAsync(company.Id, new CompanyCapabilityEditViewModel
        {
            Id = capability.Id,
            CapabilityType = CompanyCapabilityType.RoadCarrier,
            IsEnabled = false,
            ValidFromUtc = new DateTime(2026, 4, 18),
            ValidToUtc = new DateTime(2026, 4, 21),
            Notes = "changed"
        }));

        var updated = Assert.Single(await service.GetMyCapabilitiesAsync(company.Id));
        Assert.Equal("RoadCarrier", updated.CapabilityType);
        Assert.False(updated.IsEnabled);
        Assert.True(await service.DeleteCapabilityAsync(company.Id, capability.Id));
        Assert.Empty(await service.GetMyCapabilitiesAsync(company.Id));
    }


    [Fact]
    public async Task OrderCrud_CreateEditDelete_AndDetailsFlow_WorkForDraftOrder()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var pickup = new Address { Country = "BG", City = "Sofia", Street = "Pickup", PostalCode = "1000" };
        var delivery = new Address { Country = "DE", City = "Berlin", Street = "Delivery", PostalCode = "2000" };
        db.AddRange(pickup, delivery);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var orderId = await service.CreateOrderAsync(company.Id, new CompanyOrderCreateViewModel
        {
            PickupAddressId = pickup.Id,
            DeliveryAddressId = delivery.Id,
            Priority = OrderPriority.High,
            RequestedPickupDateUtc = new DateTime(2026, 4, 20),
            CustomerReference = " REF-1 ",
            Notes = " draft order "
        });

        var edit = await service.GetOrderForEditAsync(company.Id, orderId);
        Assert.NotNull(edit);
        Assert.Equal("ORD-TEST", edit!.OrderNo);
        Assert.Equal(OrderPriority.High, edit.Priority);

        var updated = await service.UpdateOrderAsync(company.Id, new CompanyOrderEditViewModel
        {
            Id = orderId,
            PickupAddressId = delivery.Id,
            DeliveryAddressId = pickup.Id,
            Priority = OrderPriority.Critical,
            RequestedPickupDateUtc = new DateTime(2026, 4, 21),
            CustomerReference = "REF-2",
            Notes = "updated"
        });
        Assert.True(updated);

        var details = await service.GetOrderDetailsAsync(company.Id, orderId);
        Assert.NotNull(details);
        Assert.Equal("Draft", details!.Status);
        Assert.True(details.CanEdit);
        Assert.True(details.CanDelete);
        Assert.True(details.CanSubmit);
        Assert.True(details.CanManageLines);

        Assert.True(await service.DeleteOrderAsync(company.Id, orderId));
        Assert.Empty((await service.GetMyOrdersAsync(company.Id)).Orders);
    }

    [Fact]
    public async Task OrderLineCrud_RejectsDuplicateLineNumbers_AndBlocksForeignOwnership()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var otherCompany = await SeedCompanyAsync(db, "Carrier B");
        var order = new Order
        {
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            OrderNo = "ORD-LINES",
            Status = OrderStatus.Draft,
            Priority = OrderPriority.Normal,
            IsActive = true
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var createModel = await service.GetCreateOrderLineModelAsync(company.Id, order.Id);
        Assert.NotNull(createModel);
        Assert.Equal(1, createModel!.LineNo);

        Assert.True(await service.CreateOrderLineAsync(company.Id, new CompanyOrderLineCreateViewModel
        {
            OrderId = order.Id,
            LineNo = 1,
            Description = "Item 1",
            Quantity = 2,
            QuantityUnit = "pcs",
            GrossWeightKg = 10,
            VolumeCbm = 1.2m,
            HsCode = "1000",
            OriginCountry = "BG"
        }));

        Assert.False(await service.CreateOrderLineAsync(company.Id, new CompanyOrderLineCreateViewModel
        {
            OrderId = order.Id,
            LineNo = 1,
            Description = "Dup"
        }));

        var line = await db.OrderLines.SingleAsync(x => x.OrderId == order.Id);
        Assert.Null(await service.GetOrderLineForEditAsync(otherCompany.Id, line.Id));

        Assert.True(await service.UpdateOrderLineAsync(company.Id, new CompanyOrderLineEditViewModel
        {
            Id = line.Id,
            OrderId = order.Id,
            LineNo = 2,
            Description = "Item 1 updated",
            Quantity = 3,
            QuantityUnit = "boxes",
            GrossWeightKg = 12,
            VolumeCbm = 1.5m,
            IsDangerousGoods = true,
            HsCode = "2000",
            OriginCountry = "DE"
        }));

        var edited = await service.GetOrderLineForEditAsync(company.Id, line.Id);
        Assert.NotNull(edited);
        Assert.Equal(2, edited!.LineNo);
        Assert.True(edited.IsDangerousGoods);

        Assert.True(await service.DeleteOrderLineAsync(company.Id, line.Id));
        Assert.Empty(await db.OrderLines.Where(x => x.OrderId == order.Id).ToListAsync());
    }

    [Fact]
    public async Task OrderStatusLifecycle_SubmitConfirmProgressComplete_AndCancelRules()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var order = new Order
        {
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            OrderNo = "ORD-STATUS",
            Status = OrderStatus.Draft,
            Priority = OrderPriority.Normal,
            IsActive = true,
            Lines = new List<OrderLine>
            {
                new() { LineNo = 1, Description = "Line" }
            }
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        Assert.True(await service.SubmitOrderAsync(company.Id, order.Id));
        Assert.True(await service.ConfirmOrderAsync(company.Id, order.Id));
        Assert.True(await service.MarkOrderInProgressAsync(company.Id, order.Id));
        Assert.True(await service.CompleteOrderAsync(company.Id, order.Id));
        Assert.False(await service.CancelOrderAsync(company.Id, order.Id, "too late"));

        var details = await service.GetOrderDetailsAsync(company.Id, order.Id);
        Assert.NotNull(details);
        Assert.Equal("Completed", details!.Status);
        Assert.False(details.CanCancel);
        Assert.False(details.CanEdit);
        Assert.True(details.StatusHistory.Count >= 4);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesCustomReason_AndRejectsSecondCancel()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var order = new Order
        {
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            OrderNo = "ORD-CANCEL",
            Status = OrderStatus.Confirmed,
            Priority = OrderPriority.Normal,
            IsActive = true
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        Assert.True(await service.CancelOrderAsync(company.Id, order.Id, "client asked"));
        Assert.False(await service.CancelOrderAsync(company.Id, order.Id, "again"));

        var history = await db.OrderStatusHistories.Where(x => x.OrderId == order.Id).OrderByDescending(x => x.ChangedAtUtc).FirstAsync();
        Assert.Equal(OrderStatus.Cancelled, history.NewStatus);
        Assert.Equal("client asked", history.Reason);
    }

    [Fact]
    public async Task ShipmentCrud_CreateEditDelete_AndListFlow_TrimsFields()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var sender = new Address { Country = "BG", City = "Sofia", Street = "Sender" };
        var receiver = new Address { Country = "DE", City = "Berlin", Street = "Receiver" };
        var order = new Order { CustomerCompanyId = company.Id, CustomerCompany = company, OrderNo = "ORD-SHIP", Status = OrderStatus.Confirmed, Priority = OrderPriority.Normal, IsActive = true };
        db.AddRange(sender, receiver, order);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var shipmentId = await service.CreateShipmentAsync(company.Id, new CompanyShipmentCreateViewModel
        {
            OrderId = order.Id,
            SenderAddressId = sender.Id,
            ReceiverAddressId = receiver.Id,
            Status = ShipmentStatus.Created,
            PrimaryMode = TransportMode.Road,
            Currency = " eur ",
            CustomerReference = " ref ",
            Notes = " note ",
            DeclaredValue = 123.45m
        });

        var edit = await service.GetShipmentForEditAsync(company.Id, shipmentId);
        Assert.NotNull(edit);
        Assert.Equal("EUR", edit!.Currency);
        Assert.Equal("ref", edit.CustomerReference);
        Assert.Equal("note", edit.Notes);

        Assert.True(await service.UpdateShipmentAsync(company.Id, new CompanyShipmentEditViewModel
        {
            Id = shipmentId,
            OrderId = order.Id,
            SenderAddressId = receiver.Id,
            ReceiverAddressId = sender.Id,
            PrimaryMode = TransportMode.Sea,
            Incoterm = Incoterm.CIF,
            DeclaredValue = 200,
            Currency = " usd ",
            CustomerReference = " ref-2 ",
            Notes = " updated "
        }));

        var list = (await service.GetMyShipmentsAsync(company.Id)).Shipments;
        var single = Assert.Single(list);
        Assert.Equal("SHP-TEST", single.ShipmentNo);
        Assert.Equal("Sea", single.PrimaryMode);
        Assert.Equal(0, single.TrackingEventsCount);

        Assert.True(await service.DeleteShipmentAsync(company.Id, shipmentId));
        Assert.Empty((await service.GetMyShipmentsAsync(company.Id)).Shipments);
    }

    [Fact]
    public async Task ShipmentStatus_UpdateShipmentStatus_ValidatesTransition_AndCreatesHistory()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var shipment = new Shipment
        {
            ShipmentNo = "SHP-STATUS",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            Status = ShipmentStatus.Created,
            PrimaryMode = TransportMode.Road
        };
        db.Shipments.Add(shipment);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        Assert.False(await service.UpdateShipmentStatusAsync(company.Id, shipment.Id, "invalid-status"));
        Assert.True(await service.UpdateShipmentStatusAsync(company.Id, shipment.Id, ShipmentStatus.InTransit.ToString(), "departed"));
        Assert.False(await service.UpdateShipmentStatusAsync(company.Id, shipment.Id, ShipmentStatus.InTransit.ToString()));
        Assert.True(await service.UpdateShipmentStatusAsync(company.Id, shipment.Id, ShipmentStatus.Delivered.ToString()));
        Assert.False(await service.UpdateShipmentStatusAsync(company.Id, shipment.Id, ShipmentStatus.Cancelled.ToString()));

        var histories = await db.ShipmentStatusHistories.Where(x => x.ShipmentId == shipment.Id).OrderBy(x => x.ChangedAtUtc).ToListAsync();
        Assert.True(histories.Count >= 2);
        Assert.Equal("departed", histories[0].Reason);
        Assert.Equal(ShipmentStatus.Delivered, (await db.Shipments.SingleAsync(x => x.Id == shipment.Id)).Status);
    }

    [Fact]
    public async Task ShipmentDetails_ReturnsTrackingPackageCargoReferenceAndTagData()
    {
        await using var db = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(db, "Carrier A");
        var sender = new Address { Country = "BG", City = "Sofia", Street = "Sender" };
        var receiver = new Address { Country = "DE", City = "Berlin", Street = "Receiver" };
        var locAddress = new Address { Country = "BG", City = "Sofia", Street = "Hub" };
        var location = new Location { Code = "SOF", Name = "Sofia Hub", Address = locAddress, LocationType = LocationType.Warehouse, IsActive = true };
        var shipment = new Shipment
        {
            ShipmentNo = "SHP-DETAILS",
            CustomerCompanyId = company.Id,
            CustomerCompany = company,
            SenderAddress = sender,
            ReceiverAddress = receiver,
            Status = ShipmentStatus.InTransit,
            PrimaryMode = TransportMode.Road,
            Currency = "EUR",
            CustomerReference = "REF-1"
        };
        var leg = new ShipmentLeg { Shipment = shipment, LegNo = 1, Mode = TransportMode.Road, OriginLocation = location, DestinationLocation = location, Status = LegStatus.Planned };
        var tracking = new TrackingEvent { Shipment = shipment, EventType = TrackingEventType.Arrived, EventTimeUtc = new DateTime(2026,4,18,8,0,0,DateTimeKind.Utc), Location = location, Details = "Arrived", Source = "manual" };
        var package = new Package { Shipment = shipment, PackageNo = "PKG-1", PackageType = PackageType.Box, WeightKg = 5, VolumeCbm = 1 };
        var packageItem = new PackageItem { Package = package, Description = "Inner item", Quantity = 1 };
        var cargo = new CargoItem { Shipment = shipment, Description = "Cargo", CargoItemType = CargoItemType.HighValue, Quantity = 2, UnitOfMeasure = "pcs", IsFragile = true };
        var reference = new ShipmentReference { Shipment = shipment, ReferenceType = ShipmentReferenceType.BookingReference, ReferenceValue = "BOOK-1" };
        var tag = new ShipmentTag { Shipment = shipment, TagType = ShipmentTagType.HighValue, Notes = "Critical" };
        db.AddRange(sender, receiver, locAddress, location, shipment, leg, tracking, package, packageItem, cargo, reference, tag);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var details = await service.GetShipmentDetailsAsync(company.Id, shipment.Id);

        Assert.NotNull(details);
        Assert.Equal("SHP-DETAILS", details!.ShipmentNo);
        Assert.Single(details.Legs);
        Assert.Single(details.TrackingEvents);
        Assert.Single(details.Packages);
        Assert.Single(details.CargoItems);
        Assert.Single(details.References);
        Assert.Single(details.Tags);
        Assert.Equal("Sofia Hub", details.TrackingEvents[0].Location);
    }

    private static CompanyPortalService CreateService(LogisticManagementApp.Infrastructure.Persistence.LogisticAppDbContext db)
    {
        var generator = new Mock<ISequentialNumberGenerator>();
        generator.Setup(x => x.GenerateOrderNoAsync()).ReturnsAsync("ORD-TEST");
        generator.Setup(x => x.GenerateShipmentNoAsync()).ReturnsAsync("SHP-TEST");
        generator.Setup(x => x.GeneratePackageNoAsync()).ReturnsAsync("PKG-TEST");
        return new CompanyPortalService(db, generator.Object);
    }

    private static async Task<Company> SeedCompanyAsync(LogisticManagementApp.Infrastructure.Persistence.LogisticAppDbContext db, string name)
    {
        var company = new Company { Name = name, CompanyType = CompanyType.Carrier, IsActive = true };
        db.Companies.Add(company);
        await db.SaveChangesAsync();
        return company;
    }
}
