using Identity_Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

public class AccountController : BaseController
{
    private readonly UserManager<IdentityUser<int>> userManager;
    private readonly SignInManager<IdentityUser<int>> signInManager;

    public AccountController(UserManager<IdentityUser<int>> userManager, SignInManager<IdentityUser<int>> signInManager)
	{
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(UserRequest userRequest)
    {

        var user = new IdentityUser<int>
        {
            UserName = userRequest.UserName,
            Email = userRequest.Email
        };

        var userCreated = await userManager.CreateAsync(user, userRequest.Password);

        if (!userCreated.Succeeded)
        {
            return BadRequest("Failed to Register User!");
        }

        return Ok("User Successfully Registered!");
    }

}
