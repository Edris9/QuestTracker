using System;
using System.Collections.Generic;
using System.Linq;
using QuestTracker.Models;
using QuestTracker.Data;

namespace QuestTracker.Services
{
    public class QuestManager
    {
        private QuestTrackerContext _context;
        private int _currentUserID;

        public QuestManager(int userID = 0)
        {
            _context = new QuestTrackerContext();
            _context.InitializeDatabase();
            _currentUserID = userID;
        }

        public void SetCurrentUser(int userID)
        {
            _currentUserID = userID;
        }

        // Lägg till nytt quest
        public bool AddQuest(Quest quest)
        {
            try
            {
                if (quest == null || string.IsNullOrWhiteSpace(quest.Title))
                    return false;

                quest.UserID = _currentUserID;
                quest.CreatedDate = DateTime.Now;

                _context.Quests.Add(quest);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Ta bort quest
        public bool RemoveQuest(int questID)
        {
            try
            {
                Quest quest = _context.Quests.FirstOrDefault(q => q.QuestID == questID && q.UserID == _currentUserID);

                if (quest == null)
                    return false;

                _context.Quests.Remove(quest);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Hämta alla quests för inloggad användare
        public List<Quest> GetAllQuests()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID).ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Hämta quest via ID
        public Quest GetQuestByID(int questID)
        {
            try
            {
                return _context.Quests.FirstOrDefault(q => q.QuestID == questID && q.UserID == _currentUserID);
            }
            catch
            {
                return null;
            }
        }

        // Slutför quest
        public bool CompleteQuest(int questID, User currentUser = null)
        {
            try
            {
                Quest quest = GetQuestByID(questID);

                if (quest == null)
                    return false;

                quest.MarkAsCompleted();
                _context.SaveChanges();

                // Skicka SMS om vi har användare
                if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Phone))
                {
                    var notificationService = new NotificationService();
                    notificationService.SendCompletionNotification(currentUser.Phone, currentUser.Username, quest.Title);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Uppdatera quest
        public bool UpdateQuest(int questID, Quest updatedQuest)
        {
            try
            {
                Quest quest = GetQuestByID(questID);

                if (quest == null)
                    return false;

                quest.UpdateQuest(updatedQuest.Title, updatedQuest.Description, updatedQuest.DueDate, updatedQuest.Priority);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Hämta väntande quests
        public List<Quest> GetPendingQuests()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID && !q.IsCompleted).ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Hämta slutförda quests
        public List<Quest> GetCompletedQuests()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID && q.IsCompleted).ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Hämta quests nära deadline
        public List<Quest> GetQuestsNearDeadline()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID && q.IsNearDeadline()).ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Hämta quests sorterade efter prioritet
        public List<Quest> GetQuestsSortedByPriority()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID && !q.IsCompleted)
                            .OrderByDescending(q => q.GetPriorityValue())
                            .ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Hämta quests sorterade efter deadline
        public List<Quest> GetQuestsSortedByDeadline()
        {
            try
            {
                return _context.Quests.Where(q => q.UserID == _currentUserID && !q.IsCompleted)
                            .OrderBy(q => q.DueDate)
                            .ToList();
            }
            catch
            {
                return new List<Quest>();
            }
        }

        // Få total antal quests
        public int GetTotalQuestCount()
        {
            try
            {
                return _context.Quests.Count(q => q.UserID == _currentUserID);
            }
            catch
            {
                return 0;
            }
        }

        // Få procent av slutförda quests
        public double GetCompletionPercentage()
        {
            try
            {
                int total = GetTotalQuestCount();
                if (total == 0)
                    return 0;

                int completed = GetCompletedQuests().Count;
                return (completed / (double)total) * 100;
            }
            catch
            {
                return 0;
            }
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