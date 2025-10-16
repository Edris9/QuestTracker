using System;

namespace QuestTracker.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Is2FAEnabled { get; set; }
        public DateTime CreatedDate { get; set; }

        // Constructor
        public User()
        {
            CreatedDate = DateTime.Now;
            Is2FAEnabled = true;
        }

        public User(string username, string email, string phone)
        {
            Username = username;
            Email = email;
            Phone = phone;
            CreatedDate = DateTime.Now;
            Is2FAEnabled = true;
        }

        // Validera lösenordsstyrka
        public bool ValidatePasswordStrength(string password)
        {
            if (password.Length < 6)
                return false;

            bool hasNumber = password.Any(char.IsDigit);
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasNumber && hasUpperCase && hasSpecialChar;
        }

        public override string ToString()
        {
            return $"Hjälte: {Username} | Email: {Email} | Skapad: {CreatedDate:yyyy-MM-dd}";
        }
    }
}