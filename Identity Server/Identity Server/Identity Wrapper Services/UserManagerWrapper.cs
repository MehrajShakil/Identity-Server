using Identity_Server.Constants;
using Identity_Server.DTOs;
using Identity_Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity_Server.Identity_Wrapper_Services;

public class UserManagerWrapper : UserManager<ApplicationUser>
{
    public UserManagerWrapper(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {


    }

    public async Task<UserRegistrationResponse> RegisterAsync(UserRegistrationRequest userRegistrationRequest)
    {

        var response = new UserRegistrationResponse();

        var user = new ApplicationUser
        {
            UserName = userRegistrationRequest.UserName,
            Email = userRegistrationRequest.Email
        };


        var userExist = await base.FindByEmailAsync(userRegistrationRequest.Email);

        if (userExist is not null)
        {
            response.StatusCode = StatusCode.Conflict;
            response.Messages = new List<string> { Account.RegistrationMessages.EmailExist };
            return response;
        }


        var userCreated = await base.CreateAsync(user, userRegistrationRequest.Password);

        if (!userCreated.Succeeded)
        {
            response.StatusCode = StatusCode.Unauthorized;
            var errors = new List<string>();
            foreach (var error in userCreated.Errors)
            {
                errors.Add(error.Description);
            }
            response.Messages = errors;
        }
        else
        {
            response.StatusCode = StatusCode.Succeeded;
            response.Messages = new List<string> { Account.RegistrationMessages.UserCreatedSuccessfully };
        }

        return response;

    }

}
