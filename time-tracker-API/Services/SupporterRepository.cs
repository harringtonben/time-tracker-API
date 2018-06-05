﻿using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllShiftsPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllWorkFromHomeAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllWorkFromHomePerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllCalloutsAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllCalloutsPerEmployee(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllShiftsByManager(int timeframe, int managerId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> PlannedvsUnplannedSickAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> PlannedvsUnplannedSickPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllEmailDaysAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllEmailDaysPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllPhoneDaysAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> AllPhoneDaysPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> TotalPhoneDaysAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> TotalPhoneDaysPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> TotalEmailDaysAllStaff(int timeframe)
        {
            throw new System.NotImplementedException();
        }

        public List<ReportMetrics> TotalEmailDaysPerEmployee(int timeframe, int employeeId)
        {
            throw new System.NotImplementedException();
        }
    }
}

