using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
}
