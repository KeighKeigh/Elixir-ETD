using MediatR;

namespace ELIXIRETD.API.Controllers.OneRDF_CONTROLLER
{

    public class OneRdfSyncCommand
    {
        public int? Id { get; set; }
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
