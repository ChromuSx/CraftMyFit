using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Progress;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Repositories
{
    public class ProgressPhotoRepository : Repository<ProgressPhoto>, IProgressPhotoRepository
    {
        public ProgressPhotoRepository(CraftMyFitDbContext context) : base(context)
        {
        }

        public async Task<List<ProgressPhoto>> GetByUserIdAsync(int userId) => await _context.ProgressPhotos
                .Where(pp => pp.UserId == userId)
                .OrderByDescending(pp => pp.Date)
                .ToListAsync();

        public async Task<List<ProgressPhoto>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate) => await _context.ProgressPhotos
                .Where(pp => pp.UserId == userId &&
                            pp.Date >= startDate &&
                            pp.Date <= endDate)
                .OrderByDescending(pp => pp.Date)
                .ToListAsync();

        public async Task<ProgressPhoto?> GetLatestByUserIdAsync(int userId) => await _context.ProgressPhotos
                .Where(pp => pp.UserId == userId)
                .OrderByDescending(pp => pp.Date)
                .FirstOrDefaultAsync();

        public async Task<List<ProgressPhoto>> GetRecentPhotosAsync(int userId, int count = 10) => await _context.ProgressPhotos
                .Where(pp => pp.UserId == userId)
                .OrderByDescending(pp => pp.Date)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetPhotoCountByUserIdAsync(int userId) => await _context.ProgressPhotos
                .CountAsync(pp => pp.UserId == userId);

        public async Task<bool> HasPhotosForDateAsync(int userId, DateTime date)
        {
            DateTime targetDate = date.Date;
            return await _context.ProgressPhotos
                .AnyAsync(pp => pp.UserId == userId && pp.Date.Date == targetDate);
        }

        public async Task<List<ProgressPhoto>> GetPhotosForComparisonAsync(int userId, DateTime compareDate, int daysTolerance = 7)
        {
            DateTime startDate = compareDate.AddDays(-daysTolerance);
            DateTime endDate = compareDate.AddDays(daysTolerance);

            return await _context.ProgressPhotos
                .Where(pp => pp.UserId == userId &&
                            pp.Date >= startDate &&
                            pp.Date <= endDate)
                .OrderBy(pp => Math.Abs((pp.Date - compareDate).TotalDays))
                .ToListAsync();
        }

        public override async Task<List<ProgressPhoto>> GetAllAsync() => await _context.ProgressPhotos
                .Include(pp => pp.User)
                .OrderByDescending(pp => pp.Date)
                .ToListAsync();

        public override async Task DeleteAsync(int id)
        {
            ProgressPhoto photo = await GetByIdAsync(id);
            if(photo != null)
            {
                // TODO: Eliminare anche il file fisico dell'immagine
                // File.Delete(photo.PhotoPath);

                _ = _context.ProgressPhotos.Remove(photo);
                _ = await _context.SaveChangesAsync();
            }
        }
    }
}