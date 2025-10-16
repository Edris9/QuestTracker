using System;
using System.Windows;
using QuestTracker.Services;

namespace QuestTracker.Views
{
    public partial class TwoFAWindow : Window
    {
        private Authenticator authenticator;

        public TwoFAWindow(Authenticator auth)
        {
            InitializeComponent();
            authenticator = auth;
        }

        // Verifiera 2FA-kod
        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = TwoFACodeInput.Text;

            // Validering
            if (string.IsNullOrWhiteSpace(enteredCode))
            {
                ErrorMessage.Text = "❌ Ange en kod!";
                return;
            }

            if (enteredCode.Length != 6 || !int.TryParse(enteredCode, out _))
            {
                ErrorMessage.Text = "❌ Koden måste vara 6 siffror!";
                TwoFACodeInput.Clear();
                return;
            }

            // Verifiera koden
            if (authenticator.VerifyTwoFACode(enteredCode))
            {
                ErrorMessage.Text = "✅ Verifiering lyckades!";
                ErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 136));

                // Stäng fönstret efter 1 sekund
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.DialogResult = true;
                        this.Close();
                    });
                });
            }
            else
            {
                ErrorMessage.Text = "❌ Fel kod! Försök igen.";
                TwoFACodeInput.Clear();
                TwoFACodeInput.Focus();
            }
        }

        // Avbryt knappen
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}