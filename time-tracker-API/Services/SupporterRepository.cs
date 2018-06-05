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
    }

    public class SupporterMetric
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int? TotalWorkedFromHome { get; set; }
        public int? TotalCalledOut { get; set; }
        public int? TotalUnplannedOut { get; set; }
    }
}

