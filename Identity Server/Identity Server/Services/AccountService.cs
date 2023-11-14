using Identity_Server.DTOs;
using Identity_Server.Entities;
using Identity_Server.Identity_Wrapper_Services;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Services;

public class AccountService : IAccountService
{

    #region Fields

    private readonly UserManagerWrapper<IdentityUser<int>> userManager;
    private readonly SigninManagerWrapper<IdentityUser<int>> signInManager;
    private readonly ILogger<AccountService> logger;
    private readonly IEmailSender emailSender;

    #endregion

    #region Injecting services

    public AccountService(UserManagerWrapper<IdentityUser<int>> userManager,
                           SigninManagerWrapper<IdentityUser<int>> signInManager,
                           ILogger<AccountService> logger,
                           IEmailSender emailSender)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
    }

    #endregion


    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userRequest)
    {
        var user = new IdentityUser<int>()
        {
            Email = userRequest.Email
        };

        UserLoginResponse response = new();

        bool accountVerified = await signInManager.CanSignInAsync(user);
        if (!accountVerified)
        {
            response.StatusCode = "401";
            response.Message = "Your account is not verified. Please verify your account for Login.";
            return response;
        }

        SignInResult canLogIn =  await signInManager.CheckPasswordSignInAsync(user, userRequest.Password, false);

        if (!canLogIn.Succeeded)
        {
            response.StatusCode = "401";
            response.Message = "Failed to SignIn";
            return response;
        }

        response.Message = "Successfully SignIn!";

        return response;
    }

    
    public async Task<UserRegistrationResponse> RegisterUserAsync(UserRegistrationRequest userRegistrationRequest)
    {
        var user = new IdentityUser<int>
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
                response.StatusCode = error.Code;
                response.Message = error.Description;
                logger.LogError($"Status Code: {error.Code}, Message: {error.Description}");
            }

            return response;
        }

        MailData mailData = GenerateEmailConfirmatinMail(userRegistrationRequest);

        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

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

    private static MailData GenerateEmailConfirmatinMail(UserRegistrationRequest userRegistrationRequest)
    {
        return new MailData
        {
            Sender = "identity@gmail.com",
            Receiver = userRegistrationRequest.Email,
            Subject = "Email Confirmation",
            Body = "Hi Please confirm your mail"
        };
    }
}
