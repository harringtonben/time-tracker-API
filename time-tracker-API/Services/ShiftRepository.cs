using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;

namespace time_tracker_API.Services
{       
    public class ShiftRepository
    {
        private string _connectionString;

        public ShiftRepository(IOptions<DatabaseOptions> dbOptions)
        {
            _connectionString = dbOptions.Value.ConnectionString;
        }
        
        public IndividualShift GetShiftByDate(int id, string date)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst<IndividualShift>(@"SELECT s.*, L.ShiftLengthName FROM Shifts s
                                                              join ShiftLength L on s.ShiftLengthId = L.ShiftLengthId
                                                              WHERE EmployeeId = @id
                                                              AND Date = @date", new {id, date});

                return result;
            }
        }
    }

    public class IndividualShift
    {
        public int ShiftId { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public bool WorkFromHome { get; set; }
        public bool Callout { get; set; }
        public bool Planned { get; set; }
        public bool ShiftLengthId { get; set; }
        public bool Email { get; set; }
        public bool Phone { get; set; }
        public bool Integrations { get; set; }
        public bool NonCoverage { get; set; }
        public string ShiftLengthName { get; set; }
    }
}