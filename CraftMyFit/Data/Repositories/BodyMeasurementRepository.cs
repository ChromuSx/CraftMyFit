using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Progress;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Repositories
{
    public class BodyMeasurementRepository : Repository<BodyMeasurement>, IBodyMeasurementRepository
    {
        public BodyMeasurementRepository(CraftMyFitDbContext context) : base(context)
        {
        }

        public async Task<List<BodyMeasurement>> GetByUserIdAsync(int userId) => await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId)
                .OrderByDescending(bm => bm.Date)
                .ToListAsync();

        public async Task<List<BodyMeasurement>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate) => await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId &&
                            bm.Date >= startDate &&
                            bm.Date <= endDate)
                .OrderByDescending(bm => bm.Date)
                .ToListAsync();

        public async Task<BodyMeasurement?> GetLatestByUserIdAsync(int userId) => await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId)
                .OrderByDescending(bm => bm.Date)
                .FirstOrDefaultAsync();

        public async Task<BodyMeasurement?> GetByUserIdAndDateAsync(int userId, DateTime date)
        {
            DateTime targetDate = date.Date;
            return await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId && bm.Date.Date == targetDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BodyMeasurement>> GetWeightHistoryAsync(int userId, int months = 12)
        {
            DateTime startDate = DateTime.Now.AddMonths(-months);

            return await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId && bm.Date >= startDate)
                .OrderBy(bm => bm.Date)
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<BodyMeasurement>>> GetMeasurementHistoryAsync(int userId, int months = 12)
        {
            DateTime startDate = DateTime.Now.AddMonths(-months);
            List<BodyMeasurement> measurements = await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId && bm.Date >= startDate)
                .OrderBy(bm => bm.Date)
                .ToListAsync();

            Dictionary<string, List<BodyMeasurement>> history = new();

            // Raggruppa per tipo di misurazione
            if(measurements.Any(m => m.Weight > 0))
            {
                history["Peso"] = measurements.Where(m => m.Weight > 0).ToList();
            }

            if(measurements.Any(m => m.Chest.HasValue))
            {
                history["Petto"] = measurements.Where(m => m.Chest.HasValue).ToList();
            }

            if(measurements.Any(m => m.Waist.HasValue))
            {
                history["Vita"] = measurements.Where(m => m.Waist.HasValue).ToList();
            }

            if(measurements.Any(m => m.Hips.HasValue))
            {
                history["Fianchi"] = measurements.Where(m => m.Hips.HasValue).ToList();
            }

            if(measurements.Any(m => m.LeftArm.HasValue))
            {
                history["Braccio Sinistro"] = measurements.Where(m => m.LeftArm.HasValue).ToList();
            }

            if(measurements.Any(m => m.RightArm.HasValue))
            {
                history["Braccio Destro"] = measurements.Where(m => m.RightArm.HasValue).ToList();
            }

            if(measurements.Any(m => m.LeftThigh.HasValue))
            {
                history["Coscia Sinistra"] = measurements.Where(m => m.LeftThigh.HasValue).ToList();
            }

            if(measurements.Any(m => m.RightThigh.HasValue))
            {
                history["Coscia Destra"] = measurements.Where(m => m.RightThigh.HasValue).ToList();
            }

            if(measurements.Any(m => m.BodyFatPercentage.HasValue))
            {
                history["Grasso Corporeo"] = measurements.Where(m => m.BodyFatPercentage.HasValue).ToList();
            }

            if(measurements.Any(m => m.MuscleMass.HasValue))
            {
                history["Massa Muscolare"] = measurements.Where(m => m.MuscleMass.HasValue).ToList();
            }

            return history;
        }

        public async Task<bool> HasMeasurementForDateAsync(int userId, DateTime date)
        {
            DateTime targetDate = date.Date;
            return await _context.BodyMeasurements
                .AnyAsync(bm => bm.UserId == userId && bm.Date.Date == targetDate);
        }

        public async Task<float?> GetWeightChangeAsync(int userId, int days = 30)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-days);

            List<BodyMeasurement> measurements = await _context.BodyMeasurements
                .Where(bm => bm.UserId == userId && bm.Weight > 0)
                .OrderByDescending(bm => bm.Date)
                .ToListAsync();

            if(measurements.Count < 2)
            {
                return null;
            }

            BodyMeasurement latestMeasurement = measurements.First();
            BodyMeasurement? olderMeasurement = measurements
                .FirstOrDefault(m => m.Date <= startDate);

            return olderMeasurement == null ? null : latestMeasurement.Weight - olderMeasurement.Weight;
        }

        public override async Task<List<BodyMeasurement>> GetAllAsync() => await _context.BodyMeasurements
                .Include(bm => bm.User)
                .OrderByDescending(bm => bm.Date)
                .ToListAsync();
    }
}