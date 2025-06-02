using CraftMyFit.Data.Interfaces;
using CraftMyFit.Models.Gamification;

namespace CraftMyFit.Services
{
    public class BadgeService
    {
        private readonly IAchievementRepository _achievementRepository;

        public BadgeService(IAchievementRepository achievementRepository) => _achievementRepository = achievementRepository;

        /// <summary>
        /// Verifica e sblocca badge in base ai progressi
        /// </summary>
        public List<Badge> CheckAndUnlockBadges(int userId, BadgeConditionType conditionType, int currentValue)
        {
            List<Badge> unlockedBadges = [];

            try
            {
                List<Badge> availableBadges = GetAvailableBadges(userId, conditionType);

                foreach(Badge? badge in availableBadges.Where(b => !b.IsEarned && currentValue >= b.ConditionValue))
                {
                    // Sblocca il badge
                    badge.EarnedDate = DateTime.Now;
                    unlockedBadges.Add(badge);

                    System.Diagnostics.Debug.WriteLine($"Badge sbloccato: {badge.Name}");
                }

                return unlockedBadges;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nel controllo badge: {ex.Message}");
                return unlockedBadges;
            }
        }

        /// <summary>
        /// Ottiene tutti i badge disponibili per una condizione specifica
        /// </summary>
        private List<Badge> GetAvailableBadges(int userId, BadgeConditionType conditionType)
        {
            // In un'implementazione reale, questi dati verrebbero dal database
            // Per ora creiamo una lista statica di badge disponibili

            List<Badge> badges = [];

            switch(conditionType)
            {
                case BadgeConditionType.WorkoutsCompleted:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Primo Passo", "Completa il tuo primo allenamento", "#4CAF50", BadgeRarity.Common, 1),
                        CreateBadge(userId, "In Movimento", "Completa 5 allenamenti", "#2196F3", BadgeRarity.Common, 5),
                        CreateBadge(userId, "Determinato", "Completa 10 allenamenti", "#FF9800", BadgeRarity.Uncommon, 10),
                        CreateBadge(userId, "Atleta", "Completa 25 allenamenti", "#9C27B0", BadgeRarity.Rare, 25),
                        CreateBadge(userId, "Campione", "Completa 50 allenamenti", "#F44336", BadgeRarity.Epic, 50),
                        CreateBadge(userId, "Leggenda", "Completa 100 allenamenti", "#FFD700", BadgeRarity.Legendary, 100)
                    });
                    break;

                case BadgeConditionType.ConsecutiveDays:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Costante", "Allenati per 3 giorni consecutivi", "#4CAF50", BadgeRarity.Common, 3),
                        CreateBadge(userId, "Disciplinato", "Allenati per 7 giorni consecutivi", "#2196F3", BadgeRarity.Uncommon, 7),
                        CreateBadge(userId, "Implacabile", "Allenati per 14 giorni consecutivi", "#FF9800", BadgeRarity.Rare, 14),
                        CreateBadge(userId, "Inarrestabile", "Allenati per 30 giorni consecutivi", "#9C27B0", BadgeRarity.Epic, 30)
                    });
                    break;

                case BadgeConditionType.PhotosUploaded:
                    badges.AddRange(new[]
                    {
                        CreateBadge(userId, "Prima Foto", "Carica la tua prima foto di progresso", "#4CAF50", BadgeRarity.Common, 1),
                        CreateBadge(userId, "Documentarista", "Carica 5 foto di progresso", "#2196F3", BadgeRarity.Common, 5),
                        CreateBadge(userId, "Cronista", "Carica 10 foto di progresso", "#FF9800", BadgeRarity.Uncommon, 10)
                    });
                    break;
            }

            return badges;
        }

        private Badge CreateBadge(int userId, string name, string description, string color, BadgeRarity rarity, int conditionValue) => new()
        {
            Name = name,
            Description = description,
            IconPath = $"badge_{name.ToLower().Replace(" ", "_")}.png",
            Color = color,
            Category = BadgeCategory.Achievement,
            Rarity = rarity,
            ConditionType = BadgeConditionType.WorkoutsCompleted,
            ConditionValue = conditionValue,
            ConditionDescription = description,
            UserId = userId,
            User = new User { Id = userId, Name = "User" }, // Placeholder
            IsVisible = true
        };
    }
}