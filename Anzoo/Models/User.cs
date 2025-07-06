using Microsoft.AspNetCore.Identity;
namespace Anzoo.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Location { get; set; }
        public int Points { get; set; } = 0; // ✅ setăm default la 0
    }
}
