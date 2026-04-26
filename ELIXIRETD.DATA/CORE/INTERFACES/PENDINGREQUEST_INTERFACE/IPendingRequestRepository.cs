using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.OneRdf.PendingRequestSetup.CreatePendingRequest.CreatePendingRequestHandler;

namespace ELIXIRETD.DATA.CORE.INTERFACES.PENDINGREQUEST_INTERFACE
{
    public interface IPendingRequestRepository
    {
        Task<User> ExistingUserByEmpId(string idPrefix, string idNumber);
        Task<PendingRequest> ExistingPendingUserByEmpId(string idPrefix, string idNumber);

        Task<bool> UpdateExistingUser(CreatePendingRequestCommand command);
        Task<bool> UpdateExistingPendingUser(CreatePendingRequestCommand command);
        Task<PendingRequest> AddNewPendingAccount(CreatePendingRequestCommand command);
        Task<bool> UsernameExist(string username);

        Task<bool> UsernameExistInPendingRequest(string idPrefix, string idNo);
    }
}
