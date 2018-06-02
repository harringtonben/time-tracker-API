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
    }
}