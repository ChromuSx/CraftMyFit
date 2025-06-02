using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace CraftMyFit.Data.Repositories
{
    public class WorkoutPlanRepository : Repository<WorkoutPlan>, IWorkoutPlanRepository
    {
        public WorkoutPlanRepository(CraftMyFitDbContext context) : base(context)
        {
        }

        public async Task<List<WorkoutPlan>> GetByUserIdAsync(int userId) => await _context.WorkoutPlans
                .Where(wp => wp.UserId == userId)
                .OrderByDescending(wp => wp.ModifiedDate)
                .ToListAsync();

        public async Task<WorkoutPlan> GetWorkoutPlanWithDetailsAsync(int planId) => await _context.WorkoutPlans
                .Include(wp => wp.WorkoutDays)
                    .ThenInclude(wd => wd.Exercises)
                        .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(wp => wp.Id == planId);
    }
}
