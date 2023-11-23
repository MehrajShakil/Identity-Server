using Identity_Server.DTOs;
using Identity_Server.Entities;
using Identity_Server.Helpers;
using Identity_Server.Interfaces;
using Identity_Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

public class AccountController : BaseController
{

    #region Fields

    private readonly IAccountService accountService;
    private readonly IEmailSender _emailSender;

    #endregion

    #region Injecting Services

    public AccountController(IAccountService accountService, IEmailSender emailSender)
    {
        _emailSender = emailSender;
        this.accountService = accountService;
    }

    #endregion


    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserRegistrationRequest userRequest)
    {
        UserRegistrationResponse response = await accountService.RegisterUserAsync(userRequest);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

    [AllowAnonymous]
    [HttpPost("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        string decodedEmail = System.Net.WebUtility.UrlDecode(email);

        UserConfirmationEmailResponse response = await accountService.ConfirmEmailAsync(token, decodedEmail);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest userRequest)
    {
        UserLoginResponse response = await accountService.LoginAsync(userRequest);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

    [AllowAnonymous]
    [HttpGet("refreshToken")]
    public async Task<IActionResult> GetAccessTokenByRefreshToken(UserRefreshTokenRequest userRefreshTokenRequest)
    {
        UserRefreshTokenResponse response = await accountService.GetAccessTokenByRefreshToken(userRefreshTokenRequest);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

    [AllowAnonymous]
    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword(UserResetPasswordRequest userResetPasswordRequest)
    {
        UserResetPasswordResponse response = await accountService.ResetUserPassword(userResetPasswordRequest);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

    [AllowAnonymous]
    [HttpPost("sendEmail")]
    public async Task<IActionResult> TestEmailSend(string email, string subject, string body)
    {

        var mailData = new MailData()
        {
            ReceiverEmail = email,
            Subject = subject,
            Body = body
        };

        bool send = await _emailSender.SendEmailAsync(mailData);
        return Ok();
    }

    /*[AllowAnonymous]
    [HttpPost("resetPasswordConfirmEmail")]
    public async Task<IActionResult> VerifyEmailForResetPassword(string token, string email)
    {

    }*/

}
