using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Security;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public ClientProfile? ClientProfile { get; set; }

        public ICollection<UserSession> Sessions { get; set; } = new HashSet<UserSession>();
    }
}
