namespace Vxp.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Data.Models;

    public class VxpUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {

        public VxpUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options) : base(userManager,
            roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("FullName", $"{user.FirstName} {user.LastName}"));
            return identity;
        }
    }

}