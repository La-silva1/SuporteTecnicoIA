using System;
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
    public class LoginControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public LoginControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // Gera um usuário com e-mail único para evitar conflitos entre testes
        private (string email, string password) GenerateUniqueUser()
        {
            var uniqueEmail = $"test-{Guid.NewGuid()}@api-cadastro.com";
            var password = "Password123!";
            return (uniqueEmail, password);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsCreated()
        {
            // Arrange
            var (email, password) = GenerateUniqueUser();
            var request = new RegisterRequest 
            { 
                Email = email, 
                Senha = password, 
                Nome = "Test Register", 
                CEP = "00000-000" 
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            // Act
            var response = await _client.PostAsync("/crie-uma-conta", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            using var client = _factory.CreateClient();
            var (email, password) = GenerateUniqueUser();

            // 1. Registro
            var registerRequest = new RegisterRequest 
            { 
                Email = email, 
                Senha = password, 
                Nome = "Test Login", 
                CEP = "00000-000" 
            };

            var registerContent = new StringContent(
                JsonSerializer.Serialize(registerRequest),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            var registerResponse = await client.PostAsync("/crie-uma-conta", registerContent);
            registerResponse.EnsureSuccessStatusCode();

            // 2. Login
            var loginRequest = new LoginRequest { Email = email, Senha = password };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            // Act
            var response = await client.PostAsync("/entrar", loginContent);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(authResponse);
            Assert.False(string.IsNullOrEmpty(authResponse.Token));
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "nonexistent@user.com", 
                Senha = "wrongpassword" 
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            );

            // Act
            var response = await _client.PostAsync("/entrar", loginContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
