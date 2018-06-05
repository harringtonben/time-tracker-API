using System;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        [HttpPost]
        public IActionResult Add([FromBody] ShiftDto shift)
        {
            var newShift = new Shift
            {
                Date = shift.Date,
                EmployeeId = shift.EmployeeId,
                ManagerId = shift.ManagerId,
                WorkFromHome = shift.WorkFromHome,
                Callout = shift.Callout,
                Planned = shift.Planned,
                ShiftLengthId = shift.ShiftLengthId,
                Email = shift.Email,
                Phone = shift.Phone,
                Integrations = shift.Integrations,
                NonCoverage = shift.NonCoverage
            };

            var addShift = _repo.AddShift(newShift);

            return addShift
                ? StatusCode((int) HttpStatusCode.Created, "Shift has been added!")
                : StatusCode((int) HttpStatusCode.InternalServerError, "There does not appear to be a shift associated with that day.");

        }

        [HttpGet("{id}")]
        public IActionResult GetShiftByDate(int id, [FromQuery] string date)
        {
            Shift getShiftByDate;
            
            if (date == null)
                return StatusCode((int) HttpStatusCode.BadRequest, "Please enter a date in order to see the shift");

            try
            {
                getShiftByDate = _repo.GetShiftByDate(id, date);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    $"It does not appear that there is a shift for this employee on {date}");
            }
            
            return StatusCode((int) HttpStatusCode.OK, getShiftByDate);
        }
        
        [HttpPut("{id}")]
        public IActionResult EditShift(int id, [FromBody] ShiftDto shift)
        {
            var shiftToEdit = new Shift
            {
                ShiftId = id,
                Date = shift.Date,
                EmployeeId = shift.EmployeeId,
                ManagerId = shift.ManagerId,
                WorkFromHome = shift.WorkFromHome,
                Callout = shift.Callout,
                Planned = shift.Planned,
                ShiftLengthId = shift.ShiftLengthId,
                Email = shift.Email,
                Phone = shift.Phone,
                Integrations = shift.Integrations,
                NonCoverage = shift.NonCoverage
            };
            
            var editShift = new ShiftModifier(_repo).EditShift(shiftToEdit);

            switch (editShift)
            {
                case StatusCodes.Success:
                    return StatusCode((int) HttpStatusCode.OK, "The shift has been edited!");
                case StatusCodes.NotFound: 
                    return StatusCode((int) HttpStatusCode.NotFound,
                        "There does not appear to be a shift associated with that day.");
                case StatusCodes.Unsuccessful:
                    return StatusCode((int) HttpStatusCode.InternalServerError,
                        "Sorry, something went wrong. Please try again later.");
                default:
                    return StatusCode((int) HttpStatusCode.InternalServerError,
                        "Sorry, something went wrong. Please try again later.");
            }
        }
    }
}