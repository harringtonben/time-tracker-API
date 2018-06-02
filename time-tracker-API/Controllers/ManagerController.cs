using System.Net;
using Microsoft.AspNetCore.Mvc;
using time_tracker_API.Services;

namespace time_tracker_API.Controllers
{
    [Route("api/managers")]
    public class ManagerController : Controller
    {
        private readonly ManagerRepository _repo;

        public ManagerController(ManagerRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var getAllManagers = _repo.GetAllManagers();

            return StatusCode((int) HttpStatusCode.OK, getAllManagers);
        }
    }
}