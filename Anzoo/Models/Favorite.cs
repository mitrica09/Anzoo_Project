namespace Anzoo.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int AdId { get; set; }
        public Ad Ad { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
