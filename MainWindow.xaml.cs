using QuestTracker.Models;
using QuestTracker.Services;
using QuestTracker.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QuestTracker
{
    public partial class MainWindow : Window
    {
        private Authenticator authenticator;
        private QuestManager questManager;
        private User currentUser;

        public MainWindow()
        {
            InitializeComponent();

            authenticator = Authenticator.Instance;
            questManager = new QuestManager();
            currentUser = authenticator.GetCurrentUser();

            if (currentUser != null)
            {
                questManager.SetCurrentUser(currentUser.UserID);
                HeroNameDisplay.Text = $"Hjälte: {currentUser.Username}";
            }
        }

        private void AddQuest_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "➕ Lägg Till Nytt Uppdrag";
            ContentPanel.Children.Clear();

            StackPanel panel = new StackPanel();

            TextBlock titleLabel = new TextBlock { Text = "Quest Titel:", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            TextBox titleInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock descLabel = new TextBlock { Text = "Beskrivning:", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            TextBox descInput = new TextBox { Height = 80, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock priorityLabel = new TextBlock { Text = "Prioritet (Hög/Medium/Låg):", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            ComboBox priorityInput = new ComboBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };
            priorityInput.Items.Add("Hög");
            priorityInput.Items.Add("Medium");
            priorityInput.Items.Add("Låg");
            priorityInput.SelectedIndex = 0;

            TextBlock dateLabel = new TextBlock { Text = "Deadline (YYYY-MM-DD):", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            TextBox dateInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };

            Button createBtn = new Button
            {
                Content = "✅ SKAPA QUEST",
                Height = 40,
                Background = System.Windows.Media.Brushes.GreenYellow,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            createBtn.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(titleInput.Text))
                {
                    MessageBox.Show("❌ Fyll i titel!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    Quest newQuest = new Quest
                    {
                        Title = titleInput.Text,
                        Description = descInput.Text,
                        Priority = priorityInput.SelectedItem.ToString(),
                        DueDate = DateTime.Parse(dateInput.Text),
                        IsCompleted = false
                    };

                    questManager.AddQuest(newQuest);
                    MessageBox.Show("✅ Quest skapat!", "Framgång", MessageBoxButton.OK, MessageBoxImage.Information);
                    ViewQuests_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Fel: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            panel.Children.Add(titleLabel);
            panel.Children.Add(titleInput);
            panel.Children.Add(descLabel);
            panel.Children.Add(descInput);
            panel.Children.Add(priorityLabel);
            panel.Children.Add(priorityInput);
            panel.Children.Add(dateLabel);
            panel.Children.Add(dateInput);
            panel.Children.Add(createBtn);

            ContentPanel.Children.Add(panel);
        }

        private void ViewQuests_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "👀 Dina Quests";
            ContentPanel.Children.Clear();

            var quests = questManager.GetAllQuests();

            if (quests.Count == 0)
            {
                TextBlock noQuestMsg = new TextBlock
                {
                    Text = "Du har inga quests ännu! Lägg till en ny.",
                    Foreground = System.Windows.Media.Brushes.Yellow,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                ContentPanel.Children.Add(noQuestMsg);
                return;
            }

            foreach (var quest in quests)
            {
                Border border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.GreenYellow,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Background = System.Windows.Media.Brushes.DarkGray,
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 15)
                };

                StackPanel questPanel = new StackPanel();

                TextBlock questTitle = new TextBlock
                {
                    Text = $"⚔️ {quest.Title}",
                    Foreground = System.Windows.Media.Brushes.GreenYellow,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                };

                TextBlock questDesc = new TextBlock
                {
                    Text = quest.Description,
                    Foreground = System.Windows.Media.Brushes.LightGray,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                TextBlock questStatus = new TextBlock
                {
                    Text = $"Status: {(quest.IsCompleted ? "✅ KLART" : "⏳ PÅGÅR")} | Prioritet: {quest.Priority} | Deadline: {quest.DueDate:yyyy-MM-dd}",
                    Foreground = System.Windows.Media.Brushes.Yellow,
                    FontSize = 12
                };

                questPanel.Children.Add(questTitle);
                questPanel.Children.Add(questDesc);
                questPanel.Children.Add(questStatus);

                border.Child = questPanel;
                ContentPanel.Children.Add(border);
            }
        }

        private void UpdateQuest_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "✏️ Uppdatera Quest";
            ContentPanel.Children.Clear();

            TextBlock infoMsg = new TextBlock
            {
                Text = "Denna funktion kommer snart! 🔜",
                Foreground = System.Windows.Media.Brushes.Yellow,
                FontSize = 14,
                Margin = new Thickness(0, 20, 0, 0)
            };
            ContentPanel.Children.Add(infoMsg);
        }

        private async void AIAdvisor_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "🤖 Guild Advisor - Lokal AI (Ollama)";
            ContentPanel.Children.Clear();

            StackPanel panel = new StackPanel();

            bool ollamaRunning = await OllamaAdvisor.IsOllamaRunning();

            if (!ollamaRunning)
            {
                TextBlock warningText = new TextBlock
                {
                    Text = "⚠️ Ollama körs inte!\n\nStarta Ollama genom att öppna en terminal och skriva:\nollama serve",
                    Foreground = System.Windows.Media.Brushes.Orange,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 0, 15),
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold
                };
                panel.Children.Add(warningText);
            }

            TextBlock infoText = new TextBlock
            {
                Text = "Beskriv vad du behöver hjälp med, så ger AI:n dig förslag! (Använder Llama 3.1 8B lokalt)",
                Foreground = System.Windows.Media.Brushes.LightCyan,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock inputLabel = new TextBlock
            {
                Text = "Din fråga eller situation:",
                Foreground = System.Windows.Media.Brushes.GreenYellow,
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBox aiInput = new TextBox
            {
                Height = 100,
                Padding = new Thickness(10),
                Background = System.Windows.Media.Brushes.DarkGray,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15),
                FontSize = 13
            };

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20) };

            Button askAIBtn = new Button
            {
                Content = "💬 Fråga AI",
                Width = 200,
                Height = 40,
                Background = System.Windows.Media.Brushes.GreenYellow,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };

            Button analyzeBtn = new Button
            {
                Content = "📊 Analysera Mina Quests",
                Width = 200,
                Height = 40,
                Background = System.Windows.Media.Brushes.Orange,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            buttonPanel.Children.Add(askAIBtn);
            buttonPanel.Children.Add(analyzeBtn);

            TextBlock responseLabel = new TextBlock
            {
                Text = "AI-svar:",
                Foreground = System.Windows.Media.Brushes.GreenYellow,
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBox aiResponse = new TextBox
            {
                Height = 200,
                Padding = new Thickness(10),
                Background = System.Windows.Media.Brushes.DarkSlateGray,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontSize = 12,
                Text = "Svaret visas här..."
            };

            askAIBtn.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(aiInput.Text))
                {
                    MessageBox.Show("❌ Skriv in din fråga först!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                askAIBtn.IsEnabled = false;
                askAIBtn.Content = "⏳ Väntar på AI...";
                aiResponse.Text = "🤖 AI tänker...";

                try
                {
                    string result = await OllamaAdvisor.GenerateQuestSuggestion(aiInput.Text);
                    aiResponse.Text = result;
                }
                catch (Exception ex)
                {
                    aiResponse.Text = $"❌ Fel: {ex.Message}";
                }
                finally
                {
                    askAIBtn.IsEnabled = true;
                    askAIBtn.Content = "💬 Fråga AI";
                }
            };

            analyzeBtn.Click += async (s, ev) =>
            {
                analyzeBtn.IsEnabled = false;
                analyzeBtn.Content = "⏳ Analyserar...";
                aiResponse.Text = "🤖 Lokal AI analyserar dina quests...";

                try
                {
                    var allQuests = questManager.GetAllQuests();
                    int completed = questManager.GetCompletedQuests().Count;
                    int pending = questManager.GetPendingQuests().Count;
                    int nearDeadline = questManager.GetQuestsNearDeadline().Count;

                    string result = await OllamaAdvisor.AnalyzeQuests(allQuests.Count, completed, pending, nearDeadline);
                    aiResponse.Text = result;
                }
                catch (Exception ex)
                {
                    aiResponse.Text = $"❌ Fel: {ex.Message}";
                }
                finally
                {
                    analyzeBtn.IsEnabled = true;
                    analyzeBtn.Content = "📊 Analysera Mina Quests";
                }
            };

            panel.Children.Add(infoText);
            panel.Children.Add(inputLabel);
            panel.Children.Add(aiInput);
            panel.Children.Add(buttonPanel);
            panel.Children.Add(responseLabel);
            panel.Children.Add(aiResponse);

            ContentPanel.Children.Add(panel);
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "📊 Guild Rapport";
            ContentPanel.Children.Clear();

            var allQuests = questManager.GetAllQuests();
            int completed = questManager.GetCompletedQuests().Count;
            int pending = questManager.GetPendingQuests().Count;
            int nearDeadline = questManager.GetQuestsNearDeadline().Count;

            StackPanel reportPanel = new StackPanel();

            TextBlock stat1 = new TextBlock { Text = $"✅ Totalt klara quests: {completed}", Foreground = System.Windows.Media.Brushes.GreenYellow, FontSize = 14, Margin = new Thickness(0, 0, 0, 10) };
            TextBlock stat2 = new TextBlock { Text = $"⏳ Pågående quests: {pending}", Foreground = System.Windows.Media.Brushes.Yellow, FontSize = 14, Margin = new Thickness(0, 0, 0, 10) };
            TextBlock stat3 = new TextBlock { Text = $"🔴 Quests nära deadline: {nearDeadline}", Foreground = System.Windows.Media.Brushes.Red, FontSize = 14, Margin = new Thickness(0, 0, 0, 20) };

            TextBlock summary = new TextBlock
            {
                Text = $"Du är en duktig hjälte! Du har slutfört {completed} quests och har {pending} pågående.",
                Foreground = System.Windows.Media.Brushes.LightCyan,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };

            reportPanel.Children.Add(stat1);
            reportPanel.Children.Add(stat2);
            reportPanel.Children.Add(stat3);
            reportPanel.Children.Add(summary);

            ContentPanel.Children.Add(reportPanel);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Är du säker på att du vill logga ut?", "Logga ut", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}