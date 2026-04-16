using LogisticManagementApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services
{
    public interface ISequentialNumberGenerator
    {
        Task<string> GenerateOrderNoAsync();
        Task<string> GenerateShipmentNoAsync();
        Task<string> GeneratePackageNoAsync();
    }

    public class SequentialNumberGenerator : ISequentialNumberGenerator
    {
        private readonly LogisticAppDbContext _dbContext;

        public SequentialNumberGenerator(LogisticAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<string> GenerateOrderNoAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"ORD-{year}-";

            return GenerateSequentialNumberAsync(
                _dbContext.Orders.Select(x => x.OrderNo),
                prefix);
        }

        public Task<string> GenerateShipmentNoAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"SHP-{year}-";

            return GenerateSequentialNumberAsync(
                _dbContext.Shipments.Select(x => x.ShipmentNo),
                prefix);
        }

        public Task<string> GeneratePackageNoAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"PKG-{year}-";

            return GenerateSequentialNumberAsync(
                _dbContext.Packages.Select(x => x.PackageNo),
                prefix);
        }

        private static string ExtractNumericSuffix(string value, string prefix)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? value[prefix.Length..]
                : string.Empty;
        }

        private async Task<string> GenerateSequentialNumberAsync(
            IQueryable<string> source,
            string prefix)
        {
            var existingNumbers = await source
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.StartsWith(prefix))
                .ToListAsync();

            var usedNumbers = existingNumbers
                .Select(x => ExtractNumericSuffix(x, prefix))
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToHashSet();

            var nextNumber = 1;

            while (usedNumbers.Contains(nextNumber))
            {
                nextNumber++;
            }

            return $"{prefix}{nextNumber:D6}";
        }
    }
}
