using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO
{
    public class OneChargingDto
    {
        
        public int? sync_id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string company_code { get; set; }
            public string company_name { get; set; }
            public string business_unit_code { get; set; }
            public string business_unit_name { get; set; }
            public string department_code { get; set; }
            public string department_name { get; set; }
        
        public string department_unit_code { get; set; }
        
        public string department_unit_name { get; set; }
            public string sub_unit_code { get; set; }
            public string sub_unit_name { get; set; }
            public string location_code { get; set; }
            public string location_name { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }


    }
}
