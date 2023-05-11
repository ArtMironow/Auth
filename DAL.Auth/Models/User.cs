using Microsoft.AspNetCore.Identity;

namespace DAL.Auth.Models
{
    public class User : IdentityUser
    {
        public string? Nickname { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
