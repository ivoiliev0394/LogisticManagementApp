using Microsoft.EntityFrameworkCore;
using LogisticManagementApp.Applicationn.Services.AdminPortal;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Tests.Helpers;

namespace LogisticManagementApp.Tests.Services;

public class AdminCrudServiceTests
{

    [Fact]
    public void GetEntityDetails_AndForms_WorkForExistingEntity()
    {
        using var db = TestDbContextFactory.Create();
        var company = new Company { Name = "Details Co", CompanyType = CompanyType.Carrier, IsActive = true };
        db.Companies.Add(company);
        db.SaveChanges();

        var service = new AdminCrudService(db);
        var key = $"Id={Uri.EscapeDataString(company.Id.ToString())}";

        var details = service.GetEntityDetails("Company", key);
        var editForm = service.GetEditForm("Company", key);
        var createForm = service.GetCreateForm("Company");

        Assert.NotNull(details);
        Assert.Equal("Details Co", details!.Fields.Single(x => x.Name == "Name").Value);
        Assert.NotNull(editForm);
        Assert.True(editForm!.IsEdit);
        Assert.False(createForm.IsEdit);
        Assert.Contains(createForm.Fields, x => x.Name == "Name");
    }

    [Fact]
    public void CreateEntity_CreatesCompany_AndGetEntityGroupsContainsCompanyGroup()
    {
        using var db = TestDbContextFactory.Create();
        var service = new AdminCrudService(db);

        var key = service.CreateEntity("Company", new Dictionary<string, string?>
        {
            ["Name"] = "Created Co",
            ["CompanyType"] = nameof(CompanyType.Carrier),
            ["IsActive"] = "true"
        });

        Assert.Contains("Id=", key);
        Assert.Contains(db.Companies, x => x.Name == "Created Co");
        Assert.Contains(service.GetEntityGroups().SelectMany(x => x.Entities), x => x.Name == "Company");
    }

    [Fact]
    public void GetEntityList_IncludesSoftDeletedRows_AndSupportsSearchAndColumnFilters()
    {
        using var db = TestDbContextFactory.Create();
        db.Companies.AddRange(
            new Company { Name = "Alpha Logistics", CompanyType = CompanyType.Carrier, IsActive = true },
            new Company { Name = "Beta Freight", CompanyType = CompanyType.Forwarder, IsActive = true },
            new Company { Name = "Gamma Cargo", CompanyType = CompanyType.Other, IsActive = false });
        db.SaveChanges();
        db.ChangeTracker.Clear();

        var service = new AdminCrudService(db);
        var betaId = db.Companies.AsNoTracking().Single(x => x.Name == "Beta Freight").Id;
        var betaKey = $"Id={Uri.EscapeDataString(betaId.ToString())}";
        Assert.True(service.DeleteEntity("Company", betaKey));
        db.ChangeTracker.Clear();

        var bySearch = service.GetEntityList("Company", searchTerm: "beta");
        var byColumn = service.GetEntityList("Company", filterColumn: "IsActive", filterValue: "Не");

        var deletedRow = Assert.Single(bySearch.Rows);
        Assert.True(deletedRow.IsDeleted);
        Assert.False(deletedRow.CanDelete);

        var inactiveRow = Assert.Single(byColumn.Rows);
        Assert.Equal("Gamma Cargo", inactiveRow.Values["Name"]);
    }

    [Fact]
    public void DeleteEntity_PerformsSoftDelete_AndSecondDeleteReturnsTrueWithoutBreaking()
    {
        using var db = TestDbContextFactory.Create();
        var company = new Company { Name = "Delete Me", CompanyType = CompanyType.Carrier, IsActive = true };
        db.Companies.Add(company);
        db.SaveChanges();
        db.ChangeTracker.Clear();

        var service = new AdminCrudService(db);
        var key = $"Id={Uri.EscapeDataString(company.Id.ToString())}";

        var firstDelete = service.DeleteEntity("Company", key);
        db.ChangeTracker.Clear();
        var secondDelete = service.DeleteEntity("Company", key);

        Assert.True(firstDelete);
        Assert.True(secondDelete);

        var saved = db.Companies.IgnoreQueryFilters().AsNoTracking().Single(x => x.Id == company.Id);
        Assert.True(saved.IsDeleted);
        Assert.NotNull(saved.DeletedAtUtc);
    }

    [Fact]
    public void UpdateEntity_CanRestoreSoftDeletedRow_WhenIsDeletedIsSetToFalse()
    {
        using var db = TestDbContextFactory.Create();
        var company = new Company
        {
            Name = "Restore Me",
            CompanyType = CompanyType.Carrier,
            IsActive = true
        };
        db.Companies.Add(company);
        db.SaveChanges();
        db.ChangeTracker.Clear();

        var service = new AdminCrudService(db);
        var key = $"Id={Uri.EscapeDataString(company.Id.ToString())}";
        Assert.True(service.DeleteEntity("Company", key));
        db.ChangeTracker.Clear();

        var updated = service.UpdateEntity("Company", key, new Dictionary<string, string?>
        {
            ["Name"] = "Restored Company",
            ["CompanyType"] = nameof(CompanyType.Forwarder),
            ["IsActive"] = "true",
            ["IsDeleted"] = "false"
        });

        Assert.True(updated);

        var saved = db.Companies.IgnoreQueryFilters().AsNoTracking().Single(x => x.Id == company.Id);
        Assert.False(saved.IsDeleted);
        Assert.Equal("Restored Company", saved.Name);
        Assert.Equal(CompanyType.Forwarder, saved.CompanyType);
    }
}
