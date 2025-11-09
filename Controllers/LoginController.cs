using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCadastro.Data;
using ApiCadastro.Models;
using ApiCadastro.Service;

namespace ApiCadastro.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public LoginController(AppDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("crie-uma-conta")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest(new ErrorResponse { Message = "Este e-mail já está em uso." });

            string passwordHash = _passwordHasher.HashPassword(request.Senha);

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Nome = request.Nome,
                Telefone = request.Telefone,
                Logradouro = request.Logradouro,
                Numero = request.Numero,
                Complemento = request.Complemento,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                CEP = request.CEP
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(newUser);
            var response = new AuthResponse
            {
                ID = newUser.ID,
                Nome = newUser.Nome,
                Email = newUser.Email,
                Token = token
            };

            return Ok(response);
        }

        [HttpPost("entrar")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new ErrorResponse { Message = "E-mail ou senha inválidos." });

            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Senha, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized(new ErrorResponse { Message = "E-mail ou senha inválidos." });

            var token = _tokenService.GenerateToken(user);
            var response = new AuthResponse
            {
                ID = user.ID,
                Nome = user.Nome,
                Email = user.Email,
                Token = token
            };

            return Ok(response);
        }
    }
}
