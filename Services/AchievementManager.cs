using QuestTracker.Models;

namespace QuestTracker.Services
{
    public class AchievementManager
    {
        private QuestManager questManager;
        private int userID;

        public AchievementManager(QuestManager qm, int userId)
        {
            questManager = qm;
            userID = userId;
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> achievements = new List<Achievement>();
            int completed = questManager.GetCompletedQuests().Count;

            if (completed >= 1)
            {
                achievements.Add(new Achievement
                {
                    Title = "🥉 Brons Hjälte",
                    Description = "Slutför din första quest",
                    Badge = "🥉",
                    IsUnlocked = true,
                    UnlockedDate = DateTime.Now
                });
            }

            if (completed >= 5)
            {
                achievements.Add(new Achievement
                {
                    Title = "🥈 Silver Hjälte",
                    Description = "Slutför 5 quests",
                    Badge = "🥈",
                    IsUnlocked = true,
                    UnlockedDate = DateTime.Now
                });
            }

            if (completed >= 10)
            {
                achievements.Add(new Achievement
                {
                    Title = "🥇 Guld Hjälte",
                    Description = "Slutför 10 quests",
                    Badge = "🥇",
                    IsUnlocked = true,
                    UnlockedDate = DateTime.Now
                });
            }

            if (completed >= 25)
            {
                achievements.Add(new Achievement
                {
                    Title = "🏆 Legend Hjälte",
                    Description = "Slutför 25 quests",
                    Badge = "🏆",
                    IsUnlocked = true,
                    UnlockedDate = DateTime.Now
                });
            }

            return achievements;
        }

        public string GetCurrentRank()
        {
            int completed = questManager.GetCompletedQuests().Count;

            if (completed >= 25) return "🏆 Legend";
            if (completed >= 10) return "🥇 Guld";
            if (completed >= 5) return "🥈 Silver";
            if (completed >= 1) return "🥉 Brons";
            return "⚔️ Nybörjare";
        }
    }
}