using ELIXIRETD.API.Controllers.OneRDF_CONTROLLER;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.SERVICES;

namespace ELIXIRETD.API.Controllers.ONECHARGING_CONTROLLER
{

    [Route("api/[controller]")]
    [ApiController]
    public class OneChargingController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;

        public OneChargingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
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

    }
}
