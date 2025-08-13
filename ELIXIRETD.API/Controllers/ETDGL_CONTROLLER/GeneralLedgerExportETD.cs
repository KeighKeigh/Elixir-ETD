//using DocumentFormat.OpenXml.EMMA;
//using ELIXIR.API.Common;
//using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
//using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
//using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading;
//using System.Threading.Tasks;

//[AllowAnonymous]
//[Route("api/charging"), ApiController]
//public class OneController : ControllerBase
//{

//    public readonly IMediator _mediator;
//    public readonly IConfiguration _config;

//    public OneController(IMediator mediator, IConfiguration iconfig)
//    {
//        _mediator = mediator;
//        _config = iconfig;
//    }


//    [HttpPost("sync")]

//    public async Task<IActionResult> Sync()
//    {

//        //var apiKeyHeader = Request.Headers["Api-Key"].ToString();
//        //var expectedApiKey = _config["Authentication:ApiKey"];


//        //if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != expectedApiKey)
//        //{
//        //    return Unauthorized("Invalid or missing API key.");
//        //}


//        var result = await _mediator.Send(new ImportChargingCommand());
//        return result.IsFailure ? BadRequest(result) : Ok(result);

//    }


//    public class ImportChargingCommand : IRequest<Result> { }

//    public class Handler : IRequestHandler<ImportChargingCommand, Result>
//    {
//        public readonly StoreContext _storeContext;
//        public readonly IHttpClientFactory _httpClientFactory;

//        public Handler(StoreContext storeContext, IHttpClientFactory httpClientFactory)
//        {
//            _storeContext = storeContext;
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<Result> Handle(ImportChargingCommand request, CancellationToken cancellationToken)
//        {


//            var client = _httpClientFactory.CreateClient();
//            var apiEndPoint = "https://api-one.rdfmis.com/api/charging_api?per_page=10&page=1&pagination=none";

//            var httpRequest = new HttpRequestMessage(HttpMethod.Get, apiEndPoint);

//            httpRequest.Headers.Add("API_KEY", "hello world!");

//            var httpResponse = await client.SendAsync(httpRequest, cancellationToken);

//            if (!httpResponse.IsSuccessStatusCode)
//            {
//                return Result.Failure(new Error("Failed to retrieve data from the source API", "NOT_FOUND"));
//            }

//            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

//            if (response == null || response.Data == null)
//            {
//                return Result.Failure(new Error("Failed to parse response from the source API.", "INVALID_RESPONSE"));
//            }

//            foreach (var item in response.Data)
//            {
//                var existing = await _storeContext.OneChargings
//                    .FirstOrDefaultAsync(x => x.SyncId == item.Id, cancellationToken);

//                if (existing == null)
//                {
//                    var charging = new OneCharging
//                    {
//                        SyncId = item.Id,
//                        Code = item.code,
//                        Name = item.name,
//                        CompanyCode = item.company_code,
//                        CompanyName = item.company_name,
//                        BusinessUnitCode = item.business_unit_code,
//                        BusinessUnitName = item.business_unit_name,
//                        DepartmentCode = item.department_code,
//                        DepartmentName = item.department_name,
//                        DepartmentUnitCode = item.unit_code,
//                        DepartmentUnitName = item.unit_name,
//                        SubUnitCode = item.sub_unit_code,
//                        SubUnitName = item.sub_unit_name,
//                        LocationCode = item.location_code,
//                        LocationName = item.location_name,
//                        DeletedAt = item.deleted_at,
//                        IsActive = item.deleted_at != null ? false : true,
//                        CreatedAt = item.created_at,
//                        UpdatedAt = item.updated_at,
//                    };

//                    await _storeContext.OneChargings.AddAsync(charging, cancellationToken);
//                }
//                else if (existing.UpdatedAt != item.updated_at)
//                {
//                    existing.Code = item.code;
//                    existing.Name = item.name;
//                    existing.CompanyCode = item.company_code;
//                    existing.CompanyName = item.company_name;
//                    existing.BusinessUnitCode = item.business_unit_code;
//                    existing.BusinessUnitName = item.business_unit_name;
//                    existing.DepartmentCode = item.department_code;
//                    existing.DepartmentName = item.department_name;
//                    existing.DepartmentCode = item.unit_code;
//                    existing.DepartmentName = item.unit_name;
//                    existing.SubUnitCode = item.sub_unit_code;
//                    existing.SubUnitName = item.sub_unit_name;
//                    existing.LocationCode = item.location_code;
//                    existing.LocationName = item.location_name;
//                    existing.DeletedAt = item.deleted_at;
//                    existing.IsActive = item.deleted_at != null ? false : true;
//                    existing.UpdatedAt = item.updated_at;
//                }


//            }
//            await _storeContext.SaveChangesAsync(cancellationToken);
//            return Result.Success("Synced Sucessful");
//        }

//    }

//    public class ApiResponse
//    {
//        public int Status { get; set; }
//        public string Message { get; set; } = string.Empty;
//        public List<ChargingDto>? Data { get; set; }
//    }

//    public class ChargingDto
//    {
//        public int Id { get; set; }
//        public string code { get; set; } = string.Empty;
//        public string name { get; set; } = string.Empty;
//        public string company_code { get; set; } = string.Empty;
//        public string company_name { get; set; } = string.Empty;
//        public string business_unit_code { get; set; } = string.Empty;
//        public string business_unit_name { get; set; } = string.Empty;
//        public string department_code { get; set; } = string.Empty;
//        public string department_name { get; set; } = string.Empty;
//        public string unit_code { get; set; } = string.Empty;
//        public string unit_name { get; set; } = string.Empty;
//        public string sub_unit_code { get; set; } = string.Empty;
//        public string sub_unit_name { get; set; } = string.Empty;
//        public string location_code { get; set; } = string.Empty;
//        public string location_name { get; set; } = string.Empty;
//        public DateTime? created_at { get; set; }
//    public DateTime? updated_at { get; set; }
//    public DateTime? deleted_at { get; set; }
//}
//}