using System;
using System.Collections.Generic;
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

        [HttpGet("{id}")]
        public IActionResult GetMetrics(int id, [FromQuery] int timeframe)
        {
            SupporterMetric supporter;
            try
            {
                supporter = _repo.GetSupportMetrics(id, timeframe);
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");  
            }

            return StatusCode((int) HttpStatusCode.OK, supporter);
        }

        [HttpGet("teammetrics")]
        public IActionResult GetTeamMetrics([FromQuery] int timeframe)
        {
            List<SupporterTeamMetric> supportTeamMetrics;

            try
            {
                supportTeamMetrics = _repo.GetTeamMetrics(timeframe);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, "Sorry, something went wrong. Please try again later.");
            }

            return StatusCode((int) HttpStatusCode.OK, supportTeamMetrics);
        }

        [HttpGet("reporting/{id}")]
        public IActionResult GetReports(int id, [FromQuery] int employeeId, int managerId, int timeframe)
        {
            var report = (Reports) id;
            
            var myReports = new ReportGenerator(_repo).GenerateReport(report, employeeId, managerId, timeframe);

            return Ok();
        }
    }

    public enum Reports
    {
        AllShiftsWithinXWeeks=1,
        WorkfromHomeWithinXWeeks,
        UnplannedCalloutsWithinXWeeks,
        AllShiftsByManagerWithinXWeeks,
        UnplannedVsPlannedSickDaysWithinXWeeks,
        AllEmailDays,
        AllPhoneDays,
        TotalPhoneDays,
        TotalEmailDays
    }
}