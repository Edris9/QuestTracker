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
            client.Timeout = TimeSpan.FromMinutes(2);
        }

        // 1. Generera quest description från titel
        public static async Task<string> GenerateQuestDescription(string questTitle)
        {
            try
            {
                string prompt = $@"Du är en Guild Advisor som skapar episka quest-beskrivningar.

                    Quest-titel: {questTitle}

                    Skapa en kort, episk beskrivning av detta uppdrag (max 3 meningar) på svenska.";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new { temperature = 0.8, max_tokens = 200 }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Fel vid generering av beskrivning.";

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                    return responseElement.GetString() ?? "";

                return "";
            }
            catch
            {
                return "Kunde inte generera beskrivning.";
            }
        }

        // 2. Föreslå prioritet baserat på titel och deadline
        public static async Task<string> SuggestPriority(string questTitle, DateTime dueDate)
        {
            try
            {
                int daysLeft = (dueDate - DateTime.Now).Days;

                string prompt = $@"Du är en Guild Advisor som bedömer quest-prioritet.

                    Quest: {questTitle}
                    Dagar kvar: {daysLeft}

                    Svara ENDAST med ETT ord: Hög, Medium eller Låg";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new { temperature = 0.3, max_tokens = 10 }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Medium";

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                {
                    string result = responseElement.GetString()?.Trim() ?? "Medium";
                    if (result.Contains("Hög") || result.Contains("hög")) return "Hög";
                    if (result.Contains("Låg") || result.Contains("låg")) return "Låg";
                    return "Medium";
                }

                return "Medium";
            }
            catch
            {
                return "Medium";
            }
        }

        // 3. Sammanfatta alla quests - heroisk briefing
        public static async Task<string> SummarizeQuests(int totalQuests, int completed, int pending, int nearDeadline)
        {
            try
            {
                string prompt = $@"Du är en Guild Advisor som ger heroiska briefings.

                    Statistik:
                    - Totalt quests: {totalQuests}
                    - Slutförda: {completed}
                    - Pågående: {pending}
                    - Nära deadline: {nearDeadline}

                    Ge en kort heroisk briefing (max 4 meningar) på svenska om hjältens situation.";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new { temperature = 0.7, max_tokens = 300 }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Kunde inte generera briefing.";

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                    return responseElement.GetString() ?? "";

                return "";
            }
            catch
            {
                return "Kunde inte generera briefing.";
            }
        }

        // Kontrollera om Ollama körs
        public static async Task<string> GenerateQuestSuggestion(string userInput)
        {
            try
            {
                string prompt = $@"Du är en Guild Advisor som hjälper hjältar att skapa quests.

                    Användaren säger: {userInput}

                    Ge förslag på quest-titel, beskrivning och prioritet på svenska.";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new { temperature = 0.7, max_tokens = 500 }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "❌ Ollama-fel";

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                    return responseElement.GetString() ?? "❌ Inget svar";

                return "❌ Kunde inte tolka svar";
            }
            catch
            {
                return "❌ Fel: Se till att Ollama körs (ollama serve)";
            }
        }

        public static async Task<string> AnalyzeQuests(int totalQuests, int completed, int pending, int nearDeadline)
        {
            try
            {
                string prompt = $@"Du är Guild Advisor. Analysera:
                - Totalt: {totalQuests}
                - Klara: {completed}
                - Pågående: {pending}
                - Nära deadline: {nearDeadline}

                Ge kort heroisk briefing på svenska (max 4 meningar).";

                var requestBody = new
                {
                    model = MODEL,
                    prompt = prompt,
                    stream = false,
                    options = new { temperature = 0.7, max_tokens = 300 }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(OLLAMA_URL, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "❌ Ollama-fel";

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                    return responseElement.GetString() ?? "❌ Inget svar";

                return "❌ Kunde inte tolka svar";
            }
            catch
            {
                return "❌ Fel: Se till att Ollama körs";
            }
        }

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
    }
}