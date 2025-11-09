using System.ComponentModel.DataAnnotations;

namespace ApiCadastro.Models
{
    public class TicketAvaliacaoRequest
    {
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Nota { get; set; }

        public string Comentario { get; set; } = string.Empty;
    }
}
