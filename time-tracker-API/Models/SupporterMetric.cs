namespace time_tracker_API.Services
{
    public class SupporterMetric
    {
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public string Name { get; set; }
        public int? TotalWorkedFromHome { get; set; }
        public int? TotalCalledOut { get; set; }
        public int? TotalUnplannedOut { get; set; }
    }
}