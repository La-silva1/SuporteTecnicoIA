using System.ComponentModel.DataAnnotations;

namespace ApiCadastro.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Senha { get; set; } = string.Empty; // Campo usado no Controller
        [Required]
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Campos de Endereço
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        [Required]
        public string CEP { get; set; } = string.Empty;
    }
}
