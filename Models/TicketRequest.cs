using System.ComponentModel.DataAnnotations;

namespace ApiCadastro.Models
{
    public class TicketRequest
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;
    }
}
    