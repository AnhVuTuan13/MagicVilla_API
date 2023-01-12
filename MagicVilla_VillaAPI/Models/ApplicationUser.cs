using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;

namespace MagicVilla_VillaAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
