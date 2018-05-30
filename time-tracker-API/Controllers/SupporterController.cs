using System.Net;
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
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var allSupporters = _repo.GetAllSupporters();

            return Ok(allSupporters);
        }

        [HttpPost]
        public IActionResult Add(SupporterDto supporter)
        {
            var newSupporter = new Supporter
            {
                Name = supporter.Name,
                Title = supporter.Title,
                ManagerId = supporter.ManagerId
            };
            
            var addSupporter = _repo.AddSupporter(newSupporter);

            return addSupporter
                ? StatusCode((int) HttpStatusCode.Created, "Your new supporter has been added!")
                : StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
        }
    }
}