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

        [HttpPut("{id}")]
        public IActionResult Edit([FromBody] ManagerDto manager, int id)
        {
            var editedManager = new Manager
            {
                ManagerId = id,
                Name = manager.Name,
                Title = manager.Title
            };
                
            bool checkManager;
            
            try
            {
                checkManager = _repo.GetManagerById(editedManager.ManagerId);
            }
            catch (SqlException)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.NotFound, "Sorry, it does not look like that person exists.");
            }

            var editManager = _repo.EditManager(editedManager);

            return editManager
                ? StatusCode((int) HttpStatusCode.OK, $"{editedManager.Name} has been updated!")
                : StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
        }
    }
}