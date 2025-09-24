using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

[Route("api/instruments")]
public class InstrumentsController : BaseController
{
    private readonly IInstrumentsService _instrumentsService;
    public InstrumentsController(IInstrumentsService instrumentsService)
    {
        _instrumentsService = instrumentsService;
    }

    [HttpGet]
    public async Task<ActionResult<InstrumentModel>> GetAvailableInstruments()
    {
        List<InstrumentModel> instruments = await _instrumentsService.GetAvailableInstrumentsAsync();

        return Ok(instruments);
    }
}