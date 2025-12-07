using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatAI
{
    public class AiService
    {
        private readonly string _apiKey = "SK-KEY-IDE";

        private readonly List<string> _badWords = new List<string>
        {
            "hülye", "béna", "barom", "idióta", "köcsög"
        };

        public bool ContainsBadWord(string message)
        {
            foreach (var word in _badWords)
            {
                if (message.Contains(word, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> GetAnswerAsync(string userMessage)
        {
            if (_apiKey == "SK-KEY-IDE")
            {
                return "Szia! Én a ChatBot vagyok. Még nincs beállítva az API kulcsom.";
            }

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var requestData = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = "Te egy segítőkész chat bot vagy." },
                        new { role = "user", content = userMessage }
                    }
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode) return "Hiba az AI elérésében.";

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);

                if (result.choices != null && result.choices.Count > 0)
                {
                    string answer = result.choices[0].message.content;
                    return answer;
                }
                return "Nem kaptam választ az AI-tól.";
            }
            catch (Exception ex)
            {
                return $"Hiba: {ex.Message}";
            }
        }
    }
}