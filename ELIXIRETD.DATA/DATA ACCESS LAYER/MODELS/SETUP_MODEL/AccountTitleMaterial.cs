using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class AccountTitleMaterial
    {
        public int Id { get; set; }
        public int? AccountTitleId { get; set; }
        public OneAccountTitle AccountTitle { get; set; }
        public int? MaterialId { get; set; }
        public Material Material { get; set; }
        public int? MaterialNo { get; set; }

    }
}
