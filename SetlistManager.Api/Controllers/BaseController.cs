using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SetlistManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]

public class BaseController : ControllerBase
{
}