using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;

namespace time_tracker_API.Services
{
    public class ManagerRepository
    {
        private string _connectionString;

        public ManagerRepository(IOptions<DatabaseOptions> dbOptions)
        {
            _connectionString = dbOptions.Value.ConnectionString;
        }
        
        public List<Manager> GetAllManagers()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Query<Manager>("SELECT * FROM Managers").ToList();

                return result;
            }
        }

        public bool AddNewManager(Manager newManager)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();
                
                var result = db.Execute(@"INSERT INTO Managers
                                                            (
                                                              Name,
                                                              Title
                                                            )
                                                      VALUES
                                                            (
                                                              @Name,
                                                              @Title
                                                            )", newManager);

                return result == 1;
            }
        }

        public bool GetManagerById(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.QueryFirst("SELECT * FROM Managers WHERE ManagerId = @id", new {id});

                return result != null;
            }
        }

        public bool EditManager(Manager editedManager)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute(@"UPDATE Managers
                                          SET Name = @Name, Title = @Title
                                          WHERE ManagerId = @ManagerId", editedManager);

                return result == 1;
            }
        }

        public bool DeleteManager(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                var result = db.Execute("DELETE FROM Managers where ManagerId = @id", new {id});

                return result == 1;
            }
        }
    }
}