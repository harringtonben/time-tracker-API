using System;
using System.Data.SqlClient;
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

        [HttpPost]
        public IActionResult Add([FromBody] ManagerDto manager)
        {
            var newManager = new Manager
            {
                Name = manager.Name,
                Title = manager.Title
            };

            var addNewManager = _repo.AddNewManager(newManager);

            return addNewManager
                ? StatusCode((int) HttpStatusCode.Created, $"{newManager.Name} has been added as a manager!")
                : StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
        }

        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            Manager manager;
            
            try
            {
                manager = _repo.GetManager(id);
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
            }

            return StatusCode((int) HttpStatusCode.OK, manager);
        }

        [HttpPut("{id}")]
        public IActionResult Edit([FromBody] ManagerDto manager, int id)
        {
            var editedManager = new Manager
            {
                ManagerId = id,
                Name = manager.Name,
                Title = manager.Title
            };
            
            var editManager = new ManagerModifier(_repo).EditManager(editedManager);

            switch (editManager)
            {
                case StatusCodes.Success:
                    return StatusCode((int) HttpStatusCode.OK, $"{editedManager.Name} has been updated!");
                case StatusCodes.NotFound:
                    return StatusCode((int) HttpStatusCode.NotFound, "Sorry, it does not look like that person exists.");
                case StatusCodes.Unsuccessful:
                    return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
                default:
                    return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
            }        
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleteManager = new ManagerModifier(_repo).DeleteManager(id);

            switch (deleteManager)
            {
                case StatusCodes.Success:
                    return StatusCode((int) HttpStatusCode.OK, "The manager has been deleted.");
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