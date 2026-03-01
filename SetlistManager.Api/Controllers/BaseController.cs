using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Base controller providing authorization, routing, and API controller conventions for all endpoints.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]

public class BaseController : ControllerBase
{
}