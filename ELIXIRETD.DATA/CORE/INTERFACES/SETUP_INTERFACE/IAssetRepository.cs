using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.Asset;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IAssetRepository
    {
        Task CreateAsset(Asset asset);

        Task ManualAddAsset(ManualAddAssetDto asset); 

        Task<PagedList<GetAssetDto>> GetAsset(UserParams userParams, string Search, bool? Archived, string Status);

        Task UpdateAssetStatus(int id);

        Task<bool>AssetExist(int id);

        Task<bool>AssetNameAlreadyExist(string assetName);

        
    }
}
