using System;
using System.Data.SqlClient;
using System.Net;

namespace time_tracker_API.Services
{
    public class ManagerModifier
    {
        private readonly ManagerRepository _repo;

        public ManagerModifier(ManagerRepository repo)
        {
            _repo = repo;
        }
        
        public StatusCodes EditManager(Manager manager)
        {
            bool checkManager;
            
            try
            {
                checkManager = _repo.GetManagerById(manager.ManagerId);
            }
            catch (SqlException)
            {
                return StatusCodes.Unsuccessful;
            }
            catch (Exception)
            {
                return StatusCodes.NotFound;
            }

            var editManager = _repo.EditManager(manager);

            return editManager
                ? StatusCodes.Success
                : StatusCodes.Unsuccessful;
        }
    }
}