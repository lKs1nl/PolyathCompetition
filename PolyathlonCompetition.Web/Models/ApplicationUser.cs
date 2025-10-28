using Microsoft.AspNetCore.Identity;

namespace PolyathlonCompetition.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }
        
        [PersonalData]
        public string? RoleType { get; set; } // "Judge", "Organizer", "Admin"
        
        [PersonalData]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}