using System;
using System.Data.SqlClient;
using System.Net;

namespace time_tracker_API.Services
{
    public class ShiftModifier
    {
        
        private readonly ShiftRepository _repo;

        public ShiftModifier(ShiftRepository repo)
        {
            _repo = repo;
        }
        
        public StatusCodes EditShift(Shift shift)
        {
            bool getShift;

            try
            {
                getShift = _repo.GetShiftById(shift.ShiftId);
            }
            catch (SqlException)
            {
                return StatusCodes.Unsuccessful;
            }
            catch (Exception)
            {
                return StatusCodes.NotFound;
            }

            var updateShift = _repo.EditShift(shift);

            return updateShift
                ? StatusCodes.Success
                : StatusCodes.Unsuccessful;

        }
    }
}