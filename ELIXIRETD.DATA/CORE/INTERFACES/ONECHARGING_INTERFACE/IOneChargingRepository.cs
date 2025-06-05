using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ONECHARGING_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.ONECHARGING_INTERFACE
{
    public interface IOneChargingRepository
    {
        Task<bool> AddDataOneCharging(List<OneChargingDto> data);
        Task<PagedList<OneChargingDto>> GetOneChargingPagination(UserParams userParams, bool? status, string search);
        Task<bool> AddAccountTitle(List<OneAccountTitleDto> data);
        Task<PagedList<OneAccountTitleDto>> GetAccountTitle(UserParams userParams, bool? status, string search);
    }
}
