using System;
using System.Collections.Generic;
using System.Linq;
using QuestTracker.Models;

namespace QuestTracker.Services
{
    public class QuestManager
    {
        private List<Quest> quests = new List<Quest>();
        private int questIDCounter = 1;

        // Lägg till nytt quest
        public bool AddQuest(Quest quest)
        {
            if (quest == null || string.IsNullOrWhiteSpace(quest.Title))
            {
                Console.WriteLine("❌ Quest måste ha en titel!");
                return false;
            }

            quest.QuestID = questIDCounter;
            questIDCounter++;
            quests.Add(quest);
            Console.WriteLine($"✅ Quest '{quest.Title}' tillagt!");
            return true;
        }

        // Ta bort quest
        public bool RemoveQuest(int questID)
        {
            Quest questToRemove = quests.FirstOrDefault(q => q.QuestID == questID);

            if (questToRemove == null)
            {
                Console.WriteLine("❌ Quest hittades inte!");
                return false;
            }

            quests.Remove(questToRemove);
            Console.WriteLine($"✅ Quest '{questToRemove.Title}' borttagen!");
            return true;
        }

        // Hämta alla quests
        public List<Quest> GetAllQuests()
        {
            return new List<Quest>(quests);
        }

        // Hämta quest via ID
        public Quest GetQuestByID(int questID)
        {
            return quests.FirstOrDefault(q => q.QuestID == questID);
        }

        // Slutför quest
        public bool CompleteQuest(int questID)
        {
            Quest quest = GetQuestByID(questID);

            if (quest == null)
            {
                Console.WriteLine("❌ Quest hittades inte!");
                return false;
            }

            quest.MarkAsCompleted();
            Console.WriteLine($"✅ Quest '{quest.Title}' slutfört!");
            return true;
        }

        // Uppdatera quest
        public bool UpdateQuest(int questID, Quest updatedQuest)
        {
            Quest quest = GetQuestByID(questID);

            if (quest == null)
            {
                Console.WriteLine("❌ Quest hittades inte!");
                return false;
            }

            quest.UpdateQuest(updatedQuest.Title, updatedQuest.Description, updatedQuest.DueDate, updatedQuest.Priority);
            Console.WriteLine($"✅ Quest '{quest.Title}' uppdaterat!");
            return true;
        }

        // Hämta väntande quests (inte slutförda)
        public List<Quest> GetPendingQuests()
        {
            return quests.Where(q => !q.IsCompleted).ToList();
        }

        // Hämta slutförda quests
        public List<Quest> GetCompletedQuests()
        {
            return quests.Where(q => q.IsCompleted).ToList();
        }

        // Hämta quests nära deadline (< 24 timmar)
        public List<Quest> GetQuestsNearDeadline()
        {
            return quests.Where(q => q.IsNearDeadline()).ToList();
        }

        // Hämta quests sorterade efter prioritet
        public List<Quest> GetQuestsSortedByPriority()
        {
            return quests.Where(q => !q.IsCompleted)
                        .OrderByDescending(q => q.GetPriorityValue())
                        .ToList();
        }

        // Hämta quests sorterade efter deadline
        public List<Quest> GetQuestsSortedByDeadline()
        {
            return quests.Where(q => !q.IsCompleted)
                        .OrderBy(q => q.DueDate)
                        .ToList();
        }

        // Få total antal quests
        public int GetTotalQuestCount()
        {
            return quests.Count;
        }

        // Få procent av slutförda quests
        public double GetCompletionPercentage()
        {
            if (quests.Count == 0)
                return 0;

            int completed = GetCompletedQuests().Count;
            return (completed / (double)quests.Count) * 100;
        }

        // Skapa rapport
        public string GenerateReport()
        {
            int total = GetTotalQuestCount();
            int completed = GetCompletedQuests().Count;
            int pending = GetPendingQuests().Count;
            int nearDeadline = GetQuestsNearDeadline().Count;
            double percentage = GetCompletionPercentage();

            string report = $@"
╔════════════════════════════════════╗
║   📊 GUILD RAPPORT                 ║
╚════════════════════════════════════╝

✅ Totalt slutförda quests: {completed}/{total} ({percentage:F1}%)
⏳ Pågående quests: {pending}
🔴 Quests nära deadline: {nearDeadline}

Du är en duktig hjälte! Fortsätt så! 🎉
";
            return report;
        }
    }
}