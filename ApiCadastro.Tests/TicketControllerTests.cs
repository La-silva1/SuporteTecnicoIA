using ApiCadastro.Models; 

namespace ApiCadastro.Tests
{
    // DTOs exclusivos para testes ou respostas que não existem em ApiCadastro.Models.
    public class ErrorResponse 
    { 
        public string Message { get; set; } = string.Empty; 
    }
    
    public class AvaliarChamadoResponse
    {
        public string Message { get; set; } = string.Empty;
        public int NotaAvaliacao { get; set; }
        public string ComentarioAvaliacao { get; set; } = string.Empty;
    }
}
