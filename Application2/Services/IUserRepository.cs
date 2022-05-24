using System.Collections.Concurrent;
using Application2.Models;

namespace Application2.Services
{
    public interface IUserRepository
    {
        void Add(User user);

        User? Delete(string id);

        User? Get(string id);

        IEnumerable<User> Get();

    }
}
