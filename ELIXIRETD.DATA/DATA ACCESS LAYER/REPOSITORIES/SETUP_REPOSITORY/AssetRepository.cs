using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.Asset;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class AssetRepository : IAssetRepository
    {
        private readonly StoreContext _context;

        public AssetRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AssetNameAlreadyExist(string assetName)
        {
             return await _context.Assets.AnyAsync(x => x.AssetName == assetName);

        }

        public async Task<bool> AssetExist(int id)
        {
            return await _context.Assets.FindAsync(id) is not null ? true : false;
        }

        public async Task CreateAsset(Asset asset)
        {
            await _context.Assets.AddAsync(asset);
        }

        public async Task<PagedList<GetAssetDto>> GetAsset(UserParams userParams, string Search, bool? Archived, string Status)
        {
            IQueryable<Asset> query = _context.Assets
                .AsNoTracking();

            if (!string.IsNullOrEmpty(Search))
                query = query.Where(x => x.AssetCode.ToLower().Contains(Search.ToLower()) ||
                x.AssetName.ToLower().Contains(Search.ToLower()));

            if (Archived is not null)
                query = query.Where(x => x.IsActive == Archived);

            if (Status is not null)
                switch (Status)
                {
                    case "Manual":
                        query = query.Where(x => x.Manual.ToLower().Contains("Manual".ToLower()));
                        break;
                    default:
                        query = query.Where(x => x.Manual == null);
                        break;

                }

            var results = query
                .Select(x => new GetAssetDto
                {
                    Id = x.Id,
                    AssetNo = x.AssetNo,
                    AssetCode = x.AssetCode,
                    AssetName = x.AssetName,
                    AddedBy = x.AddedBy,
                    DateAdded = x.DateAdded.Date,
                    ModifyBy = x.ModifyBy,
                    ModifyDate = x.ModifyDate.Value.Date,
                    StatusSync = x.StatusSync,
                    SyncDate = x.SyncDate,
                    IsActive = x.IsActive,
                    Manual = x.Manual,
                });

            return await PagedList<GetAssetDto>.CreateAsync(results, userParams.PageNumber, userParams.PageSize);

        }

        public async Task ManualAddAsset(ManualAddAssetDto asset)
        {
           var assetExist = await _context.Assets
                .FirstOrDefaultAsync(x => x.Id == asset.Id);    

            if(assetExist is not null)
            {
                assetExist.AssetCode = asset.Asset_Code;
                assetExist.AssetName = asset.Asset_Name;
                assetExist.ModifyBy = asset.Modify_By;
                assetExist.ModifyDate = DateTime.Now;
            }
            else
            {
                var create = new Asset
                {
                    AssetCode = asset.Asset_Code,
                    AssetName = asset.Asset_Name,
                    AddedBy = asset.Added_By,
                    DateAdded = DateTime.Now,
                    Manual = "Manual"

                };

                await _context.Assets.AddAsync(create);
                
            }

        }

        public async Task UpdateAssetStatus(int id)
        {
          var update = await _context.Assets
                .FirstOrDefaultAsync(x => x.Id == id);

            update.IsActive = !update.IsActive; 
            
        }
    }
}
