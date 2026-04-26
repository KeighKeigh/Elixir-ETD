using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static ELIXIRETD.API.Controllers.YMIRSYNC_CONTROLLER.GetPOFromYmirHandler;

namespace ELIXIRETD.API.Controllers.YMIRSYNC_CONTROLLER
{

    [Route("api/ymir-sync")]
    [AllowAnonymous]
    [ApiController]
    public class YmirSyncController : ControllerBase
    {
        private readonly IMediator _mediator;

        public YmirSyncController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<YmirResponse>>> GetPOFromYmir(
            [FromQuery(Name = "system_name")] string systemName,
            [FromQuery(Name = "from")] DateTime? from,
            [FromQuery(Name = "to")] DateTime? to)
        {
            var query = new GetPOFromYmirQuery
            {
                SystemName = systemName,
                From = from,
                To = to
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }

    public class GetPOFromYmirQuery : IRequest<List<YmirResponse>>
    {
        public string SystemName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }


    public class GetPOFromYmirHandler : IRequestHandler<GetPOFromYmirQuery, List<YmirResponse>>
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public GetPOFromYmirHandler(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }


        public class YmirResponse
        {
            public int id { get; set; }
            public string rr_year_number_id { get; set; }
            public int? po_id { get; set; }
            public string pr_id { get; set; }
            public int? received_by { get; set; }
            public string tagging_id { get; set; }
            public string transaction_date { get; set; }
            public string attachment { get; set; }
            public string late_attachment { get; set; }
            public string reason { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }

            public List<YmirOrders> rr_orders { get; set; }
            
        }

        public class YmirOrders
        {
            public int id { get; set; }
            public string rr_number { get; set; }
            public int? rr_id { get; set; }
            public string po_id { get; set; }
            public string pr_id { get; set; }
            public string item_id { get; set; }
            public string item_code { get; set; }
            public string item_name { get; set; }
            public int? quantity_receive { get; set; }
            public string remaining { get; set; }
            public string shipment_no { get; set; }
            public string delivery_date { get; set; }
            public string rr_date { get; set; }
            public string late_attachment { get; set; }
            public string attachment { get; set; }
            public int? sync { get; set; }
            public int? etd_sync { get; set; }
            public int? system_sync { get; set; }
            public int? f_tagged { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }

            public string UMPriceToString { get; set; }

            public Orderss order { get; set; }
            public PRTransaction pr_transaction { get; set; }
            public POTransaction po_transaction { get; set; }
        }


        public class Orderss
        {
            public int id { get; set; }
            public int pr_id { get; set; }
            public string reference_no { get; set; }
            public string pr_item_id { get; set; }
            public int? po_id { get; set; }
            public string item_id { get; set; }
            public string item_code { get; set; }
            public string item_name { get; set; }
            public int? uom_id { get; set; }
            public int? supplier_id { get; set; }
            public string attachment { get; set; }
            public string buyer_id { get; set; }
            public string buyer_name { get; set; }
            public decimal? price { get; set; }
            public string item_stock { get; set; }
            public decimal? quantity { get; set; }
            public decimal? quantity_serve { get; set; }
            public decimal? total_price { get; set; }
            public string remarks { get; set; }
            public int? warehouse_id { get; set; }
            public int? category_id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }
            public int? umd_sync { get; set; }
            public int? system_sync { get; set; }
            public string price_string { get; set; }
            public Uomss uom { get; set; }

        }
        public class Uomss
        {
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public int? is_integer { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }
        }

        public class PRTransaction
        {
            public int id { get; set; }
            public string pr_year_number_id { get; set; }
            public int? pr_number { get; set; }
            public string transaction_no { get; set; }
            public string pr_description { get; set; }
            public string date_needed { get; set; }
            public int user_id { get; set; }
            public int type_id { get; set; }
            public string type_name { get; set; }
            public int? one_charging_id { get; set; }
            public string one_charging_sync_id { get; set; }
            public string one_charging_code { get; set; }
            public string one_charging_name { get; set; }
            public string business_unit_id { get; set; }
            public string business_unit_code { get; set; }
            public string business_unit_name { get; set; }
            public string company_id { get; set; }
            public string company_code { get; set; }
            public string company_name { get; set; }
            public string department_id { get; set; }
            public string department_code { get; set; }
            public string department_name { get; set; }
            public string department_unit_id { get; set; }
            public string department_unit_code { get; set; }
            public string department_unit_name { get; set; }
            public string location_id { get; set; }
            public string location_code { get; set; }
            public string location_name { get; set; }
            public string sub_unit_id { get; set; }
            public string sub_unit_code { get; set; }
            public string sub_unit_name { get; set; }
            public int? account_title_id { get; set; }
            public string account_title_name { get; set; }
            public string supplier_id { get; set; }
            public string supplier_name { get; set; }
            public string module_name { get; set; }
            public string layer { get; set; }
            public string cap_ex { get; set; }
            public string status { get; set; }
            public string asset_code { get; set; }
            public string transaction_number { get; set; }
            public string description { get; set; }
            public string reason { get; set; }
            public string edit_remarks { get; set; }
            public string pcf_remarks { get; set; }
            public string ship_to_id { get; set; }
            public string ship_to_name { get; set; }
            public string approver_remarks { get; set; }
            public string void_generated_code { get; set; }
            public string asset { get; set; }
            public string sgp { get; set; }
            public string f1 { get; set; }
            public string f2 { get; set; }
            public string rush { get; set; }
            public string place_order { get; set; }
            public string for_po_only { get; set; }
            public string for_po_only_id { get; set; }
            public string user_tagging { get; set; }
            public string vrid { get; set; }
            public string for_marketing { get; set; }
            public string helpdesk_id { get; set; }
            public string approved_at { get; set; }
            public string rejected_at { get; set; }
            public string voided_at { get; set; }
            public string cancelled_at { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }
        }

