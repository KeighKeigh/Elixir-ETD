using DocumentFormat.OpenXml.Spreadsheet;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.Asset;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.API.Controllers.SETUP_CONTROLLER
{
    [Route("api/asset")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public AssetController(IUnitOfWork unitOfWork, StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpPost("sync-asset")]
        public async Task<IActionResult> SyncAsset(SyncAssetDto[] asset)
        {
            var duplicateList = new List<SyncAssetDto>();
            var availableImport = new List<SyncAssetDto>();

            var removeAssetNoList = new List<int?>();

            foreach (var item in asset)
            {
                if (asset.Count(x => x.AssetNo == item.AssetNo && x.Asset_Name == item.Asset_Name) > 1)
                {
                    duplicateList.Add(item);
                    continue;
                }

                var assetNoExist = await _context.Assets.FirstOrDefaultAsync(x => x.AssetNo == item.AssetNo);

                if (assetNoExist is not null)
                {
                    bool isChange = false;

                    if (assetNoExist.AssetCode != item.Asset_Code)
                        assetNoExist.AssetCode = item.Asset_Code;
                    isChange = true;


                    if (assetNoExist.AssetName != item.Asset_Name)
                        assetNoExist.AssetName = item.Asset_Name;
                    isChange = true;

                    if (isChange)
                    {
                        assetNoExist.SyncDate = DateTime.Now;
                        assetNoExist.ModifyBy = User.Identity.Name;
                        assetNoExist.ModifyDate = DateTime.Now;
                        assetNoExist.StatusSync = "New update";

                    }

                    removeAssetNoList.Add(item.AssetNo);
                }
                else
                {
                    var create = new Asset
                    {
                        
                        AssetNo = item.AssetNo,
                        AssetCode = item.Asset_Code,
                        AssetName = item.Asset_Name,
                        AddedBy = User.Identity.Name,
                        SyncDate = DateTime.Now,
                        StatusSync = "New Added",
                        DateAdded = DateTime.Now,

                    };

                    availableImport.Add(item);
                    await _unitOfWork.Asset.CreateAsset(create);
                }

            }

            var assetList = await _context.Assets
                .Where(x => x.IsActive && x.AssetNo != null)
                .ToListAsync();

            var removeAssetList = assetList.Where(x => !removeAssetNoList.Contains(x.AssetNo));

           foreach(var remove in  removeAssetList)
           {
                var removeAsset = await _context.Assets.FirstOrDefaultAsync(x => x.AssetNo == remove.AssetNo);
                removeAsset.IsActive = false;
           }

            var resultlist = new
            {
                AvailableImport = availableImport,
                DuplicateList = duplicateList,
            };

            if(duplicateList.Count() > 0)
            {
                return BadRequest(resultlist);
            }

            await _unitOfWork.CompleteAsync();
            return Ok("Success");
        }

        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<UomDto>>> GetAsset([FromQuery]UserParams userParams,[FromQuery] string Search, [FromQuery]bool? Archived,[FromQuery] string Status)
        {
            var asset = await _unitOfWork.Asset.GetAsset(userParams, Search, Archived,Status);

            Response.AddPaginationHeader(asset.CurrentPage, asset.PageSize, asset.TotalCount, asset.TotalPages, asset.HasNextPage, asset.HasPreviousPage);


            var companyResult = new
            {
                asset,
                asset.CurrentPage,
                asset.PageSize,
                asset.TotalCount,
                asset.TotalPages,
                asset.HasNextPage,
                asset.HasPreviousPage
            };

            return Ok(companyResult);

        }

        [HttpPatch("update-status/{id}")]
        public async Task<IActionResult> UpdateAssetStatus([FromRoute] int id)
        {
            var assetExist = await _unitOfWork.Asset.AssetExist(id);

            if (!assetExist)
                return BadRequest("Asset not exist!");

            await _unitOfWork.Asset.UpdateAssetStatus(id);

            await _unitOfWork.CompleteAsync();

            return Ok("Success");
        }

        [HttpPost("manual")]
        public async Task<IActionResult> ManualAddAsset([FromBody]ManualAddAssetDto asset)
        {
            var assetNameAlreadyExist = await _unitOfWork.Asset.AssetNameAlreadyExist(asset.Asset_Name);

            if (assetNameAlreadyExist)
                return BadRequest("Asset name already exist");

            await _unitOfWork.Asset.ManualAddAsset(asset);

            await _unitOfWork.CompleteAsync();
            return Ok("success");
        }
        
    }
}
