namespace Vxp.Services.Data.Tests
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Vxp.Data.Models;

    public class FakeRoleManager : RoleManager<ApplicationRole>
    {
        public FakeRoleManager() :
            base(
                new Mock<IRoleStore<ApplicationRole>>().Object,
                new IRoleValidator<ApplicationRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<ApplicationRole>>>().Object)
        {
        }
    }
}