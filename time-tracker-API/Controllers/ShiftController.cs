using System.Net;
using Microsoft.AspNetCore.Mvc;
using time_tracker_API.Services;

namespace time_tracker_API.Controllers
{   
    [Route("api/shifts")]
    public class ShiftController : Controller
    {
        private readonly ShiftRepository _repo;

        public ShiftController(ShiftRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public IActionResult GetShiftByDate(int id, [FromQuery] string date)
        {
            var getShiftByDate = _repo.GetShiftByDate(id, date);

            return StatusCode((int) HttpStatusCode.OK, getShiftByDate);
        }
    }
}