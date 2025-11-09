using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ApiCadastro.Service
{
    public class AIService : IAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public AIService(IConfiguration configuration)
        {
            // Obtém a chave da API Gemini do appsettings.json
            _apiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini:ApiKey não configurada. Verifique appsettings.json ou appsettings.Development.json.");

            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            // Endpoint do modelo Gemini 2.5 Flash
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var fullPrompt = $"Você é um assistente técnico. Gere uma resposta clara, empática e útil para o usuário com base no problema: '{prompt}'";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = fullPrompt }
                        }
                    }
                }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Retorna o erro completo para facilitar o debug
                    return $"Erro ao se comunicar com o Gemini ({response.StatusCode}): {jsonResponse}";
                }

                using var doc = JsonDocument.Parse(jsonResponse);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "A IA não retornou uma resposta.";
            }
            catch (Exception ex)
            {
                // Captura erros de rede ou de parsing
                return $"Erro ao se comunicar com o Gemini: {ex.Message}";
            }
        }
    }
}
