using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiCadastro.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiCadastro.Tests
{
    public class TicketControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public TicketControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        
        private (string email, string password) GenerateUniqueUser()
        {
            var uniqueEmail = $"ticket-test-{Guid.NewGuid()}@api-cadastro.com";
            var password = "Password123!";
            return (uniqueEmail, password);
        }

        private async Task<string> GetValidToken()
        {
            using var client = _factory.CreateClient(); 
            var (email, password) = GenerateUniqueUser();
            
            var registerRequest = new RegisterRequest { Email = email, Senha = password, Nome = "Ticket Test User", CEP = "00000-000" };
            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerRequest), 
                Encoding.UTF8, 
                new MediaTypeHeaderValue("application/json")
            );
            var registerResponse = await client.PostAsync("/crie-uma-conta", registerContent);
            registerResponse.EnsureSuccessStatusCode(); 

            var loginRequest = new LoginRequest { Email = email, Senha = password };
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest), 
                Encoding.UTF8, 
                new MediaTypeHeaderValue("application/json")
            );
            var response = await client.PostAsync("/entrar", loginContent);
            response.EnsureSuccessStatusCode(); 
            var responseString = await response.Content.ReadAsStringAsync();
            
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
            {
                Assert.Fail($"Falha ao obter token durante o login. Resposta: {responseString}");
            }
            return authResponse.Token;
        }

        [Fact]
        public async Task CriarChamado_ShouldReturnOk_WhenTicketIsCreated()
        {
            var token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new TicketRequest { Titulo = "Novo Chamado Teste", Descricao = "Descrição detalhada do problema." };
            var content = new StringContent(
                JsonSerializer.Serialize(request), 
                Encoding.UTF8, 
                new MediaTypeHeaderValue("application/json")
            );

            var response = await _client.PostAsync("/abrir-chamado", content);

            response.EnsureSuccessStatusCode(); 
            var responseString = await response.Content.ReadAsStringAsync();
            
            var criarChamadoResponse = JsonSerializer.Deserialize<CriarChamadoResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(criarChamadoResponse);
            Assert.True(criarChamadoResponse.Ticket.Id > 0);
            Assert.Equal("Novo Chamado Teste", criarChamadoResponse.Ticket.Titulo);
        }
    }
}
