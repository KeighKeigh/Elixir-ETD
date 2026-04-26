using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.OneRdf.PendingRequestSetup.GetPendingRequest
{
    public class GetPendingRequestHandler
    {
        public class GetPendingRequestResult
        {
            public int Id { get; set; }
            public string Id_Prefix { get; set; }
            public string Id_No { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Middle_Name { get; set; }
            public string Suffix { get; set; }
        }


        public class GetPendingRequestQuery : UserParams, IRequest<PagedList<GetPendingRequestResult>>
        {
            public string Search { get; set; }
        }


        public class Handler : IRequestHandler<GetPendingRequestQuery, PagedList<GetPendingRequestResult>>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetPendingRequestResult>> Handle(GetPendingRequestQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PendingRequest> pendingRequests = _context.PendingRequests;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    pendingRequests = pendingRequests.Where(x => x.IdPrefix.ToLower().Contains(request.Search.ToLower()));
                }

                var result = pendingRequests.Select(x => new GetPendingRequestResult
                {
                    Id = x.Id,
                    Id_Prefix = x.IdPrefix,
                    Id_No = x.IdNo,
                    Username = x.Username,
                    Password = x.Password,
                    First_Name = x.FirstName,
                    Last_Name = x.LastName,
                    Middle_Name = x.MiddleName,
                    Suffix = x.Suffix,
                });

                return await PagedList<GetPendingRequestResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
