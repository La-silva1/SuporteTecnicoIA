using System;

namespace ApiCadastro.Models
{
    public class User
    {
        public int ID { get; set; } // PK

        public string Email { get; set; } = string.Empty; // Único
        public string PasswordHash { get; set; } = string.Empty; // Hash da senha
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Campos de Endereço
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty; 
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
    }
}
