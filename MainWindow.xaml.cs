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

            Button aiDescBtn = new Button
            {
                Content = "🤖 AI: Skapa Beskrivning",
                Height = 35,
                Background = System.Windows.Media.Brushes.Orange,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 0, 15)
            };

            aiDescBtn.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(titleInput.Text))
                {
                    MessageBox.Show("❌ Fyll i titel först!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                aiDescBtn.IsEnabled = false;
                aiDescBtn.Content = "⏳ Genererar...";

                string description = await OllamaAdvisor.GenerateQuestDescription(titleInput.Text);
                descInput.Text = description;

                aiDescBtn.IsEnabled = true;
                aiDescBtn.Content = "🤖 AI: Skapa Beskrivning";
            };

            TextBlock priorityLabel = new TextBlock { Text = "Prioritet:", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            ComboBox priorityInput = new ComboBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };
            priorityInput.Items.Add("Hög");
            priorityInput.Items.Add("Medium");
            priorityInput.Items.Add("Låg");
            priorityInput.SelectedIndex = 1;

            TextBlock dateLabel = new TextBlock { Text = "Deadline Datum (YYYY-MM-DD):", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            TextBox dateInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock timeLabel = new TextBlock { Text = "Deadline Tid (HH:MM):", Foreground = System.Windows.Media.Brushes.GreenYellow, Margin = new Thickness(0, 0, 0, 5) };
            TextBox timeInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15), Text = "23:59" };

            Button aiPriorityBtn = new Button
            {
                Content = "🤖 AI: Föreslå Prioritet",
                Height = 35,
                Background = System.Windows.Media.Brushes.Orange,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 0, 15)
            };

            aiPriorityBtn.Click += async (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(titleInput.Text) || string.IsNullOrWhiteSpace(dateInput.Text))
                {
                    MessageBox.Show("❌ Fyll i titel och deadline först!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    DateTime dueDate = DateTime.Parse($"{dateInput.Text} {timeInput.Text}");
                    aiPriorityBtn.IsEnabled = false;
                    aiPriorityBtn.Content = "⏳ Analyserar...";

                    string priority = await OllamaAdvisor.SuggestPriority(titleInput.Text, dueDate);

                    for (int i = 0; i < priorityInput.Items.Count; i++)
                    {
                        if (priorityInput.Items[i].ToString() == priority)
                        {
                            priorityInput.SelectedIndex = i;
                            break;
                        }
                    }

                    aiPriorityBtn.IsEnabled = true;
                    aiPriorityBtn.Content = "🤖 AI: Föreslå Prioritet";
                }
                catch
                {
                    MessageBox.Show("❌ Ogiltigt datum/tid format!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    aiPriorityBtn.IsEnabled = true;
                    aiPriorityBtn.Content = "🤖 AI: Föreslå Prioritet";
                }
            };

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
                    DateTime dueDate = DateTime.Parse($"{dateInput.Text} {timeInput.Text}");

                    Quest newQuest = new Quest
                    {
                        Title = titleInput.Text,
                        Description = descInput.Text,
                        Priority = priorityInput.SelectedItem.ToString(),
                        DueDate = dueDate,
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
            panel.Children.Add(aiDescBtn);
            panel.Children.Add(priorityLabel);
            panel.Children.Add(priorityInput);
            panel.Children.Add(dateLabel);
            panel.Children.Add(dateInput);
            panel.Children.Add(timeLabel);
            panel.Children.Add(timeInput);
            panel.Children.Add(aiPriorityBtn);
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
                var borderBrush = quest.IsNearDeadline()
                    ? System.Windows.Media.Brushes.Red
                    : System.Windows.Media.Brushes.GreenYellow;

                Border border = new Border
                {
                    BorderBrush = borderBrush,
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

                double hoursLeft = quest.GetHoursRemaining();
                string timeWarning = quest.IsNearDeadline() ? "🔴 NÄRA DEADLINE! " : "";

                TextBlock questStatus = new TextBlock
                {
                    Text = $"{timeWarning}Status: {(quest.IsCompleted ? "✅ KLART" : "⏳ PÅGÅR")} | Prioritet: {quest.Priority} | Deadline: {quest.DueDate:yyyy-MM-dd HH:mm} | {hoursLeft:F1}h kvar",
                    Foreground = quest.IsNearDeadline() ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Yellow,
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

            var quests = questManager.GetPendingQuests();

            if (quests.Count == 0)
            {
                TextBlock noQuestMsg = new TextBlock
                {
                    Text = "Du har inga pågående quests att uppdatera!",
                    Foreground = System.Windows.Media.Brushes.Yellow,
                    FontSize = 14,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                ContentPanel.Children.Add(noQuestMsg);
                return;
            }

            StackPanel panel = new StackPanel();

            TextBlock selectLabel = new TextBlock { Text = "Välj quest att uppdatera:", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };

            ComboBox questSelector = new ComboBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.Black, Margin = new Thickness(0, 0, 0, 15) };

            foreach (var quest in quests)
            {
                questSelector.Items.Add($"{quest.QuestID} - {quest.Title}");
            }
            questSelector.SelectedIndex = 0;

            TextBlock titleLabel = new TextBlock { Text = "Ny Titel:", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };

            TextBox titleInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock descLabel = new TextBlock { Text = "Ny Beskrivning:", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };

            TextBox descInput = new TextBox { Height = 80, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock priorityLabel = new TextBlock { Text = "Ny Prioritet:", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };

            ComboBox priorityInput = new ComboBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };
            priorityInput.Items.Add("Hög");
            priorityInput.Items.Add("Medium");
            priorityInput.Items.Add("Låg");
            priorityInput.SelectedIndex = 1;

            TextBlock dateLabel = new TextBlock { Text = "Ny Deadline Datum (YYYY-MM-DD):", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };

            TextBox dateInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) };

            TextBlock timeLabel = new TextBlock { Text = "Ny Deadline Tid (HH:MM):", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            TextBox timeInput = new TextBox { Height = 35, Padding = new Thickness(10), Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15), Text = "23:59" };

            questSelector.SelectionChanged += (s, ev) =>
            {
                if (questSelector.SelectedIndex >= 0)
                {
                    var selectedQuest = quests[questSelector.SelectedIndex];
                    titleInput.Text = selectedQuest.Title;
                    descInput.Text = selectedQuest.Description;
                    dateInput.Text = selectedQuest.DueDate.ToString("yyyy-MM-dd");
                    timeInput.Text = selectedQuest.DueDate.ToString("HH:mm");

                    for (int i = 0; i < priorityInput.Items.Count; i++)
                    {
                        if (priorityInput.Items[i].ToString() == selectedQuest.Priority)
                        {
                            priorityInput.SelectedIndex = i;
                            break;
                        }
                    }
                }
            };

            if (quests.Count > 0)
            {
                var firstQuest = quests[0];
                titleInput.Text = firstQuest.Title;
                descInput.Text = firstQuest.Description;
                dateInput.Text = firstQuest.DueDate.ToString("yyyy-MM-dd");
                timeInput.Text = firstQuest.DueDate.ToString("HH:mm");

                for (int i = 0; i < priorityInput.Items.Count; i++)
                {
                    if (priorityInput.Items[i].ToString() == firstQuest.Priority)
                    {
                        priorityInput.SelectedIndex = i;
                        break;
                    }
                }
            }

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 0) };

            Button updateBtn = new Button
            {
                Content = "✏️ UPPDATERA",
                Width = 150,
                Height = 40,
                Background = System.Windows.Media.Brushes.GreenYellow,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };

            Button completeBtn = new Button
            {
                Content = "✅ SLUTFÖR",
                Width = 150,
                Height = 40,
                Background = System.Windows.Media.Brushes.LightGreen,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            updateBtn.Click += (s, ev) =>
            {
                if (questSelector.SelectedIndex < 0)
                {
                    MessageBox.Show("❌ Välj en quest!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var selectedQuest = quests[questSelector.SelectedIndex];
                    DateTime newDueDate = DateTime.Parse($"{dateInput.Text} {timeInput.Text}");

                    Quest updatedQuest = new Quest
                    {
                        Title = titleInput.Text,
                        Description = descInput.Text,
                        Priority = priorityInput.SelectedItem.ToString(),
                        DueDate = newDueDate
                    };

                    questManager.UpdateQuest(selectedQuest.QuestID, updatedQuest);
                    MessageBox.Show("✅ Quest uppdaterad!", "Framgång", MessageBoxButton.OK, MessageBoxImage.Information);
                    ViewQuests_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Fel: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            completeBtn.Click += (s, ev) =>
            {
                if (questSelector.SelectedIndex < 0)
                {
                    MessageBox.Show("❌ Välj en quest!", "Fel", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedQuest = quests[questSelector.SelectedIndex];

                MessageBoxResult result = MessageBox.Show($"Slutföra quest: {selectedQuest.Title}?", "Bekräfta", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    questManager.CompleteQuest(selectedQuest.QuestID);
                    MessageBox.Show("✅ Quest slutförd!", "Framgång", MessageBoxButton.OK, MessageBoxImage.Information);
                    ViewQuests_Click(null, null);
                }
            };

            buttonPanel.Children.Add(updateBtn);
            buttonPanel.Children.Add(completeBtn);

            panel.Children.Add(selectLabel);
            panel.Children.Add(questSelector);
            panel.Children.Add(titleLabel);
            panel.Children.Add(titleInput);
            panel.Children.Add(descLabel);
            panel.Children.Add(descInput);
            panel.Children.Add(priorityLabel);
            panel.Children.Add(priorityInput);
            panel.Children.Add(dateLabel);
            panel.Children.Add(dateInput);
            panel.Children.Add(timeLabel);
            panel.Children.Add(timeInput);
            panel.Children.Add(buttonPanel);

            ContentPanel.Children.Add(panel);
        }

        private async void AIAdvisor_Click(object sender, RoutedEventArgs e)
        {
            ContentTitle.Text = "🤖 Guild Advisor - Heroisk Briefing";
            ContentPanel.Children.Clear();

            StackPanel panel = new StackPanel();

            bool ollamaRunning = await OllamaAdvisor.IsOllamaRunning();

            if (!ollamaRunning)
            {
                TextBlock warningText = new TextBlock
                {
                    Text = "⚠️ Ollama körs inte! Starta: ollama serve",
                    Foreground = System.Windows.Media.Brushes.Orange,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 0, 15),
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold
                };
                panel.Children.Add(warningText);
            }

            Button briefingBtn = new Button
            {
                Content = "📜 FÅ HEROISK BRIEFING",
                Height = 50,
                Background = System.Windows.Media.Brushes.GreenYellow,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 20, 0, 20)
            };

            TextBox briefingResponse = new TextBox
            {
                Height = 300,
                Padding = new Thickness(15),
                Background = System.Windows.Media.Brushes.DarkSlateGray,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontSize = 13,
                Text = "Klicka på knappen för att få din heroiska briefing..."
            };

            briefingBtn.Click += async (s, ev) =>
            {
                briefingBtn.IsEnabled = false;
                briefingBtn.Content = "⏳ Skapar briefing...";
                briefingResponse.Text = "🤖 Guild Advisor förbereder din briefing...";

                try
                {
                    var allQuests = questManager.GetAllQuests();
                    int completed = questManager.GetCompletedQuests().Count;
                    int pending = questManager.GetPendingQuests().Count;
                    int nearDeadline = questManager.GetQuestsNearDeadline().Count;

                    string result = await OllamaAdvisor.SummarizeQuests(allQuests.Count, completed, pending, nearDeadline);
                    briefingResponse.Text = $"📜 HEROISK BRIEFING\n\n{result}";
                }
                catch (Exception ex)
                {
                    briefingResponse.Text = $"❌ Fel: {ex.Message}";
                }
                finally
                {
                    briefingBtn.IsEnabled = true;
                    briefingBtn.Content = "📜 FÅ HEROISK BRIEFING";
                }
            };

            panel.Children.Add(briefingBtn);
            panel.Children.Add(briefingResponse);

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