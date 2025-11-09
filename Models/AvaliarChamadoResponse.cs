namespace ApiCadastro.Models
{
    public class AvaliarChamadoResponse
    {
        public string Message { get; set; }
        public int Id { get; set; }
        public int? NotaAvaliacao { get; set; }
        public string ComentarioAvaliacao { get; set; }
    }
}
