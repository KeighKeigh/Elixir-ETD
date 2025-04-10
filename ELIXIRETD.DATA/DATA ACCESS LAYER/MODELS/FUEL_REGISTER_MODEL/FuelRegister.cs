﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL
{
    public class FuelRegister : BaseEntity
    {
       
        public string Source { get; set; }
        public string RequestorId { get; set; }
        public string RequestorName { get; set; }
        public string Company_Code { get; set; }
        public string Company_Name { get; set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }

        public string Location_Code { get; set; }
        public string Location_Name { get; set; }

        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get; set; }
        public string EmpId { get; set; }
        public string Fullname { get; set; }

        public int? AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public decimal? Odometer { get; set; }

        public string Added_By { get; set; }
        public DateTime Created_At {  get; set; } = DateTime.Now;
        public string Modified_By {  get; set; }
        public DateTime? Updated_At { get; set; }

        public bool Is_Approve { get; set; }
        public string Approve_By { get; set; }
        public DateTime? Approve_At { get; set; }

        public bool? Is_Transact {  get; set; }
        public string Transact_By { get; set; }
        public DateTime? Transact_At { get; set; }

        public bool Is_Reject { get; set; } = false;
        public string Reject_Remarks { get; set; }
        public string Reject_By { get; set; }

        public bool Is_Active { get; set; } = true;
        public string Remarks { get; set; }

        public ICollection<FuelRegisterDetail> FuelRegisterDetails { get; set; }

    }
}
