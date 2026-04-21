using LogisticManagementApp.Domain.Enums.Billing;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Models.CompanyPortal.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Controllers
{
    public partial class CompanyController
    {
        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                return Forbid();
            }

            var companyName = await _dbContext.Companies
                .Where(x => x.Id == companyId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "Company";

            return View("~/Views/CompanyPortal/Reports.cshtml", new CompanyReportsIndexViewModel
            {
                CompanyName = companyName
            });
        }

        [HttpGet]
        public async Task<IActionResult> OperationalReport()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                return Forbid();
            }

            var companyName = await _dbContext.Companies
                .Where(x => x.Id == companyId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "Company";

            var recentOrders = await _dbContext.Orders
                .Where(x => x.CustomerCompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(10)
                .Select(x => new CompanyOperationalOrderRowViewModel
                {
                    OrderNo = x.OrderNo,
                    Status = x.Status.ToString(),
                    Priority = x.Priority.ToString(),
                    PickupDate = x.RequestedPickupDateUtc.HasValue ? x.RequestedPickupDateUtc.Value.ToString("dd.MM.yyyy") : "-",
                    Reference = string.IsNullOrWhiteSpace(x.CustomerReference) ? "-" : x.CustomerReference
                })
                .ToListAsync();

            var model = new CompanyOperationalReportViewModel
            {
                CompanyName = companyName,
                GeneratedAtUtc = DateTime.UtcNow,
                OrdersCount = await _dbContext.Orders.CountAsync(x => x.CustomerCompanyId == companyId.Value),
                ShipmentsCount = await _dbContext.Shipments.CountAsync(x => x.CustomerCompanyId == companyId.Value),
                ActiveShipmentsCount = await _dbContext.Shipments.CountAsync(x => x.CustomerCompanyId == companyId.Value && x.Status != ShipmentStatus.Delivered && x.Status != ShipmentStatus.Cancelled),
                InvoicesCount = await _dbContext.Invoices.CountAsync(x => x.BillToCompanyId == companyId.Value),
                RoutesCount = await _dbContext.Routes.CountAsync(x => x.CompanyId == companyId.Value),
                BookingsCount = await _dbContext.Bookings.CountAsync(x => x.CarrierCompanyId == companyId.Value || (x.Shipment != null && x.Shipment.CustomerCompanyId == companyId.Value)),
                RecentOrders = recentOrders
            };

            ViewData["Title"] = "Company Operational Report";
            return View("~/Views/CompanyPortal/OperationalReport.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AssetsReport()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                return Forbid();
            }

            var companyName = await _dbContext.Companies
                .Where(x => x.Id == companyId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "Company";

            var latestAssets = new List<CompanyAssetRowViewModel>();

            latestAssets.AddRange(await _dbContext.Vessels
                .Where(x => x.CompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(3)
                .Select(x => new CompanyAssetRowViewModel { Mode = "Sea", Identifier = x.ImoNumber ?? x.Name, Type = x.VesselType.ToString(), Status = x.Status.ToString() })
                .ToListAsync());

            latestAssets.AddRange(await _dbContext.Vehicles
                .Where(x => x.CompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(3)
                .Select(x => new CompanyAssetRowViewModel { Mode = "Road", Identifier = x.RegistrationNumber, Type = x.VehicleType.ToString(), Status = x.Status.ToString() })
                .ToListAsync());

            latestAssets.AddRange(await _dbContext.Aircraft
                .Where(x => x.CompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(3)
                .Select(x => new CompanyAssetRowViewModel { Mode = "Air", Identifier = x.TailNumber, Type = x.AircraftType.ToString(), Status = x.Status.ToString() })
                .ToListAsync());

            latestAssets.AddRange(await _dbContext.Trains
                .Where(x => x.CompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(3)
                .Select(x => new CompanyAssetRowViewModel { Mode = "Rail", Identifier = x.TrainNumber, Type = x.TrainType.ToString(), Status = x.Status.ToString() })
                .ToListAsync());

            latestAssets.AddRange(await _dbContext.Containers
                .Where(x => x.OwnerCompanyId == companyId.Value)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(3)
                .Select(x => new CompanyAssetRowViewModel { Mode = "Cargo Unit", Identifier = x.ContainerNumber, Type = x.ContainerType.ToString(), Status = x.Status.ToString() })
                .ToListAsync());

            var model = new CompanyAssetsReportViewModel
            {
                CompanyName = companyName,
                GeneratedAtUtc = DateTime.UtcNow,
                VesselsCount = await _dbContext.Vessels.CountAsync(x => x.CompanyId == companyId.Value),
                VehiclesCount = await _dbContext.Vehicles.CountAsync(x => x.CompanyId == companyId.Value),
                AircraftCount = await _dbContext.Aircraft.CountAsync(x => x.CompanyId == companyId.Value),
                TrainsCount = await _dbContext.Trains.CountAsync(x => x.CompanyId == companyId.Value),
                ContainersCount = await _dbContext.Containers.CountAsync(x => x.OwnerCompanyId == companyId.Value),
                TripsCount = await _dbContext.Trips.CountAsync(x => x.Vehicle != null && x.Vehicle.CompanyId == companyId.Value),
                VoyagesCount = await _dbContext.Voyages.CountAsync(x => x.Vessel.CompanyId == companyId.Value),
                FlightsCount = await _dbContext.Flights.CountAsync(x => x.Aircraft.CompanyId == companyId.Value),
                RailMovementsCount = await _dbContext.RailMovements.CountAsync(x => x.Train != null && x.Train.CompanyId == companyId.Value),
                LatestAssets = latestAssets.Take(10).ToList()
            };

            ViewData["Title"] = "Company Assets Report";
            return View("~/Views/CompanyPortal/AssetsReport.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> FinanceReport()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                return Forbid();
            }

            var companyName = await _dbContext.Companies
                .Where(x => x.Id == companyId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "Company";

            var latestInvoices = await _dbContext.Invoices
                .Where(x => x.BillToCompanyId == companyId.Value)
                .OrderByDescending(x => x.IssueDateUtc)
                .Take(8)
                .Select(x => new CompanyFinanceInvoiceRowViewModel
                {
                    InvoiceNo = x.InvoiceNo,
                    IssueDate = x.IssueDateUtc.ToString("dd.MM.yyyy"),
                    Status = x.Status.ToString(),
                    Currency = x.Currency,
                    Total = x.TotalAmount.ToString("0.##")
                })
                .ToListAsync();

            var latestPayments = await _dbContext.Payments
                .Where(x => x.Invoice.BillToCompanyId == companyId.Value)
                .OrderByDescending(x => x.PaymentDateUtc)
                .Take(8)
                .Select(x => new CompanyFinancePaymentRowViewModel
                {
                    InvoiceNo = x.Invoice.InvoiceNo,
                    PaymentDate = x.PaymentDateUtc.ToString("dd.MM.yyyy"),
                    Status = x.Status.ToString(),
                    Currency = x.Currency,
                    Amount = x.Amount.ToString("0.##")
                })
                .ToListAsync();

            var model = new CompanyFinanceReportViewModel
            {
                CompanyName = companyName,
                GeneratedAtUtc = DateTime.UtcNow,
                InvoicesCount = await _dbContext.Invoices.CountAsync(x => x.BillToCompanyId == companyId.Value),
                CreditNotesCount = await _dbContext.CreditNotes.CountAsync(x => x.BillToCompanyId == companyId.Value),
                PaymentsCount = await _dbContext.Payments.CountAsync(x => x.Invoice.BillToCompanyId == companyId.Value),
                TotalInvoiced = await _dbContext.Invoices.Where(x => x.BillToCompanyId == companyId.Value && x.Status != InvoiceStatus.Cancelled).SumAsync(x => (decimal?)x.TotalAmount) ?? 0m,
                TotalTax = await _dbContext.Invoices.Where(x => x.BillToCompanyId == companyId.Value && x.Status != InvoiceStatus.Cancelled).SumAsync(x => (decimal?)x.TaxAmount) ?? 0m,
                TotalPaid = await _dbContext.Payments.Where(x => x.Invoice.BillToCompanyId == companyId.Value && x.Status == PaymentStatus.Completed).SumAsync(x => (decimal?)x.Amount) ?? 0m,
                CreditIssued = await _dbContext.CreditNotes.Where(x => x.BillToCompanyId == companyId.Value && x.Status != CreditNoteStatus.Cancelled).SumAsync(x => (decimal?)x.TotalAmount) ?? 0m,
                LatestInvoices = latestInvoices,
                LatestPayments = latestPayments
            };

            ViewData["Title"] = "Company Finance Report";
            return View("~/Views/CompanyPortal/FinanceReport.cshtml", model);
        }
    }
}
