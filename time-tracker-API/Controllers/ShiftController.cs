using System;
using System.Data.SqlClient;
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
            IndividualShift getShiftByDate;
            
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
        public IActionResult EditShift(int id, [FromQuery] ShiftDto shift)
        {
            return Ok();
        }
    }

    public class ShiftDto
    {
        public int ShiftId { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public bool WorkFromHome { get; set; }
        public bool Callout { get; set; }
        public bool Planned { get; set; }
        public int ShiftLengthId { get; set; }
        public bool email { get; set; }
        public bool phone { get; set; }
        public bool integrations { get; set; }
        public bool NonCoverage { get; set; }
    }
}