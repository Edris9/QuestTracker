using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuestTracker.Services
{
    public class OllamaAdvisor
    {
        private static readonly string OLLAMA_URL = "http://localhost:11434/api/generate";
        private static readonly string MODEL = "llama3.1:8b";
        private static readonly HttpClient client = new HttpClient();

        static OllamaAdvisor()
        {
            // Sätt längre timeout för lokala modeller
            client.Timeout = TimeSpan.FromMinutes(2);
        }

        // Generera quest-förslag med Ollama
        public static async Task<string> GenerateQuestSuggestion(string userInput)
        {
            try
            {
                string prompt = $@"Du är en Guild Advisor som hjälper hjältar att skapa och organisera quests.

Användaren säger: {userInput}

Ge konkreta förslag på:
1. En passande quest-titel
2. En detaljerad beskrivning
3. Rekommenderad prioritet (Hög/Medium/Låg)
4. Förslag på deadline

Svara kort och koncist på svenska.";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new
                    {
                        temperature = 0.7,
                        top_p = 0.9,
                        max_tokens = 500
                    }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"❌ Ollama-fel: {response.StatusCode}\n{responseBody}";
                }

                // Parsa JSON-svar från Ollama
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("response", out JsonElement responseElement))
                {
                    return responseElement.GetString() ?? "❌ Inget svar från Ollama";
                }

                return "❌ Kunde inte tolka Ollama-svar";
            }
            catch (HttpRequestException ex)
            {
                return $"❌ Nätverksfel: {ex.Message}\n\nSe till att Ollama körs! Kör 'ollama serve' i terminalen.";
            }
            catch (TaskCanceledException)
            {
                return "❌ Timeout: Ollama tog för lång tid att svara. Försök igen.";
            }
            catch (Exception ex)
            {
                return $"❌ Fel: {ex.Message}";
            }
        }

        // Analysera användarens quests med Ollama
        public static async Task<string> AnalyzeQuests(int totalQuests, int completed, int pending, int nearDeadline)
        {
            try
            {
                string prompt = $@"Du är en Guild Advisor som analyserar hjältars quest-prestationer.

Statistik:
- Totalt quests: {totalQuests}
- Slutförda: {completed}
- Pågående: {pending}
- Nära deadline: {nearDeadline}

Ge hjälten:
1. En kort analys av deras prestationer
2. Tips för att förbättra produktivitet
3. Motiverande ord

Svara kort på svenska (max 5 meningar).";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new
                    {
                        temperature = 0.7,
                        top_p = 0.9,
                        max_tokens = 300
                    }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"❌ Ollama-fel: {response.StatusCode}";
                }

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("response", out JsonElement responseElement))
                {
                    return responseElement.GetString() ?? "❌ Inget svar från Ollama";
                }

                return "❌ Kunde inte tolka Ollama-svar";
            }
            catch (HttpRequestException ex)
            {
                return $"❌ Nätverksfel: {ex.Message}\n\nSe till att Ollama körs! Kör 'ollama serve' i terminalen.";
            }
            catch (TaskCanceledException)
            {
                return "❌ Timeout: Ollama tog för lång tid att svara. Försök igen.";
            }
            catch (Exception ex)
            {
                return $"❌ Fel: {ex.Message}";
            }
        }

        // Kontrollera om Ollama körs
        public static async Task<bool> IsOllamaRunning()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:11434/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Hämta lista över tillgängliga modeller
        public static async Task<string[]> GetAvailableModels()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:11434/api/tags");
                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("models", out JsonElement modelsArray))
                {
                    var models = new System.Collections.Generic.List<string>();
                    foreach (JsonElement model in modelsArray.EnumerateArray())
                    {
                        if (model.TryGetProperty("name", out JsonElement nameElement))
                        {
                            models.Add(nameElement.GetString() ?? "");
                        }
                    }
                    return models.ToArray();
                }

                return new string[0];
            }
            catch
            {
                return new string[0];
            }
        }
    }
}