using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.API.Controllers.OneRDF_CONTROLLER;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.SERVICES;
using MediatR;
using RDF.Arcana.API.Features.Authenticate.AuthXApi;

namespace ELIXIRETD.API.Controllers.ONECHARGING_CONTROLLER
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OneChargingController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMediator _mediator;

        public OneChargingController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpPost]
        [ApiKeyAuth]
        [Route("AddOneCharging")]
        public async Task<IActionResult> AddDataOneCharging([FromBody] List<OneChargingDto> data)
        {


            var addFuel = await _unitOfWork.One.AddDataOneCharging(data);

            if(addFuel == false)
            {
                return BadRequest("error");
            }

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully created");
        }

        [HttpGet]
        [Route("GetOneCharging")]
        public async Task<ActionResult<IEnumerable<OneChargingDto>>> OneChargingPagination([FromQuery] UserParams userParams, bool? status, string search)
        {
            var oneChargingList = await _unitOfWork.One.GetOneChargingPagination(userParams, status, search);
            Response.AddPaginationHeader(oneChargingList.CurrentPage, oneChargingList.PageSize, oneChargingList.TotalCount, oneChargingList.TotalPages, oneChargingList.HasNextPage, oneChargingList.HasPreviousPage);

            var oneChargingResult = new
            {
                oneChargingList,
                oneChargingList.CurrentPage,
                oneChargingList.PageSize,
                oneChargingList.TotalCount,
                oneChargingList.TotalPages,
                oneChargingList.HasNextPage,
                oneChargingList.HasPreviousPage

            };

            return Ok(oneChargingResult);
        }


        [HttpPost]
        //[ApiKeyAuth]
        [Route("AddAccountTitle")]
        public async Task<IActionResult> AddAccountTitle([FromBody] List<OneAccountTitleDto> data)
        {


            var addFuel = await _unitOfWork.One.AddAccountTitle(data);

            if (addFuel == false)
            {
                return BadRequest("error");
            }

            await _unitOfWork.CompleteAsync();

            return Ok("Successfully created");
        }

        [HttpGet]
        [Route("GetAccountTitle")]
        public async Task<ActionResult<IEnumerable<OneAccountTitleDto>>> GetAccountTitle([FromQuery] UserParams userParams, bool? status, string search)
        {
            var oneChargingList = await _unitOfWork.One.GetAccountTitle(userParams, status, search);
            Response.AddPaginationHeader(oneChargingList.CurrentPage, oneChargingList.PageSize, oneChargingList.TotalCount, oneChargingList.TotalPages, oneChargingList.HasNextPage, oneChargingList.HasPreviousPage);

            var oneChargingResult = new
            {
                oneChargingList,
                oneChargingList.CurrentPage,
                oneChargingList.PageSize,
                oneChargingList.TotalCount,
                oneChargingList.TotalPages,
                oneChargingList.HasNextPage,
                oneChargingList.HasPreviousPage

            };

            return Ok(oneChargingResult);
        }


        
    }
}
