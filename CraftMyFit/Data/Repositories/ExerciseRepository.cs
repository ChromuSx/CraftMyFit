using CraftMyFit.Data.Interfaces;
using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Workout;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CraftMyFit.Data.Repositories
{
    public class ExerciseRepository : Repository<Exercise>, IExerciseRepository
    {
        public ExerciseRepository(CraftMyFitDbContext context) : base(context)
        {
        }

        public async Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup) => await _context.Exercises
                .Where(e => e.MuscleGroup.ToLower() == muscleGroup.ToLower())
                .OrderBy(e => e.Name)
                .ToListAsync();

        public async Task<List<Exercise>> SearchExercisesAsync(string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            string lowerSearchTerm = searchTerm.ToLower();

            return await _context.Exercises
                .Where(e => e.Name.ToLower().Contains(lowerSearchTerm) ||
                           e.Description.ToLower().Contains(lowerSearchTerm) ||
                           e.MuscleGroup.ToLower().Contains(lowerSearchTerm))
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<List<Exercise>> GetExercisesWithoutEquipmentAsync() => await _context.Exercises
                .Where(e => e.RequiredEquipmentJson == "[]" || string.IsNullOrEmpty(e.RequiredEquipmentJson))
                .OrderBy(e => e.Name)
                .ToListAsync();

        public async Task<List<Exercise>> GetExercisesByEquipmentAsync(List<string> equipment)
        {
            List<Exercise> exercises = await _context.Exercises.ToListAsync();

            return exercises
                .Where(e =>
                {
                    try
                    {
                        List<string> requiredEquipment = string.IsNullOrEmpty(e.RequiredEquipmentJson)
                            ? []
                            : JsonSerializer.Deserialize<List<string>>(e.RequiredEquipmentJson) ?? [];

                        return !requiredEquipment.Any() || requiredEquipment.All(req => equipment.Contains(req));
                    }
                    catch
                    {
                        return false;
                    }
                })
                .OrderBy(e => e.Name)
                .ToList();
        }

        public async Task<List<string>> GetAllMuscleGroupsAsync() => await _context.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .OrderBy(mg => mg)
                .ToListAsync();

        public async Task<bool> ExerciseExistsAsync(string name) => await _context.Exercises
                .AnyAsync(e => e.Name.ToLower() == name.ToLower());

        public override async Task<List<Exercise>> GetAllAsync() => await _context.Exercises
                .OrderBy(e => e.MuscleGroup)
                .ThenBy(e => e.Name)
                .ToListAsync();
    }
}