using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Identity_Wrapper_Services
{
    public class RoleManagerWrapper<TRole> : RoleManager<TRole> where TRole : class
    {
        public RoleManagerWrapper(IRoleStore<TRole> store, IEnumerable<IRoleValidator<TRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<TRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
