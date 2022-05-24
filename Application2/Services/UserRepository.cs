using System.Collections.Concurrent;
using Application2.Models;

namespace Application2.Services
{
    public class UserRepository : IUserRepository
    {

        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();


        public void Add(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Id)) throw new ArgumentException(nameof(user.Id));
            if (string.IsNullOrWhiteSpace(user.Name)) throw new ArgumentException(nameof(user.Name));

            if (!_users.TryAdd(user.Id, user))
                throw new InvalidOperationException("Couldn't add the specified user.");
        }

        public User? Delete(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));

            if (!_users.TryRemove(id, out var user))
                return null;

            return user;
        }

        public User? Get(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));

            if (!_users.TryGetValue(id, out var user))
                return null;

            return user;
        }

        public IEnumerable<User> Get()
        {
            if(!_users.Any())
                return Enumerable.Empty<User>();

            return _users.Values;
        }
    }
}
