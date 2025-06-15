namespace Anzoo.Models
{
    public class AdImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool IsMain { get; set; } = false;
        public int AdId { get; set; }
        public Ad Ad { get; set; }
    }
}
