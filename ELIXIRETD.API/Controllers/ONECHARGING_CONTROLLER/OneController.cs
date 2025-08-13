using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.DATA.CORE.API_RESPONSE;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RDF.Arcana.API.Features.Authenticate.AuthXApi;
using System;
using static System.Net.WebRequestMethods;

namespace ELIXIRETD.API.Controllers.ONECHARGING_CONTROLLER
{
    [Route("api/charging")] 
    [ApiController]
    [AllowAnonymous]
    public class OneController : ControllerBase
    {

        public readonly IMediator _mediator;
        private readonly IConfiguration _config;

        public OneController(IMediator mediator, IConfiguration config)
        {
            _mediator = mediator;
            _config = config;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {

            var apiKeyHeader = Request.Headers["Api-Key"].ToString();
            var expectedApiKey = _config["Authentication:ApiKey"];



            if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != expectedApiKey)
            {
                return Unauthorized("Invalid or missing API key.");
            }

            var result = await _mediator.Send(new ImportChargingCommand());
            return result.IsFailure ? BadRequest(result) : Ok(result);

        }


        public class ImportChargingCommand : IRequest<Result> { }

        public class Handler : IRequestHandler<ImportChargingCommand, Result>
        {
            public readonly StoreContext _storeContext;
            public readonly IHttpClientFactory _httpClientFactory;

            public Handler(StoreContext storeContext, IHttpClientFactory httpClientFactory)
            {
                _storeContext = storeContext;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Result> Handle(ImportChargingCommand request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient();
                var apiEndPoint = "https://api-one.rdfmis.com/api/charging_api?per_page=10&page=1&pagination=none";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndPoint);
                httpRequest.Headers.Add("API_KEY", "hello world!");

                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return Result.Failure(new Error("Failed to retrieve data from the source API", "NOT_FOUND"));
                }
                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
                if (response == null || response.Data == null)
                {
                    return Result.Failure(new Error("Failed to parse response from the source API.", "INVALID_RESPONSE"));
                }
                
                foreach (var item in response.Data)
                {
                    var existing = await _storeContext.OneRdfs
                        .FirstOrDefaultAsync(x => x.sync_id == item.Id, cancellationToken);

                    if (existing == null)
                    {
                        var charging = new OneRdf
                        {
                            sync_id = item.Id,
                            code = item.code,
                            name = item.name,
                            company_code = item.company_code,
                            company_name = item.company_name,
                            business_unit_code = item.business_unit_code,
                            business_unit_name = item.business_unit_name,
                            department_code = item.department_code,
                            department_name = item.department_name,
                            department_unit_code = item.unit_code,
                            department_unit_name = item.unit_name,
                            sub_unit_code = item.sub_unit_code,
                            sub_unit_name = item.sub_unit_name,
                            location_code = item.location_code,
                            location_name = item.location_name,
                            deleted_at = item.deleted_at,
                            IsActive = item.deleted_at != null ? false : true,
                            DateAdded = item.created_at,
                            UpdatedAt  = item.updated_at,
                        };

                        await _storeContext.OneRdfs.AddAsync(charging, cancellationToken);
                    }
                    else if (existing.UpdatedAt != item.updated_at)
                    {
                        existing.code = item.code;
                        existing.name = item.name;
                        existing.company_code = item.company_code;
                        existing.company_name = item.company_name;
                        existing.business_unit_code = item.business_unit_code;
                        existing.business_unit_name = item.business_unit_name;
                        existing.department_code = item.department_code;
                        existing.department_name = item.department_name;
                        existing.department_unit_code = item.unit_code;
                        existing.department_unit_name = item.unit_name;
                        existing.sub_unit_code = item.sub_unit_code;
                        existing.sub_unit_name = item.sub_unit_name;
                        existing.location_code = item.location_code;
                        existing.location_name = item.location_name;
                        existing.deleted_at = item.deleted_at;
                        existing.IsActive = item.deleted_at != null ? false : true;
                        existing.UpdatedAt = item.updated_at;
                    }


                }
                await _storeContext.SaveChangesAsync(cancellationToken);
                return Result.Success("Charging synced successfully.");
            }

        }

        public class ApiResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<ChargingDto>? Data { get; set; }
        }

        public class ChargingDto
        {
            public int Id { get; set; }
            public string code { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string company_code { get; set; } = string.Empty;
            public string company_name { get; set; } = string.Empty;
            public string business_unit_code { get; set; } = string.Empty;
            public string business_unit_name { get; set; } = string.Empty;
            public string department_code { get; set; } = string.Empty;
            public string department_name { get; set; } = string.Empty;
            public string unit_code { get; set; } = string.Empty;
            public string unit_name { get; set; } = string.Empty;
            public string sub_unit_code { get; set; } = string.Empty;
            public string sub_unit_name { get; set; } = string.Empty;
            public string location_code { get; set; } = string.Empty;
            public string location_name { get; set; } = string.Empty;
            public DateTime? created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? deleted_at { get; set; }
        }
    }
}
