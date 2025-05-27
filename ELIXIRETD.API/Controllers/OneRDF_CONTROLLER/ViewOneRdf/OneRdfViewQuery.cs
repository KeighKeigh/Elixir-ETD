using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using MediatR;

namespace ELIXIRETD.API.Controllers.OneRDF_CONTROLLER.ViewOneRdf
{
    public partial class OneRdfView
    {
        public class OneRdfViewQuery : UserParams, IRequest<PagedList<OneRdfViewResult>>
        {
            public string Search {  get; set; }

        }
    }
}
