using Microsoft.AspNetCore.Identity;
namespace TIKNA.Models
{
    public class ApplicationUser:IdentityUser
    {
        public Customer? Customer { get; set; }
    }

}

