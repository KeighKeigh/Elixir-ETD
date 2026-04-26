using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using MediatR;
using RDF.Arcana.API.Features.Authenticate.AuthXApi;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.OneRdf.PendingRequestSetup.CreatePendingRequest.CreatePendingRequestHandler;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.CQRS.OneRdf.PendingRequestSetup.GetPendingRequest.GetPendingRequestHandler;

namespace ELIXIRETD.API.Controllers.PENDINGREQUEST_CONTROLLER
{
    [Route("api/PendingRequest")]
    [ApiController]
    public class PendingRequestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PendingRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [ApiKeyAuth]
        [HttpPost]
        public async Task<IActionResult> CreatePendingRequest(CreatePendingRequestCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPendingRequest([FromQuery] GetPendingRequestQuery query)
        {
            try
            {

                var onecharging = await _mediator.Send(query);

                Response.AddPaginationHeader(

                onecharging.CurrentPage,
                onecharging.PageSize,
                onecharging.TotalCount,
                onecharging.TotalPages,
                onecharging.HasPreviousPage,
                onecharging.HasNextPage

                );

                var result = new
                {
                    onecharging,
                    onecharging.CurrentPage,
                    onecharging.PageSize,
                    onecharging.TotalCount,
                    onecharging.TotalPages,
                    onecharging.HasPreviousPage,
                    onecharging.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
