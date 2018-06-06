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


        public List<ReportMetrics> GenerateReport(Reports report, int employeeId, int managerId, int timeframe)
        {
            List<ReportMetrics> metrics;
            
            switch (report)
            {
                case Reports.AllShiftsWithinXWeeks:
                    if (employeeId == 0)
                    {
                        metrics = _repo.AllShiftsAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.AllShiftsPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.WorkfromHomeWithinXWeeks:
                    if (employeeId == 0)
                    {
                        metrics = _repo.AllWorkFromHomeAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.AllWorkFromHomePerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.UnplannedCalloutsWithinXWeeks:
                    if (employeeId == 0)
                    {
                        metrics = _repo.AllCalloutsAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.AllCalloutsPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.AllShiftsByManagerWithinXWeeks:
                    metrics = _repo.AllShiftsByManager(timeframe, managerId);

                    return metrics;
                case Reports.UnplannedVsPlannedSickDaysWithinXWeeks:
                    if (employeeId == 0)
                    {
                        metrics = _repo.PlannedvsUnplannedSickAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.PlannedvsUnplannedSickPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.AllEmailDays:
                    if (employeeId == 0)
                    {
                        metrics = _repo.AllEmailDaysAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.AllEmailDaysPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.AllPhoneDays:
                    if (employeeId == 0)
                    {
                        metrics = _repo.AllPhoneDaysAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.AllPhoneDaysPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.TotalPhoneDays:
                    if (employeeId == 0)
                    {
                        metrics = _repo.TotalPhoneDaysAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.TotalPhoneDaysPerEmployee(timeframe, employeeId);
                    return metrics;
                case Reports.TotalEmailDays:
                    if (employeeId == 0)
                    {
                        metrics = _repo.TotalEmailDaysAllStaff(timeframe);
                        return metrics;
                    }

                    metrics = _repo.TotalEmailDaysPerEmployee(timeframe, employeeId);
                    return metrics;
                default:
                    return null;
            }
        }
    }
}