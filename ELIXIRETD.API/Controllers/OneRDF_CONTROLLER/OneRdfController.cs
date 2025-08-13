using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using MediatR;
using RDF.Arcana.API.Features.Authenticate.AuthXApi;
using static ELIXIRETD.API.Controllers.OneRDF_CONTROLLER.OneRdfSync;
using static ELIXIRETD.API.Controllers.OneRDF_CONTROLLER.ViewOneRdf.OneRdfView;

namespace ELIXIRETD.API.Controllers.OneRDF_CONTROLLER
{

    public class OneRdfSyncBatchCommand : IRequest<Result>
    {
        public List<OneRdfSyncCommand> OneCharging { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OneRdfController : ControllerBase
    {

        private readonly IMediator _mediator;
        public OneRdfController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ApiKeyAuth]
        [Route("OneRdfSync")]
        public async Task<IActionResult> OneRdfSync([FromBody] OneRdfSyncBatchCommand command)
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

        [HttpGet("page")]
        public async Task<IActionResult> ViewOneCharging([FromQuery] OneRdfViewQuery query)
        {
            try
            {
                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(

                   users.CurrentPage,
                   users.PageSize,
                   users.TotalCount,
                   users.TotalPages,
                   users.HasNextPage,
                   users.HasPreviousPage

                    );

                var results = new
                {
                    users,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasNextPage,
                    users.HasPreviousPage
                };

                var successResult = Result.Success(results);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
