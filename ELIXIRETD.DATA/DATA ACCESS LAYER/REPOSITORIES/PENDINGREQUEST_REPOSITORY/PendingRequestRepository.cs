using ELIXIRETD.DATA.CORE.INTERFACES.PENDINGREQUEST_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.OneRdf.PendingRequestSetup.CreatePendingRequest.CreatePendingRequestHandler;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.PENDINGREQUEST_REPOSITORY
{
    public class PendingRequestRepository : IPendingRequestRepository
    {
        private readonly StoreContext _context;
        public PendingRequestRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<User> ExistingUserByEmpId(string idPrefix, string idNumber)
        {
            var empId = $"{idPrefix}-{idNumber}";
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == empId);

            return existingUser;
        }

        public async Task<PendingRequest> ExistingPendingUserByEmpId(string idPrefix, string idNumber)
        {
            var existingUser = await _context.PendingRequests.FirstOrDefaultAsync(x => x.IdPrefix == idPrefix && x.IdNo == idNumber);

            return existingUser;
        }

        public async Task<bool> UpdateExistingUser(CreatePendingRequestCommand command)
        {

            var empId = $"{command.Id_Prefix}-{command.Id_No}";
            var existingUserName = await _context.Users.FirstOrDefaultAsync(x => x.EmpId == empId);

            if (existingUserName != null)
            {
                existingUserName.UserName = command.Username;
                existingUserName.Password = command.Password;

                return true;
            }



            return false;
        }

        public async Task<bool> UpdateExistingPendingUser(CreatePendingRequestCommand command)
        {
            var existingUser = await _context.PendingRequests.FirstOrDefaultAsync(x => x.IdPrefix == command.Id_Prefix && x.IdNo == command.Id_No);

            if (existingUser == null)
            {
                return false;
            }

            existingUser.Username = command.Username;
            existingUser.Password = command.Password;
            existingUser.FirstName = command.First_Name;
            existingUser.MiddleName = command.Middle_Name;
            existingUser.LastName = command.Last_Name;
            existingUser.Suffix = command.Suffix;

            return true;
        }

        public async Task<PendingRequest> AddNewPendingAccount(CreatePendingRequestCommand command)
        {
            var addNewPendingAccount = new PendingRequest
            {
                IdPrefix = command.Id_Prefix,
                IdNo = command.Id_No,
                Username = command.Username,
                Password = command.Password,
                FirstName = command.First_Name,
                LastName = command.Last_Name,
                MiddleName = command.Middle_Name,
                Suffix = command.Suffix,
            };

            await _context.PendingRequests.AddAsync(addNewPendingAccount);
            return addNewPendingAccount;
        }


        public async Task<bool> UsernameExist(string username)
        {
            var usernameExist = await _context.Users.AnyAsync(x => x.UserName == username);

            return usernameExist;
        }

        public async Task<bool> UsernameExistInPendingRequest(string idPrefix, string idNo)
        {
            var usernameExist = await _context.PendingRequests.AnyAsync(x => x.IdPrefix == idPrefix && x.IdNo == idNo);

            return usernameExist;
        }
    }
}
