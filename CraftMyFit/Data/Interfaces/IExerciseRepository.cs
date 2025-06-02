using CraftMyFit.Data.Repositories.Base;
using CraftMyFit.Models.Workout;

namespace CraftMyFit.Data.Interfaces
{
    public interface IExerciseRepository : IRepository<Exercise>
    {
        /// <summary>
        /// Ottiene tutti gli esercizi per un gruppo muscolare specifico
        /// </summary>
        Task<List<Exercise>> GetByMuscleGroupAsync(string muscleGroup);

        /// <summary>
        /// Cerca esercizi per nome, descrizione o gruppo muscolare
        /// </summary>
        Task<List<Exercise>> SearchExercisesAsync(string searchTerm);

        /// <summary>
        /// Ottiene esercizi che non richiedono attrezzatura
        /// </summary>
        Task<List<Exercise>> GetExercisesWithoutEquipmentAsync();

        /// <summary>
        /// Ottiene esercizi che possono essere eseguiti con l'attrezzatura disponibile
        /// </summary>
        Task<List<Exercise>> GetExercisesByEquipmentAsync(List<string> equipment);

        /// <summary>
        /// Ottiene tutti i gruppi muscolari disponibili
        /// </summary>
        Task<List<string>> GetAllMuscleGroupsAsync();

        /// <summary>
        /// Verifica se esiste già un esercizio con il nome specificato
        /// </summary>
        Task<bool> ExerciseExistsAsync(string name);
    }
}