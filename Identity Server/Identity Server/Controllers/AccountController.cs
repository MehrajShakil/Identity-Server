using Identity_Server.Entities;
using Identity_Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

public class AccountController : BaseController
{
    private readonly UserManager<IdentityUser<int>> userManager;
    private readonly SignInManager<IdentityUser<int>> signInManager;
    private readonly ILogger<AccountController> logger;
    private readonly IEmailSender emailSender;

    public AccountController(UserManager<IdentityUser<int>> userManager,
                             SignInManager<IdentityUser<int>> signInManager,
                             ILogger<AccountController> logger,
                             IEmailSender emailSender)
	{
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
        this.emailSender = emailSender;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserRequest userRequest)
    {
        var user = new IdentityUser<int>
        {
            UserName = userRequest.UserName,
            Email = userRequest.Email
        };

        var userCreated = await userManager.CreateAsync(user, userRequest.Password);
        var verifiedEmail = await userManager.IsEmailConfirmedAsync(user);

        var mailData = new MailData
        {
            Sender = "identity@gmail.com",
            Receiver = userRequest.Email,
            Subject = "Email Confirmation",
            Body = "Hi Please confirm your mail"
        };

        await emailSender.SendEmailAsync(mailData);


        if (!userCreated.Succeeded)
        {
            logger.LogError($"Failed to Register user: Email: {user.Email}");
            return BadRequest("Failed to Register User!");
        }


        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

        

        return Ok("User Successfully Registered!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserRequest userRequest)
    {

        var user = new IdentityUser<int>
        {
            Email = userRequest.Email,
        };

        var verifiedEmail = await userManager.IsEmailConfirmedAsync(user);

        if (!verifiedEmail)
        {
            logger.LogError($"Email: {userRequest.Email} is not verified");
            return Unauthorized("Please! Verify your email before login.");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, userRequest.Password, false);

        if (!signInResult.Succeeded)
        {
            //NB:: For wrong Credentials status code should be 401(Unauthorized). 
            logger.LogError($"Wrong Credentials: Email {userRequest.Email}");
            return Unauthorized("Wrong Credentials!");
        }

        var token = await userManager.GenerateUserTokenAsync(user, "", "");

        return Ok("Successfully login");
    }

}
