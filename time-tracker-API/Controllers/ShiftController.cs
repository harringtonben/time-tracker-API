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

       
    }
}