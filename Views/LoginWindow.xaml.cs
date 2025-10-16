using System;
using System.Windows;
using QuestTracker.Services;

namespace QuestTracker.Views
{
    public partial class LoginWindow : Window
    {
        private Authenticator authenticator;
        private TwoFAWindow twoFAWindow;

        public LoginWindow()
        {
            InitializeComponent();
            authenticator = Authenticator.Instance;
        }

        // Byta till Login-fliken
        private void LoginTab_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
        }

        // Byta till Register-fliken
        private void RegisterTab_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
        }

        // Logga in knappen
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsername.Text;
            string password = LoginPassword.Password;

            // Validering
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LoginErrorMessage.Text = "❌ Fyll i alla fält!";
                return;
            }

            // Försök logga in
            if (authenticator.Login(username, password))
            {
                // Generera 2FA-kod
                string code = authenticator.GenerateTwoFACode();

                // Visa koden i en popup
                MessageBox.Show($"Din 2FA-kod är:\n\n{code}", "🔐 2FA-kod", MessageBoxButton.OK, MessageBoxImage.Information);

                // Öppna 2FA-fönstret
                twoFAWindow = new TwoFAWindow(authenticator);
                twoFAWindow.ShowDialog();

                // Om 2FA lyckades, öppna huvudfönstret
                if (authenticator.IsUserLoggedIn())
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            else
            {
                LoginErrorMessage.Text = "❌ Fel användarnamn eller lösenord!";
                LoginPassword.Clear();
            }
        }

        // Registrera knappen
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsername.Text;
            string email = RegisterEmail.Text;
            string phone = RegisterPhone.Text;
            string password = RegisterPassword.Password;

            // Validering
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                RegisterErrorMessage.Text = "❌ Fyll i alla fält!";
                return;
            }

            // Validera email format
            if (!email.Contains("@"))
            {
                RegisterErrorMessage.Text = "❌ Ogiltigt email-format!";
                return;
            }

            // Försök registrera
            if (authenticator.Register(username, password, email, phone))
            {
                RegisterErrorMessage.Text = "✅ Registrering lyckades! Logga nu in.";
                RegisterErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 136));

                // Rensa fälten
                RegisterUsername.Clear();
                RegisterEmail.Clear();
                RegisterPhone.Clear();
                RegisterPassword.Clear();

                MessageBox.Show("✅ Registrering lyckades!\n\nLogga nu in med dina uppgifter.", "Framgång", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                RegisterErrorMessage.Text = "❌ Registrering misslyckades!";
            }
        }
    }
}