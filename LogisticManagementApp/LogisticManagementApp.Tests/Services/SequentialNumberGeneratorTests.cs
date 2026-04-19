using LogisticManagementApp.Applicationn.Services;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Tests.Helpers;

namespace LogisticManagementApp.Tests.Services;

public class SequentialNumberGeneratorTests
{
    [Fact]
    public async Task GenerateOrderNoAsync_ReturnsFirstNumber_WhenNoOrdersExist()
    {
        await using var db = TestDbContextFactory.Create();
        var generator = new SequentialNumberGenerator(db);

        var result = await generator.GenerateOrderNoAsync();

        Assert.Equal($"ORD-{DateTime.UtcNow.Year}-000001", result);
    }

    [Fact]
    public async Task GenerateShipmentNoAsync_SkipsUsedNumbers_AndIgnoresMalformedValues()
    {
        await using var db = TestDbContextFactory.Create();
        var company = new Company { Name = "Company", CompanyType = CompanyType.Carrier, IsActive = true };
        var year = DateTime.UtcNow.Year;

        db.Companies.Add(company);
        db.Shipments.AddRange(
            new Shipment { ShipmentNo = $"SHP-{year}-000001", CustomerCompany = company },
            new Shipment { ShipmentNo = $"SHP-{year}-000002", CustomerCompany = company },
            new Shipment { ShipmentNo = $"SHP-{year}-000004", CustomerCompany = company },
            new Shipment { ShipmentNo = $"SHP-{year}-ABC", CustomerCompany = company },
            new Shipment { ShipmentNo = $"SHP-{year - 1}-000003", CustomerCompany = company });

        await db.SaveChangesAsync();

        var generator = new SequentialNumberGenerator(db);

        var result = await generator.GenerateShipmentNoAsync();

        Assert.Equal($"SHP-{year}-000003", result);
    }

    [Fact]
    public async Task GeneratePackageNoAsync_ConsidersSoftDeletedPackages_BecauseQueryFiltersAreIgnored()
    {
        await using var db = TestDbContextFactory.Create();
        var year = DateTime.UtcNow.Year;

        var company = new Company { Name = "Company", CompanyType = CompanyType.Carrier, IsActive = true };
        var shipment = new Shipment { ShipmentNo = $"SHP-{year}-000010", CustomerCompany = company };

        db.AddRange(company, shipment);
        db.Packages.AddRange(
            new Package { PackageNo = $"PKG-{year}-000001", Shipment = shipment },
            new Package { PackageNo = $"PKG-{year}-000002", Shipment = shipment, IsDeleted = true, DeletedAtUtc = DateTime.UtcNow });

        await db.SaveChangesAsync();

        var generator = new SequentialNumberGenerator(db);

        var result = await generator.GeneratePackageNoAsync();

        Assert.Equal($"PKG-{year}-000003", result);
    }
}
