//using MediatR;
//using System.Security.Claims;

//namespace ELIXIRETD.API.Controllers.ETDGL_CONTROLLER
//{
//    public class GeneralLedgerExportETD
//    {
//        [Route("api/check-ins-reports"), ApiController]
//        public class CheckInReports : ControllerBase
//        {
//            private readonly IMediator _mediator;
//            public CheckInReports(IMediator mediator)
//            {
//                _mediator = mediator;
//            }

//            [HttpGet]
//            public async Task<IActionResult> CheckInsResult([FromQuery] CheckInReportsQuery query)
//            {
//                try
//                {
//                    if (User.Identity is ClaimsIdentity identity
//                   && IdentityHelper.TryGetUserId(identity, out var userId))
//                    {
//                        query.AddedBy = userId;

//                        var roleClaim = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role);

//                    }

//                    var result = await _mediator.Send(query);
//                    return result;
//                }
//                catch (Exception ex)
//                {
//                    return BadRequest(ex.Message);
//                }
//            }
//            public class CheckInReportsQuery : IRequest<IActionResult>
//            {
//                public int? AddedBy { get; set; }
//                public DateTime DateFrom { get; set; }
//                public DateTime DateTo { get; set; }
//                public int? ClusterId { get; set; }
//            }
//        }
//}
