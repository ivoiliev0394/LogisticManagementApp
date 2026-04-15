using LogisticManagementApp.Applicationn.Interfaces;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.Home;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services
{
    public class HomeService : IHomeService
    {
        private readonly LogisticAppDbContext _db;

        public HomeService(LogisticAppDbContext db)
        {
            _db = db;
        }

        public async Task<HomeViewModel> GetHomeDataAsync()
        {
            var countries = await _db.Addresses
                .Where(x => !string.IsNullOrWhiteSpace(x.Country))
                .Select(x => x.Country)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var companies = await _db.Companies
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => x.Name)
                .ToListAsync();

            return new HomeViewModel
            {
                SupportedCountries = countries,
                CourierCompanies = companies
            };
        }

        public async Task<HomeViewModel> TrackShipmentAsync(string? trackingNumber)
        {
            var model = await GetHomeDataAsync();
            model.TrackingNumber = trackingNumber?.Trim();

            if (string.IsNullOrWhiteSpace(model.TrackingNumber))
            {
                return model;
            }

            var shipment = await _db.Shipments
                .AsNoTracking()
                .Include(x => x.ReceiverAddress)
                .FirstOrDefaultAsync(x => x.ShipmentNo == model.TrackingNumber);

            if (shipment is null)
            {
                model.TrackingFound = false;
                model.Status = "Няма намерена пратка";
                return model;
            }

            var trackingEvents = await _db.TrackingEvents
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipment.Id)
                .Include(x => x.Location)
                    .ThenInclude(x => x.Address)
                .OrderByDescending(x => x.EventTimeUtc)
                .ToListAsync();

            var latestEvent = trackingEvents.FirstOrDefault();

            model.TrackingFound = true;
            model.ShipmentNo = shipment.ShipmentNo;
            model.Status = shipment.Status.ToString();
            model.LastUpdate = latestEvent?.EventTimeUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
            model.Details = latestEvent?.Details;

            if (latestEvent?.Location is not null)
            {
                model.CurrentLocation =
                    $"{latestEvent.Location.Name}, {latestEvent.Location.Address.City}, {latestEvent.Location.Address.Country}";
            }

            model.TrackingEvents = trackingEvents
                .Select(x => new TrackingEventViewModel
                {
                    EventType = x.EventType.ToString(),
                    EventTime = x.EventTimeUtc.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                    Location = x.Location != null
                        ? $"{x.Location.Name}, {x.Location.Address.City}, {x.Location.Address.Country}"
                        : null,
                    Details = x.Details
                })
                .ToList();

            return model;
        }
    }
}
