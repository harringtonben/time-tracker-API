using System;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            var editSupporter = new SupporterModifier(_repo).EditEmployee(editedSupporter);

            switch (editSupporter)
            {
               case StatusCodes.Success:
                   return StatusCode((int) HttpStatusCode.OK, $"{editedSupporter.Name} has been updated!");
               case StatusCodes.NotFound:
                   return StatusCode((int) HttpStatusCode.NotFound, "Sorry, it does not look like that person exists.");
               case StatusCodes.Unsuccessful:
                   return StatusCode((int) HttpStatusCode.InternalServerError,
                       "Sorry, something went wrong. Please try again later.");
               default: 
                   return StatusCode((int) HttpStatusCode.InternalServerError,
                       "Sorry, something went wrong. Please try again later.");
            }            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleteSupporter = new SupporterModifier(_repo).DeleteEmployee(id);

            switch (deleteSupporter)
            {
                case StatusCodes.Success:
                    return StatusCode((int) HttpStatusCode.OK, "The employee has been deleted.");
                case StatusCodes.NotFound:
                    return StatusCode((int) HttpStatusCode.NotFound, "Sorry, it does not look like that person exists.");
                case StatusCodes.Unsuccessful:
                    return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");  
                default:
                    return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
            }
        }
    }
}