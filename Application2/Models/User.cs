namespace Application2.Models
{
    public class User
    {
        public User(string id, string name, DateTime? timestamp = null)
        {
            Id = id;
            Name = name;
            Timestamp = timestamp ?? DateTime.UtcNow;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
