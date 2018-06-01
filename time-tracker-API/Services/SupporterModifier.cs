using System;
using System.Data.SqlClient;
using System.Net;

namespace time_tracker_API.Services
{
    public class SupporterModifier
    {
        private readonly SupporterRepository _repo;

        public SupporterModifier(SupporterRepository repo)
        {
            _repo = repo;
        }

        public StatusCodes EditEmployee(Supporter supporter)
        {
            bool checkSupporter;
            
            try
            {
                checkSupporter = _repo.GetSupporterById(supporter.EmployeeId);
            }
            catch (SqlException)
            {
                return StatusCodes.Unsuccessful;
            }
            catch (Exception)
            {
                return StatusCodes.NotFound;
            }

            var editSupporter = _repo.EditSupporter(supporter);

            return editSupporter
                ? StatusCodes.Success
                : StatusCodes.Unsuccessful;
        }
    }
}