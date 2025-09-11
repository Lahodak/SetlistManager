using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SetlistManager.API.Controllers;

[ApiController]
[Authorize]

public class BaseController : ControllerBase
{
}