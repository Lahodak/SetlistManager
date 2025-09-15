using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SetlistManager. Api.Controllers;

[ApiController]
[Authorize]

public class BaseController : ControllerBase
{
}