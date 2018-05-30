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
                
                var result = db.Query<Supporter>("select * from employees").ToList();
                return result;
            }
        }

    }
}