using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ApiCadastro.Models;

namespace ApiCadastro.Tests
{
    public class SecurityTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public SecurityTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // Gera um token JWT válido simulando o fluxo real de registro e login
        private async Task<string> GetValidToken()
        {
            using var client = _factory.CreateClient();
            var uniqueEmail = $"security-test-{System.Guid.NewGuid()}@api-cadastro.com";
            var password = "Password123!";

            // 1. Registro de usuário
            var registerRequest = new RegisterRequest 
            { 
                Email = uniqueEmail, 
                Senha = password, 
                Nome = "Security Test User", 
                CEP = "00000-000" 
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerRequest),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            await client.PostAsync("/crie-uma-conta", registerContent);

            // 2. Login para obter o token JWT
            var loginRequest = new LoginRequest { Email = uniqueEmail, Senha = password };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            var response = await client.PostAsync("/entrar", loginContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return authResponse?.Token ?? string.Empty;
        }

        [Theory]
        [InlineData("/abrir-chamado")] // Endpoint protegido (POST)
        [InlineData("/meus-chamados")] // Endpoint protegido (GET)
        public async Task SecuredEndpoints_ShouldReturnUnauthorized_WhenNoTokenIsProvided(string url)
        {
            // Arrange
            var request = new TicketRequest { Titulo = "Título", Descricao = "Descrição" };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            // Act
            HttpResponseMessage response = url.StartsWith("/abrir-chamado")
                ? await _client.PostAsync(url, content)
                : await _client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SecuredEndpoint_ShouldReturnOk_WhenValidTokenIsProvided()
        {
            // Arrange
            var token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new TicketRequest 
            { 
                Titulo = "Chamado Autorizado", 
                Descricao = "Teste de autorização OK." 
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            // Act
            var response = await _client.PostAsync("/abrir-chamado", content);

            // Assert
            Assert.True(
                response.IsSuccessStatusCode,
                $"Esperado sucesso, mas recebeu {response.StatusCode}. Resposta: {await response.Content.ReadAsStringAsync()}"
            );
        }
    }
}
