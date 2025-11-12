using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCadastro.Data;
using ApiCadastro.Models;
using ApiCadastro.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiCadastro.Controllers
{
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAIService _aiService;

        public TicketController(AppDbContext context, IAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // Criar novo ticket
        [Authorize]
        [HttpPost("abrir-chamado")]
        public async Task<IActionResult> CriarChamado([FromBody] TicketRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = new Ticket
            {
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                UserId = int.Parse(userId ?? "0"),
                DataCriacao = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Gera resposta automática via IA
            var respostaIA = await _aiService.GenerateContentAsync(request.Descricao);
            ticket.RespostaIA = respostaIA;
            ticket.Status = "Respondido"; // Atualiza o status do chamado para respondido
            Console.WriteLine($"Ticket Status before second save: {ticket.Status}");
            await _context.SaveChangesAsync();

            return Ok(new CriarChamadoResponse
            {
                Message = "Chamado criado com sucesso!",
                Ticket = ticket
            });
        }

        // Listar tickets do usuário autenticado
        [Authorize]
        [HttpGet("meus-chamados")]
        public async Task<IActionResult> MeusChamados()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = await _context.Tickets
                .Where(t => t.UserId.ToString() == userId)
                .ToListAsync();

            return Ok(tickets);
        }

        [Authorize]
        [HttpPost("avaliar-chamado/{id}")]
        public async Task<IActionResult> AvaliarChamado(int id, [FromBody] TicketAvaliacaoRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound(new ErrorResponse { Message = "Chamado não encontrado." });

            if (ticket.UserId.ToString() != userId)
                return Forbid("Você só pode avaliar seus próprios chamados.");

            if (ticket.Status == "Aberto")
                return BadRequest(new ErrorResponse { Message = "O chamado ainda não foi respondido e não pode ser avaliado." });

            ticket.NotaAvaliacao = request.Nota;
            ticket.ComentarioAvaliacao = request.Comentario;
            await _context.SaveChangesAsync();

            return Ok(new AvaliarChamadoResponse
            {
                Message = "Avaliação registrada com sucesso!",
                Id = ticket.Id,
                NotaAvaliacao = ticket.NotaAvaliacao,
                ComentarioAvaliacao = ticket.ComentarioAvaliacao
            });
        }

    }
}
