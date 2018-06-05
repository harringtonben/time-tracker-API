namespace time_tracker_API.Controllers
{
    public class SupporterTeamMetric
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int? TotalWorkedFromHome { get; set; }
        public int? TotalCalledOut { get; set; }
        public int? TotalUnplannedOut { get; set; }
        public int? PhoneDays { get; set; }
        public int? EmailDays { get; set; }
        public int? IntegrationsDays { get; set; }
        public int? NonCoverageDays { get; set; }
    }
}