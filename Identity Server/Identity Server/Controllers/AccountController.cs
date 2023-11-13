using Identity_Server.DTOs;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService accountService;

    public AccountController(IAccountService accountService)
	{
        this.accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserRegistrationRequest userRequest)
    {
        var response = await accountService.RegisterUserAsync(userRequest);
        
        ///TODO:: need to handle the return type for status code.

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest userRequest)
    {
        var response = await accountService.LoginAsync(userRequest);

        ///TODO:: need to handle the return type for status code 

        return Ok(response);
    }

}
