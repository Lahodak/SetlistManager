using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.Common.Models;
using System.Reflection.Metadata.Ecma335;

namespace SetlistManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetlistsController : ControllerBase
    {
        private readonly ISetlistsDB _setlistsDB;
        public SetlistsController(ISetlistsDB setlistsDB)
        {
            _setlistsDB = setlistsDB;
        }
        [HttpPost]
        public async Task<int> UploadSetlistToDb(SetlistModel setlistModel)
        {
            return await _setlistsDB.SaveSetlist(setlistModel);
        }
        [HttpGet]
        public async Task<SetlistModel> GetSetlistById(int id)
        {

            return new SetlistModel();
        }
    }
}