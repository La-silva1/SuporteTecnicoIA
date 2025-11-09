using System;

namespace ApiCadastro.Models
{
    public class AuthResponse
    {
        public int ID { get; set; } // Corresponde ao User.ID
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
