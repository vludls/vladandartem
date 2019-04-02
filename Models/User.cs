using Microsoft.AspNetCore.Identity;

namespace vladandartem.Models
{    
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}