using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.Asset
{
    public class GetAssetDto
    {
        public int Id { get; set; }
        public int? AssetNo { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime SyncDate { get; set; }
        public string StatusSync { get; set; }
        public bool IsActive { get; set; }
        public string Manual { get; set; }
    }
}
