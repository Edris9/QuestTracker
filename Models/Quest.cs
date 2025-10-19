using System;

namespace QuestTracker.Models
{
    public class Quest
    {
        public int QuestID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public Quest()
        {
            CreatedDate = DateTime.Now;
            IsCompleted = false;
            Priority = "Medium";
        }

        public Quest(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            CreatedDate = DateTime.Now;
            IsCompleted = false;
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
            CompletedDate = DateTime.Now;
        }

        public void UpdateQuest(string title, string description, DateTime dueDate, string priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
        }

        public bool IsNearDeadline()
        {
            if (IsCompleted)
                return false;

            TimeSpan timeLeft = DueDate - DateTime.Now;
            return timeLeft.TotalHours <= 24 && timeLeft.TotalHours > 0;
        }

        public double GetHoursRemaining()
        {
            if (IsCompleted)
                return 0;

            TimeSpan timeLeft = DueDate - DateTime.Now;
            return timeLeft.TotalHours;
        }

        public int GetDaysRemaining()
        {
            if (IsCompleted)
                return 0;

            TimeSpan timeLeft = DueDate - DateTime.Now;
            return (int)timeLeft.TotalDays;
        }

        public int GetPriorityValue()
        {
            switch (Priority.ToLower())
            {
                case "hög":
                    return 3;
                case "medium":
                    return 2;
                case "låg":
                    return 1;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            return $"[{(IsCompleted ? "✅" : "⏳")}] {Title} - Prioritet: {Priority} - Deadline: {DueDate:yyyy-MM-dd HH:mm}";
        }
    }
}