using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.API.Controllers.OneRDF_CONTROLLER.ViewOneRdf
{
    public partial class OneRdfView
    {
        public class Handler : IRequestHandler<OneRdfViewQuery, PagedList<OneRdfViewResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<OneRdfViewResult>> Handle(OneRdfViewQuery request, CancellationToken cancellationToken)
            {


                IQueryable<OneRdf> viewQuery = _context.OneRdfs
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();

                if (!string.IsNullOrEmpty(request.Search))
                    viewQuery = viewQuery.Where(v => v.code.ToLower().Contains(request.Search));

                var results = viewQuery.Select(v => new OneRdfViewResult
                {
                    code = v.code,
                    name = v.name,
                    company_code = v.company_code,
                    company_name = v.company_name,
                    company_id = v.company_id,
                    business_unit_code = v.business_unit_code,
                    business_unit_name = v.business_unit_name,
                    business_unit_id = v.business_unit_id,
                    department_code = v.department_code,
                    department_name = v.department_name,
                    department_id = v.department_id,
                    department_unit_code= v.department_unit_code,
                    department_unit_name= v.department_unit_name,
                    department_unit_id= v.department_unit_id,
                    sub_unit_code = v.sub_unit_code,
                    sub_unit_name = v.sub_unit_name,
                    sub_unit_id = v.sub_unit_id,
                    location_code = v.location_code,
                    location_name = v.location_name,
                    location_id = v.location_id,
                });
                return await PagedList<OneRdfViewResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }

        }
    }
}
