using System;

namespace time_tracker_API.Services
{
    public class ReportMetrics
    {
        public int ShiftId { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public bool WorkFromHome { get; set; }
        public bool Callout { get; set; }
        public bool Planned { get; set; }
        public bool Phone { get; set; }
        public bool Email { get; set; }
        public bool Integrations { get; set; }
        public bool NonCoverage { get; set; }
        public int? UnplannedSickTime { get; set; }
        public int? TotalPhoneDays { get; set; }
        public int? TotalEmailDays { get; set; }
    }
}