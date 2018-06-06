namespace time_tracker_API.Controllers
{
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