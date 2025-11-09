using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCadastro.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public string RespostaIA { get; set; } = string.Empty;

        public string Status { get; set; } = "Aberto";

        public DateTime DataCriacao { get; set; }

        public int? NotaAvaliacao { get; set; }

        public string ComentarioAvaliacao { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}


