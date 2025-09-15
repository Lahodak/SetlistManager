using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManger.Business.Services;  
namespace SetlistManager.Api.Controllers;

[Route("api/languages")]

public class LanguagesController : BaseController
{
    private readonly ILanguageService _languageService;

    public LanguagesController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LanguageModel>>> GetAvailableLanguages() 
        => Ok(await _languageService.GetAvailableLanguagesAsync());
}