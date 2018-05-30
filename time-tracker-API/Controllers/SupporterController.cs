using Microsoft.AspNetCore.Mvc;
using time_tracker_API.Services;

namespace time_tracker_API.Controllers
{
    [Route("api/supporters")]
    public class SupporterController : Controller
    {
        private readonly SupporterRepository _repo;

        public SupporterController(SupporterRepository repo)
        {
            _repo = repo;
        }
        
        public IActionResult GetAll()
        {
            var allSupporters = _repo.GetAllSupporters();

            return Ok(allSupporters);
        }
    }
}