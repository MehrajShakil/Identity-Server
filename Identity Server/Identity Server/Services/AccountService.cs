using Identity_Server.DTOs;
using Identity_Server.Entities;
using Identity_Server.Extensions;
using Identity_Server.Identity_Wrapper_Services;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Security.Claims;

namespace Identity_Server.Services;

public class AccountService : IAccountService
{

    #region Fields

    private readonly UserManagerWrapper<ApplicationUser> userManager;
    private readonly SigninManagerWrapper<ApplicationUser> signInManager;
    private readonly RoleManagerWrapper<ApplicationUserRole> roleManager;
    private readonly ILogger<AccountService> logger;
    private readonly IEmailSender emailSender;
    private readonly IJwtTokenProvider jwtTokenProvider;

    #endregion

    #region Injecting services

    public AccountService(UserManagerWrapper<ApplicationUser> userManager,
                           SigninManagerWrapper<ApplicationUser> signInManager,
                           ILogger<AccountService> logger,
                           IEmailSender emailSender,
                           IJwtTokenProvider jwtTokenProvider,
                           RoleManagerWrapper<ApplicationUserRole> roleManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
        this.jwtTokenProvider = jwtTokenProvider;
        this.roleManager = roleManager;
    }

    #endregion


    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userRequest)
    {

        var user = await userManager.FindByEmailAsync(userRequest.Email);

        UserLoginResponse response = new();

        bool accountVerified = await signInManager.CanSignInAsync(user);
        if (!accountVerified)
        {
            response.StatusCode = 401;
            response.Message = "Your account is not verified. Please verify your account for Login.";
            return response;
        }

        SignInResult canLogIn =  await signInManager.CheckPasswordSignInAsync(user, userRequest.Password, false);

        if (!canLogIn.Succeeded)
        {
            response.StatusCode = 401;
            response.Message = "Failed to SignIn";
            return response;
        }

        var claims = await userManager.GetClaimsAsync(user) as List<Claim>;

        response.AccessKey = jwtTokenProvider.GetJwtToken(claims);

        response.Message = "Successfully SignIn!";
        return response;
    }

    
    public async Task<UserRegistrationResponse> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
    {
        var user = new ApplicationUser
        {
            UserName = userRegistrationRequest.UserName,
            Email = userRegistrationRequest.Email
        };

        var response = new UserRegistrationResponse();

        /*

        I this It will be handled by Create Asysnc.

                var userExist =  await userManager.FindByEmailAsync(userRegistrationRequest.Email);

                if(userExist is not null)
                {
                    response.StatusCode = "409"; // status code for conflict
                    response.Message = "User Already exist with this email.";
                    return response;
                }
        */

        var userCreated = await userManager.CreateAsync(user, userRegistrationRequest.Password);

        if (!userCreated.Succeeded)
        {
            foreach (var error in userCreated.Errors)
            {
                response.StatusCode = 401;
                response.Message = error.Description;
                logger.LogError($"Status Code: {error.Code}, Message: {error.Description}");
            }

            return response;
        }


        var confirmationUrl =  await GenerateConfirmationUrl(user);

        MailData mailData = GenerateEmailConfirmatinMail(userRegistrationRequest, confirmationUrl);


        var sendMail = await emailSender.SendEmailAsync(mailData);

        if (!sendMail)
        {
            response.Message = "Failed to send confirmation mail";
        }
        else
        {
            response.Message = $"Please check your mail to verify your email";
        }

        return response;
    }

    public async Task<UserConfirmationEmailResponse> ConfirmEmailAsync(string token, string email)
    {
         var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return new UserConfirmationEmailResponse { Message = "User is not found!"};
        }
        IdentityResult confirmEmail = await userManager.ConfirmEmailAsync(user, token);
        if (!confirmEmail.Succeeded)
        {
            return new UserConfirmationEmailResponse { Message = "Failed to Confirm Email."};
        }
        return new UserConfirmationEmailResponse { Message = "Confirmation Successfull."};
    }

    private async Task<string> GenerateConfirmationUrl(ApplicationUser user)
    {
        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedEmail = System.Web.HttpUtility.UrlEncode(user.Email);
        string url = $"{"clientUrl"}/confirmEmail?token={confirmationToken}&email={encodedEmail}";
        return url;
    }

    private static MailData GenerateEmailConfirmatinMail(UserRegistrationRequest userRegistrationRequest, string confirmationUrl)
    {
        return new MailData
        {
            ReceiverEmail = userRegistrationRequest.Email,
            Subject = "Email Confirmation",
            Body = $"Please Follow the link to confirm your account: {confirmationUrl}"
        };
    }
}
