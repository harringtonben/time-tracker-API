using System.Collections.Generic;
using time_tracker_API.Controllers;

namespace time_tracker_API.Services
{
    public class ReportGenerator
    {
        private readonly SupporterRepository _repo;

        public ReportGenerator(SupporterRepository repo)
        {
            _repo = repo;
        }


        public object GenerateReport(Reports report, int employeeId, int managerId, int timeframe)
        {
            List<ReportMetrics> metrics;
            
            switch (report)
            {
                case Reports.AllShiftsWithinXWeeks:
                    if (employeeId == 0)
                        metrics = _repo.AllShiftsAllStaff(timeframe);
                    
                    metrics = _repo.AllShiftsPerEmployee(timeframe, employeeId);
                    
                    return metrics;
                case Reports.WorkfromHomeWithinXWeeks:
                    if (employeeId == 0)
                        metrics = _repo.AllWorkFromHomeAllStaff(timeframe);

                    metrics = _repo.AllWorkFromHomePerEmployee(timeframe, employeeId);
                    
                    return metrics;
                case Reports.UnplannedCalloutsWithinXWeeks:
                    if (employeeId == 0)
                        metrics = _repo.AllCalloutsAllStaff(timeframe);
                    
                    metrics = _repo.AllCalloutsPerEmployee(timeframe, employeeId);

                    return metrics;
                case Reports.AllShiftsByManagerWithinXWeeks:
                    metrics = _repo.AllShiftsByManager(timeframe, managerId);

                    return metrics;
                case Reports.UnplannedVsPlannedSickDaysWithinXWeeks:
                    if (employeeId == 0)
                        metrics = _repo.PlannedvsUnplannedSickAllStaff(timeframe);
                    
                    metrics = _repo.PlannedvsUnplannedSickPerEmployee(timeframe, employeeId);

                    return metrics;
                case Reports.AllEmailDays:
                    if (employeeId == 0)
                        metrics = _repo.AllEmailDaysAllStaff(timeframe);
                    
                    metrics = _repo.AllEmailDaysPerEmployee(timeframe, employeeId);

                    return metrics;
                case Reports.AllPhoneDays:
                    if (employeeId == 0)
                        metrics = _repo.AllPhoneDaysAllStaff(timeframe);

                    metrics = _repo.AllPhoneDaysPerEmployee(timeframe, employeeId);

                    return metrics;
                case Reports.TotalPhoneDays:
                    if (employeeId == 0)
                        metrics = _repo.TotalPhoneDaysAllStaff(timeframe);

                    metrics = _repo.TotalPhoneDaysPerEmployee(timeframe, employeeId);

                    return metrics;
                case Reports.TotalEmailDays:
                    if (employeeId == 0)
                        metrics = _repo.TotalEmailDaysAllStaff(timeframe);

                    metrics = _repo.TotalEmailDaysPerEmployee(timeframe, employeeId);

                    return metrics;
                default:
                    return null;
            }
        }
    }

    public class ReportMetrics
    {
        public int ShiftId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int? TotalWorkedFromHome { get; set; }
        public int? TotalCalledOut { get; set; }
        public int? TotalUnplannedOut { get; set; }
        public int? PhoneDays { get; set; }
        public int? EmailDays { get; set; }
        public int? IntegrationsDays { get; set; }
        public int? NonCoverageDays { get; set; }
        public int? UnplannedSickTime { get; set; }
        public int? TotalPhoneDays { get; set; }
        public int? TotalEmailDays { get; set; }
    }
}