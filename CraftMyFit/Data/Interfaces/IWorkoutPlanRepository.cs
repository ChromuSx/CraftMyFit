using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Workout;

namespace CraftMyFit.Data.Interfaces
{
    public interface IWorkoutPlanRepository : IRepository<WorkoutPlan>
    {
        Task<List<WorkoutPlan>> GetByUserIdAsync(int userId);
        Task<WorkoutPlan> GetWorkoutPlanWithDetailsAsync(int planId);
    }
}
