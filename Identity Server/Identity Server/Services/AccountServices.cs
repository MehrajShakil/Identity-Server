using Identity_Server.DTOs;
using Identity_Server.Entities;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Services;

public class AccountServices : IAccountService
{

    #region Fields

    private readonly UserManager<IdentityUser<int>> userManager;
    private readonly SignInManager<IdentityUser<int>> signInManager;
    private readonly ILogger<AccountServices> logger;
    private readonly IEmailSender emailSender;

    #endregion

    #region Injecting services

    public AccountServices(UserManager<IdentityUser<int>> userManager,
                           SignInManager<IdentityUser<int>> signInManager,
                           ILogger<AccountServices> logger,
                           IEmailSender emailSender)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
    }

    #endregion


    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest user)
    {
        throw new NotImplementedException();
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
