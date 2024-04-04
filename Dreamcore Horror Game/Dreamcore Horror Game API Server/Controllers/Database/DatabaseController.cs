using Microsoft.AspNetCore.Mvc;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DatabaseController : ControllerBase
    {
        protected readonly DreamcoreHorrorGameContext _context;

        protected const string ENTITY_SET_IS_NULL = "Requested entity set is null.";
        protected const string ID_DOES_NOT_MATCH = "Received parameter 'id' does not match the 'id' value of the object.";
        protected const string INVALID_ENTITY_DATA = "Invalid entity data.";

        public DatabaseController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // POST note:
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    }
}
