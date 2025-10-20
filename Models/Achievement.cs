using System;

namespace QuestTracker.Models
{
    public class Achievement
    {
        public int AchievementID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; } // 🥉 🥈 🥇 🏆
        public DateTime UnlockedDate { get; set; }
        public bool IsUnlocked { get; set; }

        public Achievement()
        {
            IsUnlocked = false;
        }
    }
}