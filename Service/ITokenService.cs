using ApiCadastro.Models;
using System.Threading.Tasks;

namespace ApiCadastro.Service
{

    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
