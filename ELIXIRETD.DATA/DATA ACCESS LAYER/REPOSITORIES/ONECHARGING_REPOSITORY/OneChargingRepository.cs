using ELIXIRETD.DATA.CORE.INTERFACES.ONECHARGING_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ONECHARGING_REPOSITORY
{
    public class OneChargingRepository : IOneChargingRepository
    {
        private readonly StoreContext _context;
        public OneChargingRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddDataOneCharging(List<OneChargingDto> data)
        {
            var allCommands = data;

            var incomingSyncIds = allCommands
                .Where(x => x.sync_id != null)
                .Select(x => x.sync_id)
                .ToList();
            var existingSyncIds = await _context.OneRdfs
                .Where(x => incomingSyncIds.Contains(x.sync_id))
                .Select(x => x.sync_id).ToListAsync();

            var updateSync = allCommands.Where(x => existingSyncIds.Contains(x.sync_id)).ToList();
            var newSync = allCommands.Where(x => !existingSyncIds.Contains(x.sync_id)).ToList();


            var dataSync = newSync.Select(x => new OneRdf
            {
                code = x.code,
                name = x.name,
                sync_id = x.sync_id,
                company_code = x.company_code,
                company_name = x.company_name,
                business_unit_code = x.business_unit_code,
                business_unit_name = x.business_unit_name,
                department_code = x.department_code,
                department_name = x.department_name,
                department_unit_code = x.department_unit_code,
                department_unit_name = x.department_unit_name,
                sub_unit_code = x.sub_unit_code,
                sub_unit_name = x.sub_unit_name,
                location_code = x.location_code,
                location_name = x.location_name,
                deleted_at = x.deleted_at,
                IsActive = x.deleted_at != null ? false : true,

            }).ToList();


            await _context.OneRdfs.AddRangeAsync(dataSync);

            foreach (OneChargingDto datas in updateSync)
            {
                var updatedata = _context.OneRdfs.FirstOrDefault(o => o.sync_id == datas.sync_id);
                if (updatedata != null)
                {
                    updatedata.code = datas.code;
                    updatedata.name = datas.name;
                    updatedata.company_code = datas.company_code;
                    updatedata.company_name = datas.company_name;
                    updatedata.business_unit_code = datas.business_unit_code;
                    updatedata.business_unit_name = datas.business_unit_name;
                    updatedata.department_code = datas.department_code;
                    updatedata.department_name = datas.department_name;
                    updatedata.department_unit_code = datas.department_unit_code;
                    updatedata.department_unit_name = datas.department_unit_name;
                    updatedata.sub_unit_code = datas.sub_unit_code;
                    updatedata.sub_unit_name = datas.sub_unit_name;
                    updatedata.location_code = datas.location_code;
                    updatedata.location_name = datas.location_name;
                    updatedata.deleted_at = datas.deleted_at;
                    updatedata.IsActive = datas.deleted_at != null ? false : true;
                }

            }

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<PagedList<OneChargingDto>> GetOneChargingPagination(UserParams userParams, bool? status, string search)
        {
            var result = _context.OneRdfs.Select(x => new OneChargingDto
            {
                sync_id = x.sync_id,
                code = x.code,
                name = x.name,
                company_code = x.company_code,
                company_name = x.company_name,
                business_unit_code = x.business_unit_code,
                business_unit_name = x.business_unit_name,
                department_code = x.department_code,
                department_name = x.department_name,
                department_unit_code = x.department_unit_code,
                department_unit_name = x.department_unit_name,
                sub_unit_code = x.sub_unit_code,
                sub_unit_name = x.sub_unit_name,
                location_code = x.location_code,
                location_name = x.location_name,
                IsActive = x.IsActive,
                UpdatedAt = x.UpdatedAt,

            });

            if (status != null)
            {
                result = result.Where(x => x.IsActive == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(x => Convert.ToString(x.code).ToLower().Contains(search.Trim().ToLower())
                                        || Convert.ToString(x.name).ToLower().Contains(search.Trim().ToLower()));
            }
            return await PagedList<OneChargingDto>.CreateAsync(result, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<OneChargingAccountTitleDto>> GetAccountTitlePagination(UserParams userParams, bool? status, string search)
        {
            var result = _context.OneAccountTitles.Select(x => new OneChargingAccountTitleDto
            {
                SyncId = x.SyncId,
                AccountCode = x.AccountCode,
                AccountDescription = x.AccountDescription,
                AccountType = x.AccountType,
                AccountGroup = x.AccountGroup,
                AccountSubgroup = x.AccountSubgroup,
                FinancialStatement = x.FinancialStatement,
                NormalBalance = x.NormalBalance,
                Allocation = x.Allocation,
                Unit = x.Unit,
                Charging = x.Charging,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                IsActive = x.IsActive,

            });

            if (status != null)
            {
                result = result.Where(x => x.IsActive == status);
            }

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(x => Convert.ToString(x.AccountCode).ToLower().Contains(search.Trim().ToLower())
                                        || Convert.ToString(x.AccountDescription).ToLower().Contains(search.Trim().ToLower()));
            }
            return await PagedList<OneChargingAccountTitleDto>.CreateAsync(result, userParams.PageNumber, userParams.PageSize);
        }


    }
}
