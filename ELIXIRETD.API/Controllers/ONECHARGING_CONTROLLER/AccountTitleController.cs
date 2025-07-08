using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.API.Controllers.ONECHARGING_CONTROLLER
{
    [Route("api/accountTitle")]
    [ApiController]
    [AllowAnonymous]
    public class AccountTitleController : ControllerBase
    {
        public readonly IMediator _mediator;

        public AccountTitleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        

        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            var result = await _mediator.Send(new ImportAccountTitleCommand());
            return result.IsFailure ? BadRequest(result) : Ok(result);
        }

        public class ImportAccountTitleCommand : IRequest<Result> { }

        public class Handler : IRequestHandler<ImportAccountTitleCommand, Result>
        {
            public readonly StoreContext _storeContext;
            public readonly IHttpClientFactory _httpClientFactory;

            public Handler(StoreContext storeContext, IHttpClientFactory httpClientFactory)
            {
                _storeContext = storeContext;
                _httpClientFactory = httpClientFactory;
            }

            public async Task<Result> Handle(ImportAccountTitleCommand request, CancellationToken cancellationToken)
            {
                var client = _httpClientFactory.CreateClient();
                var endpoint = "https://api-one.rdfmis.com/api/account_title_external?per_page=10&page=1&pagination=none";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("API_KEY", "hello world!");

                var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return Result.Failure(new Error("Failed to retrieve data from the source API.", "NOT_FOUND"));
                }

                var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);
                if (response == null || response.Data == null)
                {
                    return Result.Failure(new Error("Failed to parse response from the source API.", "INVALID_RESPONSE"));
                }

                foreach (var item in response.Data)
                {
                    var existing = await _storeContext.OneAccountTitles
                        .FirstOrDefaultAsync(x => x.SyncId == item.id, cancellationToken);

                    if (existing == null)
                    {
                        var accountTitle = new OneAccountTitle
                        {
                            SyncId = item.id,
                            AccountCode = item.code,
                            AccountDescription = item.name,
                            AccountType = item.account_type_name,
                            AccountGroup = item.account_group_name,
                            AccountSubgroup = item.account_sub_group_name,
                            FinancialStatement = item.financial_statement_name,
                            NormalBalance = item.normal_balance_name,
                            Allocation = item.allocation_name,
                            Unit = item.account_unit_name,
                            Charging = item.charge,
                            CreatedAt = item.created_at,
                            UpdatedAt = item.updated_at,
                            DeletedAt = item.deleted_at
                        };

                        await _storeContext.OneAccountTitles.AddAsync(accountTitle, cancellationToken);
                    }
                    else if (existing.UpdatedAt != item.updated_at)
                    {
                        existing.AccountCode = item.code;
                        existing.AccountDescription = item.name;
                        existing.AccountType = item.account_type_name;
                        existing.AccountGroup = item.account_group_name;
                        existing.AccountSubgroup = item.account_sub_group_name;
                        existing.FinancialStatement = item.financial_statement_name;
                        existing.NormalBalance = item.normal_balance_name;
                        existing.Allocation = item.allocation_name;
                        existing.Unit = item.account_unit_name;
                        existing.Charging = item.charge;
                        existing.UpdatedAt = item.updated_at;
                        existing.DeletedAt = item.deleted_at;
                    }
                }

                await _storeContext.SaveChangesAsync(cancellationToken);
                return Result.Success("Account Titles synced successfully.");
            }

        }

        public class ApiResponse
        {
            public int? Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public List<AccountTitleDto> Data { get; set; }
        }

        public class AccountTitleDto
        {
            public int? id { get; set; }
            public string code { get; set; }
            public string name { get; set; }

            public string account_type_name { get; set; }
            public string account_group_name { get; set; }
            public string account_sub_group_name { get; set; }
            public string financial_statement_name { get; set; }
            public string normal_balance_name { get; set; }
            public string allocation_name { get; set; }
            public string account_unit_name { get; set; }
            public string charge { get; set; }

            public DateTime? created_at { get; set; }
            public DateTime? updated_at { get; set; }
            public DateTime? deleted_at { get; set; }


        }

    }
}
