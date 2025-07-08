using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO
{
    public class AccountTitleDto
    {
        public int? SyncId { get; set; }
        public string AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public string AccountType { get; set; }
        public string AccountGroup { get; set; }
        public string AccountSubgroup { get; set; }
        public string FinancialStatement { get; set; }
        public string NormalBalance { get; set; }
        public string Allocation { get; set; }
        public string Unit { get; set; }
        public string Charging { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
