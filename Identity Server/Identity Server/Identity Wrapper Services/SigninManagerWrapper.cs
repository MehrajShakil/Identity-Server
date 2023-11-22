using Identity_Server.Constants;
using Identity_Server.DTOs;
using Identity_Server.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Identity_Server.Identity_Wrapper_Services;

public class SigninManagerWrapper : SignInManager<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> userManager;

    public SigninManagerWrapper(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<ApplicationUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        this.userManager = userManager;
    }

    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userRequest)
    {
        UserLoginResponse response = new()
        {
            Email = userRequest.Email
        };

        var user = await userManager.FindByEmailAsync(userRequest.Email);
        if (user is null)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = new List<string> { Account.LogInMessages.RegisterAccountRequired };
            return response;
        }

        bool accountVerified = await base.CanSignInAsync(user);
        if (!accountVerified)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = new List<string> { Account.LogInMessages.EmailVerificationRequired };
            return response;
        }

        SignInResult canLogIn = await base.CheckPasswordSignInAsync(user, userRequest.Password, false);

        if (!canLogIn.Succeeded)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = new List<string> { Account.LogInMessages.WrongCredentials };
            return response;
        }

        response.UserName = user.UserName;
        response.StatusCode = StatusCode.Succeeded;
        response.Messages = new List<string> { Account.LogInMessages.LogInSuccess };
        response.Claims = await userManager.GetClaimsAsync(user) as List<Claim> ?? new List<Claim>();

        return response;
    }

}
