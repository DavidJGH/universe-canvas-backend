using System.Threading.Tasks;
using universe_canvas.Models;

namespace universe_canvas.Repositories;

public interface IUserRepository
{
    Task<bool> Register(User user, string password);
    Task<Tokens> Authenticate(User user, string password);
}
