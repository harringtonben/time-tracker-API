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
    }
    
    public class Manager
    {
        public int ManagerId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }  
    }
}