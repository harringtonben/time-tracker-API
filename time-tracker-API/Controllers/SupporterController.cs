using System;
using System.Data.SqlClient;
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
        public IActionResult Add([FromBody]SupporterDto supporter)
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

        [HttpPut("{id}")]
        public IActionResult Edit([FromBody] SupporterDto supporter, int id)
        {
            var editedSupporter = new Supporter
            {
                EmployeeId = id,
                Name = supporter.Name,
                Title = supporter.Title,
                ManagerId = supporter.ManagerId
            };

            try
            {
                var checkSupporter = _repo.GetSupporterById(id);
            }
            catch (SqlException)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.NotFound, "Sorry, it does not look like that person exists.");
            }

            var editSupporter = _repo.EditSupporter(editedSupporter);

            return Ok($"{editedSupporter.Name} has been updated!");
        }
    }
}