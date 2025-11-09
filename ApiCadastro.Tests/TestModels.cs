using ApiCadastro.Models;

namespace ApiCadastro.Tests
{
    // Modelo simplificado para deserializar respostas de autenticação nos testes
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    // Modelo usado para validar a resposta de criação de chamados nos testes
    public class CriarChamadoResponse
    {
        public Ticket Ticket { get; set; } = new Ticket();
        public string Message { get; set; } = string.Empty;
    }
}
