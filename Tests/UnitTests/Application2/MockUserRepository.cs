using System.Collections.Generic;
using System.Linq;
using Application2.Models;
using Application2.Services;

namespace Tests.UnitTests.Application2
{
    public class MockUserRepository : IUserRepository
    {
        private readonly List<User> _users = new List<User>();
        public void Add(User user)
        {
            _users.Add(user);
        }

        public User? Delete(string id)
        {
            var user = Get(id);

            if (user == null) return null;

            if (_users.Remove(user)) return user;

            return null;
        }

        public User? Get(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public IEnumerable<User> Get()
        {
            return _users;
        }
    }
}
