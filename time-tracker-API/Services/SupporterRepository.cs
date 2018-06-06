using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using time_tracker_API.Controllers;

namespace time_tracker_API.Services
{
    public class SupporterRepository
    {
        private string _connectionString;

        public SupporterRepository(IOptions<DatabaseOptions> dbOptions)
        {
            _connectionString = dbOptions.Value.ConnectionString;
        }
        
        public List<Supporter> GetAllSupporters()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                
                var result = db.Query<Supporter>("SELECT * from employees").ToList();
                return result;
            }
        }

        public bool AddSupporter(Supporter supporter)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute(@"INSERT INTO Employees
                                                            (
                                                              Name,
                                                              Title,
                                                              ManagerId
                                                            )
                                                        VALUES 
                                                            (
                                                              @Name,
                                                              @Title,
                                                              @ManagerId  
                                                            )", supporter);
                return result == 1;
            }
        }

        public bool EditSupporter(Supporter editedSupporter)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute(@"UPDATE Employees
                                          SET Name = @name, TItle = @Title, ManagerId = @ManagerId
                                          WHERE EmployeeId = @EmployeeId", editedSupporter);

                return result == 1;
            }
        }

        public bool GetSupporterById(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst("SELECT * FROM Employees where employeeId = @id", new { id });

                return result != null;
            }   
        }

        public bool DeleteSupporter(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute("DELETE FROM Employees WHERE employeeId = @id", new {id});

                return result == 1;
            }
        }

        public SupporterMetric GetSupportMetrics(int id, int days)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst<SupporterMetric>(@"WITH WorkFromHomeCTE (EmployeId, TotalWorkedFromHome) AS
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE WorkFromHome = 1
                                                                AND EmployeeId = @id
                                                                AND Date > GETDATE() - @days
                                                                GROUP BY EmployeeId
                                                                ),
                                                                CalloutCTE (EmployeeId, TotalCalledOut) AS
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Callout = 1
                                                                AND EmployeeId = @id
                                                                AND Date > GETDATE() - @days
                                                                GROUP BY EmployeeId
                                                                ),
                                                                UnplannedCalloutCTE (EmployeeId, TotalUnplannedOut) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Callout = 1
                                                                AND Planned = 0
                                                                AND EmployeeId = @id
                                                                AND Date > GETDATE() - @days
                                                                GROUP BY EmployeeId
                                                                )
                                                                
                                                                SELECT DISTINCT e.EmployeeId, e.Name, w.TotalWorkedFromHome, c.TotalCalledOut, u.TotalUnplannedOut from Employees e
                                                                LEFT JOIN  WorkFromHomeCTE w on w.EmployeId = e.EmployeeId
                                                                LEFT JOIN CalloutCTE c on c.EmployeeId = e.EmployeeId
                                                                LEFT JOIN UnplannedCalloutCTE u on u.EmployeeId = e.EmployeeId
                                                                WHERE e.EmployeeId = @id", new {id, days});

                return result;
            }
        }

        public List<SupporterTeamMetric> GetTeamMetrics(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<SupporterTeamMetric>(@"WITH WorkFromHomeCTE (EmployeId, TotalWorkedFromHome) AS
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE WorkFromHome = 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                CalloutCTE (EmployeeId, TotalCalledOut) AS
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Callout = 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                UnplannedCalloutCTE (EmployeeId, TotalUnplannedOut) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Callout = 1
                                                                AND Planned = 0
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                PhoneDays (EmployeeId, PhoneDays) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Phone = 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                EmailDays (EmployeeId, EmailDays) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Email = 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                IntegrationsDays (EmployeeId, IntegrationsDays) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE Integrations = 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                ),
                                                                NonCoverageDays (EmployeeId, NonCoverageDays) as
                                                                (
                                                                SELECT EmployeeId, COUNT(*) FROM Shifts
                                                                WHERE NonCoverage= 1
                                                                AND Date > GETDATE() - @timeframe
                                                                GROUP BY EmployeeId
                                                                )
                                                                
                                                                SELECT DISTINCT e.EmployeeId, e.Name, w.TotalWorkedFromHome, c.TotalCalledOut, u.TotalUnplannedOut, p.PhoneDays, em.EmailDays, i.IntegrationsDays, n.NonCoverageDays from Employees e
                                                                LEFT JOIN  WorkFromHomeCTE w on w.EmployeId = e.EmployeeId
                                                                LEFT JOIN CalloutCTE c on c.EmployeeId = e.EmployeeId
                                                                LEFT JOIN UnplannedCalloutCTE u on u.EmployeeId = e.EmployeeId
                                                                LEFT JOIN PhoneDays p on p.EmployeeId = e.EmployeeId
                                                                LEFT JOIN EmailDays em on em.EmployeeId = e.EmployeeId
                                                                LEFT JOIN IntegrationsDays i on i.EmployeeId = e.EmployeeId
                                                                LEFT JOIN NonCoverageDays n on n.EmployeeId = e.EmployeeId",
                    new {timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllShiftsAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe", new {timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllShiftsPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where s.EmployeeId = @employeeId
                                                        and date > getdate() - @timeframe", new {employeeId, timeframe}).ToList();

                return result;      
            }
        }

        public List<ReportMetrics> AllWorkFromHomeAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and WorkFromHome = 1", new {timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllWorkFromHomePerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where s.EmployeeId = @employeeId
                                                        and date > getdate() - @timeframe
                                                        and WorkFromHome = 1", new {employeeId, timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllCalloutsAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and Callout = 1
                                                        and Planned = 0", new {timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllCalloutsPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where s.EmployeeId = @employeeId
                                                        and date > getdate() - @timeframe
                                                        and Callout = 1
                                                        and Planned = 0", new {employeeId, timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> AllShiftsByManager(int timeframe, int managerId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where s.ManagerId = @managerId
                                                        and date > getdate() - @timeframe", new {managerId, timeframe}).ToList();

                return result;
            }
        }

        public List<ReportMetrics> PlannedvsUnplannedSickAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"with callout_cte (sickdays, employeeid) as
                                                        (
                                                        select count(*) as sickdays, EmployeeId from shifts
                                                        where date > getdate() - @timeframe
                                                        and Callout = 1
                                                        and Planned = 0
                                                        group by EmployeeId
                                                        ),
                                                        allshift_cte (allworkdays, employeeid) as
                                                        (
                                                        select count(*) as allworkdays, EmployeeId from Shifts
                                                        where date > getdate() - @timeframe
                                                        group by EmployeeId
                                                        )
                                                        select distinct (c.sickdays * 100)/a.allworkdays as unplannedsicktime, e.Name from shifts s
                                                        join callout_cte c on c.employeeid = s.EmployeeId
                                                        join allshift_cte a on a.employeeid = s.EmployeeId
                                                        join Employees e on e.employeeId = s.employeeId
                                                        ", new {timeframe}).ToList();

                return result;   
            }
        }

        public List<ReportMetrics> PlannedvsUnplannedSickPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"with callout_cte (sickdays, employeeid) as
                                                        (
                                                        select count(*) as sickdays, EmployeeId from shifts
                                                        where EmployeeId = @employeeId
                                                        and date > getdate() - @timeframe
                                                        and Callout = 1
                                                        and Planned = 0
                                                        group by EmployeeId
                                                        ),
                                                        allshift_cte (allworkdays, employeeid) as
                                                        (
                                                        select count(*) as allworkdays, EmployeeId from Shifts
                                                        where EmployeeId = @employeeId
                                                        and date > getdate() - @timeframe
                                                        group by EmployeeId
                                                        )
                                                        select distinct (c.sickdays * 100)/a.allworkdays as unplannedsicktime, e.Name from shifts s
                                                        join callout_cte c on c.employeeid = s.EmployeeId
                                                        join allshift_cte a on a.employeeid = s.EmployeeId
                                                        join Employees e on e.employeeId = s.employeeId
                                                        ", new {employeeId, timeframe}).ToList();

                return result;   
            }
        }

        public List<ReportMetrics> AllEmailDaysAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and Email = 1", new {timeframe}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> AllEmailDaysPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and s.EmployeeId = @employeeId
                                                        and Email = 1", new {timeframe, employeeId}).ToList();

                return result;   
            }
        }

        public List<ReportMetrics> AllPhoneDaysAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and Phone = 1", new {timeframe}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> AllPhoneDaysPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select s.*, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and s.EmployeeId = @employeeId
                                                        and Phone = 1", new {timeframe, employeeId}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> TotalPhoneDaysAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select count(*) as totalphonedays, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and Phone = 1
                                                        group by e.Name", new {timeframe}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> TotalPhoneDaysPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select count(*) as totalphonedays, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and s.EmployeeId = @employeeId
                                                        and Phone = 1
                                                        group by e.Name", new {timeframe, employeeId}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> TotalEmailDaysAllStaff(int timeframe)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select count(*) as totalemaildays, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and Email = 1
                                                        group by e.Name", new {timeframe}).ToList();

                return result;    
            }
        }

        public List<ReportMetrics> TotalEmailDaysPerEmployee(int timeframe, int employeeId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<ReportMetrics>(@"select count(*) as totalemaildays, e.name from Shifts s
                                                        join Employees e on e.employeeid = s.employeeid
                                                        where date > getdate() - @timeframe
                                                        and s.EmployeeId = @employeeId
                                                        and Email = 1
                                                        group by e.Name", new {timeframe, employeeId }).ToList();

                return result;     
            }
        }
    }
}

