using System;

namespace time_tracker_API.Controllers
{
    public class ShiftDto
    {
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public bool WorkFromHome { get; set; }
        public bool Callout { get; set; }
        public bool Planned { get; set; }
        public int ShiftLengthId { get; set; }
        public bool Email { get; set; }
        public bool Phone { get; set; }
        public bool Integrations { get; set; }
        public bool NonCoverage { get; set; }
    }
}