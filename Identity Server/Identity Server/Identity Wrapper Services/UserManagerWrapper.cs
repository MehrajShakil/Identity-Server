using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity_Server.Identity_Wrapper_Services;

public class UserManagerWrapper<TUser> : UserManager<TUser> where TUser : class
{
    public UserManagerWrapper(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {


    }
}