        public class POTransaction
        {
            public int id { get; set; }
            public string po_year_number_id { get; set; }
            public int? pr_number { get; set; }
            public int? po_number { get; set; }
            public string po_description { get; set; }
            public string date_needed { get; set; }
            public int? user_id { get; set; }
            public int? type_id { get; set; }
            public string type_name { get; set; }
            public int? one_charging_id { get; set; }
            public string one_charging_sync_id { get; set; }
            public string one_charging_code { get; set; }
            public string one_charging_name { get; set; }
            public string business_unit_id { get; set; }
            public string business_unit_code { get; set; }
            public string business_unit_name { get; set; }
            public string company_id { get; set; }
            public string company_code { get; set; }
            public string company_name { get; set; }
            public string department_id { get; set; }
            public string department_code { get; set; }
            public string department_name { get; set; }
            public string department_unit_id { get; set; }
            public string department_unit_code { get; set; }
            public string department_unit_name { get; set; }
            public string location_id { get; set; }
            public string location_code { get; set; }
            public string location_name { get; set; }
            public string sub_unit_id { get; set; }
            public string sub_unit_code { get; set; }
            public string sub_unit_name { get; set; }
            public int? account_title_id { get; set; }
            public string account_title_name { get; set; }
            public int? supplier_id { get; set; }
            public string supplier_name { get; set; }
            public string module_name { get; set; }
            public string layer { get; set; }
            public string cap_ex { get; set; }
            public string status { get; set; }
            public string asset_code { get; set; }
            public string transaction_number { get; set; }
            public string description { get; set; }
            public string reason { get; set; }
            public string edit_remarks { get; set; }
            public string pcf_remarks { get; set; }
            public string ship_to_id { get; set; }
            public string ship_to_name { get; set; }
            public string approver_remarks { get; set; }
            public string void_generated_code { get; set; }
            public string asset { get; set; }
            public string sgp { get; set; }
            public string f1 { get; set; }
            public string f2 { get; set; }
            public string rush { get; set; }
            public string place_order { get; set; }
            public string for_po_only { get; set; }
            public string for_po_only_id { get; set; }
            public string user_tagging { get; set; }
            public string vrid { get; set; }
            public string for_marketing { get; set; }
            public string helpdesk_id { get; set; }
            public string approved_at { get; set; }
            public string rejected_at { get; set; }
            public string voided_at { get; set; }
            public string cancelled_at { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string deleted_at { get; set; }
        }
        public async Task<List<YmirResponse>> Handle(GetPOFromYmirQuery request, CancellationToken cancellationToken)
        {
            var token = _config["Ymir:BearerToken"];
            var baseUrl = "https://rdfymir.com/backend/public/api/etd_api";
            //var baseUrl = "https://pretestomega.rdfymir.com/backend/public/api/etd_api"; //pretest

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.SystemName))

                queryParams.Add($"system_name={Uri.EscapeDataString(request.SystemName)}");

            if (request.From.HasValue)

                queryParams.Add($"from={request.From.Value:yyyy-MM-dd}");

            if (request.To.HasValue)

                queryParams.Add($"to={request.To.Value:yyyy-MM-dd}");

            var query = string.Join("&", queryParams);

            var requestUrl = string.IsNullOrWhiteSpace(query) ? baseUrl : $"{baseUrl}?{query}";

            _httpClient.DefaultRequestHeaders.Remove("Token");
            _httpClient.DefaultRequestHeaders.Add("Token", $"Bearer {token}");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);//pretest

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();



            List<YmirResponse> ymirResults;
            try
            {
                ymirResults = JsonSerializer.Deserialize<List<YmirResponse>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch 
            {
                ymirResults = new List<YmirResponse>();
            }

            if (ymirResults == null) return new List<YmirResponse>();


            foreach (var po in ymirResults)
            {
                foreach (var order in po.rr_orders)
                {
                    order.UMPriceToString = order.order.price_string;
                }
            }

            return ymirResults;
        }
    }

}
