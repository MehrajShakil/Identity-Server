using Identity_Server.Constants;
using Identity_Server.DTOs;
using Identity_Server.Entities;
using Identity_Server.Extensions;
using Identity_Server.Identity_Wrapper_Services;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace Identity_Server.Services;

public class AccountService : IAccountService
{

    #region Fields

    private readonly UserManagerWrapper userManager;
    private readonly SigninManagerWrapper LogInManager;
    private readonly RoleManagerWrapper<ApplicationUserRole> roleManager;
    private readonly IConfiguration configuration;
    private readonly ILogger<AccountService> logger;
    private readonly IEmailSender emailSender;
    private readonly IJwtTokenProvider jwtTokenProvider;

    #endregion

    #region Injecting services

    public AccountService(UserManagerWrapper userManager,
                           SigninManagerWrapper signInManager,
                           ILogger<AccountService> logger,
                           IEmailSender emailSender,
                           IJwtTokenProvider jwtTokenProvider,
                           RoleManagerWrapper<ApplicationUserRole> roleManager,
                           IConfiguration configuration)
    {
        this.userManager = userManager;
        this.LogInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
        this.jwtTokenProvider = jwtTokenProvider;
        this.roleManager = roleManager;
        this.configuration = configuration;
    }

    #endregion


    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userRequest)
    {

        UserLoginResponse response = await LogInManager.LoginAsync(userRequest);

        if (response.StatusCode == StatusCode.Succeeded)
        {   
            response.AccessToken = jwtTokenProvider.GetJwtAccessToken(response.Claims);
            response.RefreshToken = jwtTokenProvider.GetJwtRefreshToken();
            response.Claims = new();
        }

        return response;
    }
    
    public async Task<UserRegistrationResponse> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
    {
        var user = new ApplicationUser
        {
            UserName = userRegistrationRequest.UserName,
            Email = userRegistrationRequest.Email
        };

        var response = await userManager.RegisterAsync(userRegistrationRequest);

        if(response.StatusCode != StatusCode.Succeeded)
        {
            return response;
        }

        var confirmationUrl =  await GenerateEmailConfirmationUrl(user);

        MailData mailData = GenerateEmailConfirmatinMailData(userRegistrationRequest, confirmationUrl);

        var sendMail = await emailSender.SendEmailAsync(mailData);

        if (!sendMail)
        {
            response.Messages.Add(Account.EmailSendingMessages.Failed);
        }
        else
        {
            response.Messages.Add(Account.RegistrationMessages.CheckEmailToVerifyAccount);
        }

        return response;
    }

    public async Task<UserConfirmationEmailResponse> ConfirmEmailAsync(string token, string email)
    {
         var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            return new UserConfirmationEmailResponse { Messages = new List<string> { "User is not found!" } };
        }
        IdentityResult confirmEmail = await userManager.ConfirmEmailAsync(user, token);
        if (!confirmEmail.Succeeded)
        {
            return new UserConfirmationEmailResponse { Messages = new List<string> { "Failed to Confirm Email." } };
        }
        return new UserConfirmationEmailResponse { Messages = new List<string> { "Confirmation Successfull." } };
    }


    private async Task<string> GenerateEmailConfirmationUrl(ApplicationUser user)
    {
        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedEmail = System.Web.HttpUtility.UrlEncode(user.Email);
        string url = $"{configuration["clientUrl"]}/confirmEmail?token={confirmationToken}&email={encodedEmail}";
        return url;
    }

    private static MailData GenerateEmailConfirmatinMailData(UserRegistrationRequest userRegistrationRequest, string confirmationUrl)
    {
        return new MailData
        {
            ReceiverEmail = userRegistrationRequest.Email,
            Subject = "Email Confirmation",
            Body = $"Please Follow the link to confirm your account: {confirmationUrl}"
        };
    }
}
