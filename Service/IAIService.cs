using System.Threading.Tasks;

namespace ApiCadastro.Service
{
    public interface IAIService
    {
        Task<string> GenerateContentAsync(string prompt);
    }
}
