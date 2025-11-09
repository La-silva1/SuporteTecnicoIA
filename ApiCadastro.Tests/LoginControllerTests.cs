using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using ApiCadastro.Controllers;
using ApiCadastro.Data;
using ApiCadastro.Models; // ADICIONADO: Para resolver RegisterRequest e LoginRequest
using ApiCadastro.Service;

namespace ApiCadastro.Tests
{
    public class LoginControllerTests : IDisposable
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly AppDbContext _dbContext;

        public LoginControllerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new AppDbContext(dbContextOptions);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsCreated()
        {
            // Arrange
            var controller = new LoginController(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object);
            // Os DTOs agora são resolvidos para ApiCadastro.Models.RegisterRequest
            var request = new RegisterRequest { Email = "test@example.com", Senha = "password", Nome = "Test User", CEP = "00000-000" }; 

            _passwordHasherMock.Setup(p => p.HashPassword(request.Senha)).Returns("hashed_password");
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("test_token");

            // Act
            var result = await controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // AuthResponse está definido em TestModels.cs, no namespace ApiCadastro.Tests
            var authResponse = Assert.IsType<ApiCadastro.Models.AuthResponse>(okResult.Value);
            Assert.Equal("test_token", authResponse.Token);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailExists()
        {
            // Arrange
            var existingUser = new User { Email = "test@example.com", PasswordHash = "hashed_password", Nome = "Existing", CEP = "00000-000" };
            _dbContext.Users.Add(existingUser);
            await _dbContext.SaveChangesAsync();

            var controller = new LoginController(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object);
            var request = new RegisterRequest { Email = "test@example.com", Senha = "password", Nome = "Test User", CEP = "00000-000" };

            // Act
            var result = await controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // ErrorResponse é um modelo auxiliar de teste.
            var error = Assert.IsType<ApiCadastro.Models.ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Este e-mail já está em uso.", error.Message);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User { Email = "test@example.com", PasswordHash = "hashed_password", Nome = "Existing", CEP = "00000-000" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var controller = new LoginController(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object);
            var request = new LoginRequest { Email = "test@example.com", Senha = "password" };

            _passwordHasherMock.Setup(p => p.VerifyPassword(request.Senha, user.PasswordHash)).Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns("test_token");

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authResponse = Assert.IsType<ApiCadastro.Models.AuthResponse>(okResult.Value);
            Assert.Equal("test_token", authResponse.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var controller = new LoginController(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object);
            var request = new LoginRequest { Email = "test@example.com", Senha = "password" };

            // Act
            var result = await controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<ApiCadastro.Models.ErrorResponse>(unauthorizedResult.Value);
            Assert.Equal("E-mail ou senha inválidos.", error.Message);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var user = new User { Email = "test@example.com", PasswordHash = "hashed_password", Nome = "Existing", CEP = "00000-000" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var controller = new LoginController(_dbContext, _tokenServiceMock.Object, _passwordHasherMock.Object);
            var request = new LoginRequest { Email = "test@example.com", Senha = "wrong_password" };

            _passwordHasherMock.Setup(p => p.VerifyPassword(request.Senha, user.PasswordHash)).Returns(false);

            // Act
            var result = await controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var error = Assert.IsType<ApiCadastro.Models.ErrorResponse>(unauthorizedResult.Value);
            Assert.Equal("E-mail ou senha inválidos.", error.Message);
        }
    }
}
