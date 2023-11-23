using System.Security.Claims;
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

        var dbUser = await userManager.FindByEmailAsync(userRequest.Email);
        if (dbUser is not null)
        {
            dbUser.RefreshToken = response.RefreshToken;
            dbUser.RefreshTokenExpires = DateTime.Now;
            await userManager.UpdateAsync(dbUser);
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

        var body = $"Please Follow the link to confirm your account: {confirmationUrl}";
        var subject = "Email confirmation";

        MailData mailData = GenerateEmailConfirmationMailData(userRegistrationRequest.Email, subject, body);

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

    public async Task<UserRefreshTokenResponse> GetAccessTokenByRefreshToken(UserRefreshTokenRequest userRefreshTokenRequest)
    {
        UserRefreshTokenResponse response = new();
        var user = await userManager.FindByEmailAsync(userRefreshTokenRequest.Email);
        if (user is null)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = new List<string> { Account.UserNotFound };
            return response;
        }

        if (user.CanRefreshToken(userRefreshTokenRequest.RefreshToken))
        {
            response.StatusCode = StatusCode.Succeeded;
            response.AccessToken = jwtTokenProvider.GetJwtAccessToken(new List<Claim>());
            response.RefreshToken = jwtTokenProvider.GetJwtRefreshToken();
            response.RefreshTokenExpires = DateTime.Now;
            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpires = response.RefreshTokenExpires;
            await userManager.UpdateAsync(user);
        }
        else
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = new List<string> { "Revoked" };
        }

        return response;
    }

    public async Task<UserResetPasswordResponse> ResetUserPassword(UserResetPasswordRequest userResetPasswordRequest)
    {
        UserResetPasswordResponse response = new();
        var user = await userManager.FindByEmailAsync(userResetPasswordRequest.Email);

        if (user is null)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = [Account.UserNotFound]; 
            return response;
        }

        var confirmationUrl = await GenerateResetPasswordEmailConfirmationUrl(user);
        var body= $"Please click into the following link to reset your password: {confirmationUrl}";
        var subject = "Reset Password Email Confirmation";
        var mailData = GenerateEmailConfirmationMailData(user.Email, subject, body);

        if (await emailSender.SendEmailAsync(mailData))
        {
            response.StatusCode = StatusCode.Succeeded;
            response.Messages = ["Please check your email to confirm your email address and reset your password"];
        }
        else
        {
            response.StatusCode = StatusCode.BadGateWay;
            response.Messages = [Account.EmailSendingMessages.Failed];
        }

        return response;
    }

    /*
    public async Task<UserResetPasswordResponse> VerifyEmailForResetPassword(string token, string email)
    {
        UserResetPasswordResponse response = new();
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            response.StatusCode = StatusCode.Unauthorized;
            response.Messages = [Account.UserNotFound];
            return response;
        }

        if(await userManager.)

    }*/

    private async Task<string> GenerateEmailConfirmationUrl(ApplicationUser user)
    {
        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedEmail = System.Web.HttpUtility.UrlEncode(user.Email);
        string url = $"{configuration["clientUrl"]}/api/account/confirmEmail?token={confirmationToken}&email={encodedEmail}";
        return url;
    }

    private async Task<string> GenerateResetPasswordEmailConfirmationUrl(ApplicationUser user)
    {
        var confirmationToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedEmail = System.Web.HttpUtility.UrlEncode(user.Email);
        string url = $"{configuration["clientUrl"]}/api/account/resetPasswordConfirmEmail?token={confirmationToken}&email={encodedEmail}";
        return url;
    }

    private static MailData GenerateEmailConfirmationMailData(string email, string subject, string body)
    {
        return new MailData
        {
            ReceiverEmail = email,
            Subject = subject,
            Body = body
        };
    }


}
