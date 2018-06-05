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
        
        public Shift GetShiftByDate(int id, string date)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst<Shift>(@"SELECT s.*, L.ShiftLengthName FROM Shifts s
                                                              join ShiftLength L on s.ShiftLengthId = L.ShiftLengthId
                                                              WHERE EmployeeId = @id
                                                              AND Date = @date", new {id, date});

                return result;
            }
        }

        public bool GetShiftById(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst("SELECT * FROM Shifts WHERE ShiftId = @id", new {id});

                return result != null;
            }
        }

        public bool EditShift(Shift shift)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute(@"UPDATE Shifts
                                                  SET Date = @Date,
                                                      EmployeeId = @EmployeeId,
                                                      MangerId = @ManagerId,
                                                      WorkFromHome = @WorkFromHome,
                                                      Callout = @Callout, Planned = @Planned,
                                                      ShiftLengthId = @ShiftLengthId,
                                                      Email = @Email,
                                                      Phone = @Phone,
                                                      Integrations = @Integrations,
                                                      NonCoverage = @NonCoverage
                                                  WHERE ShiftId = @ShiftId", shift);

                return result == 1;
            }
        }

        public bool AddShift(Shift newShift)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute(@"INSERT INTO Shifts
                                                      (
                                                        Date,
                                                        EmployeeId,
                                                        ManagerId,
                                                        WorkFromHome,
                                                        Callout,
                                                        Planned,
                                                        ShiftLengthId,
                                                        Email,
                                                        Phone,
                                                        Integrations,
                                                        NonCoverage
                                                      )
                                                    VALUES
                                                      (
                                                        @Date,
                                                        @EmployeeId,
                                                        @ManagerId,
                                                        @WorkFromHome,
                                                        @Callout,
                                                        @Planned,
                                                        @ShiftLengthId,
                                                        @Email,
                                                        @Phone,
                                                        @Integrations,
                                                        @NonCoverage
                                                      )", newShift);

                return result == 1;
            }
        }
    }
}