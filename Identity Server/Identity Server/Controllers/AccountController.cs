﻿using Identity_Server.DTOs;
using Identity_Server.Helpers;
using Identity_Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

public class AccountController : BaseController
{

    #region Fields

    private readonly IAccountService accountService;

    #endregion

    #region Injecting Services

    public AccountController(IAccountService accountService)
	{
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
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest userRequest)
    {
        UserLoginResponse response = await accountService.LoginAsync(userRequest);

        return ActionResutlHelper
            .ReturnActionResult(response, response.StatusCode);
    }

}
