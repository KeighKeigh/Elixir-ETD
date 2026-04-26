using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL
{
    public class PendingRequest : BaseEntity
    {
        public string IdPrefix { get; set; } = string.Empty;
        public string IdNo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
    }
}
