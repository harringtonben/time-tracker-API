﻿using System;
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
        public IActionResult EditShift(int id, [FromQuery] ShiftDto shift)
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

            bool getShift;

            try
            {
                getShift = _repo.GetShiftById(shiftToEdit.ShiftId);
            }
            catch (SqlException)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.NotFound,
                    "There does not appear to be a shift associated with that day.");
            }

            var updateShift = _repo.EditShift(shiftToEdit);

            return updateShift
                ? StatusCode((int) HttpStatusCode.OK, "The shift has been edited!")
                : StatusCode((int) HttpStatusCode.InternalServerError,
                    "Sorry, something went wrong. Please try again later.");
        }
    }
}