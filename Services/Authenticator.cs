using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using QuestTracker.Models;

namespace QuestTracker.Services
{
    public class Authenticator
    {
        private static Authenticator _instance;
        private List<User> users = new List<User>();
        private User currentUser = null;
        private string twoFACode = "";
        private Random random = new Random();

        // Singleton pattern
        public static Authenticator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Authenticator();
                return _instance;
            }
        }

        private Authenticator() { }

        // Registrera en ny hjälte
        public bool Register(string username, string password, string email, string phone)
        {
            if (users.Any(u => u.Username == username))
                return false;

            User newUser = new User(username, email, phone);

            if (!newUser.ValidatePasswordStrength(password))
                return false;

            newUser.PasswordHash = HashPassword(password);
            newUser.UserID = users.Count + 1;

            users.Add(newUser);
            return true;
        }

        // Logga in en hjälte
        public bool Login(string username, string password)
        {
            User user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return false;

            if (!VerifyPassword(password, user.PasswordHash))
                return false;

            currentUser = user;
            return true;
        }

        // Generera 2FA-kod
        public string GenerateTwoFACode()
        {
            twoFACode = random.Next(100000, 999999).ToString();
            return twoFACode;
        }

        // Verifiera 2FA-kod
        public bool VerifyTwoFACode(string enteredCode)
        {
            return enteredCode == twoFACode;
        }

        // Logga ut
        public void Logout()
        {
            currentUser = null;
        }

        // Kolla om någon är inloggad
        public bool IsUserLoggedIn()
        {
            return currentUser != null;
        }

        // Få den inloggade användaren
        public User GetCurrentUser()
        {
            return currentUser;
        }

        // Hash lösenord (med Salt)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Verifiera lösenord
        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }

        // Hämta alla användare (för test/debug)
        public List<User> GetAllUsers()
        {
            return users;
        }
    }
}