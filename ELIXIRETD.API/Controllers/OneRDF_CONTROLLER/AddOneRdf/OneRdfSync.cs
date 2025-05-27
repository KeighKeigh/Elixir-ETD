//using DocumentFormat.OpenXml.Spreadsheet;
//using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ONERDF_MODEL;
//using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace ELIXIRETD.API.Controllers.OneRDF_CONTROLLER
//{
//    public partial class OneRdfSync
//    {
//        public class Handler : IRequestHandler<OneRdfSyncBatchCommand, Result>
//        {
//            private readonly StoreContext _context;

//            public Handler(StoreContext context)
//            {
//                _context = context;
//            }

//            public async Task<Result> Handle(OneRdfSyncBatchCommand command, CancellationToken cancellationToken)
//            {


//                var allCommands = command;

//                var incomingSyncIds = allCommands
//                    .Where(x => x.sync_id != null)
//                    .Select(x => x.sync_id)
//                    .ToList();
//                var existingSyncIds = await _context.OneRdfs
//                    .Where(x => incomingSyncIds.Contains(x.sync_id))
//                    .Select(x => x.sync_id)
//                    .ToListAsync(cancellationToken);

//                var updateSync = allCommands.Where(x => existingSyncIds.Contains(x.sync_id)).ToList();
//                var newSync = allCommands.Where(x => !existingSyncIds.Contains(x.sync_id)).ToList();


//                await SyncData(newSync, cancellationToken);
//                await UpdateSyncData(updateSync, cancellationToken);

//                await _context.SaveChangesAsync();

//                return Result.Success();
//            }



//            public async Task SyncData(List<OneRdfSyncCommand> command, CancellationToken cancellationToken)
//            {
//                var dataSync = command.Select(x => new OneRdf
//                {
//                    code = x.code,
//                    name = x.name,
//                    sync_id = x.sync_id,
//                    company_code = x.company_code,
//                    company_name = x.company_name,
//                    company_id = x.company_id,
//                    business_unit_code = x.business_unit_code,
//                    business_unit_name = x.business_unit_name,
//                    business_unit_id = x.business_unit_id,
//                    department_code = x.department_code,
//                    department_name = x.department_name,
//                    department_id = x.department_id,
//                    department_unit_code = x.department_unit_code,
//                    department_unit_name = x.department_unit_name,
//                    department_unit_id = x.department_unit_id,
//                    sub_unit_code = x.sub_unit_code,
//                    sub_unit_name = x.sub_unit_name,
//                    sub_unit_id = x.sub_unit_id,
//                    location_code = x.location_code,
//                    location_name = x.location_name,
//                    location_id = x.location_id,
//                    deleted_at = x.deleted_at,

//                }).ToList();


//                await _context.OneRdfs.AddRangeAsync(dataSync);
//            }

//            public async Task UpdateSyncData(List<OneRdfSyncCommand> command, CancellationToken cancellationToken)
//            {
//                foreach (OneRdfSyncCommand data in command)
//                {
//                    var updatedata = _context.OneRdfs.FirstOrDefault(o => o.sync_id == data.sync_id);
//                    if (updatedata != null)
//                    {
//                        updatedata.code = data.code;
//                        updatedata.name = data.name;
//                        updatedata.company_code = data.company_code;
//                        updatedata.company_name = data.company_name;
//                        updatedata.company_id = data.company_id;
//                        updatedata.business_unit_code = data.business_unit_code;
//                        updatedata.business_unit_name = data.business_unit_name;
//                        updatedata.business_unit_id = data.business_unit_id;
//                        updatedata.department_code = data.department_code;
//                        updatedata.department_name = data.department_name;
//                        updatedata.department_id = data.department_id;
//                        updatedata.department_unit_code = data.department_unit_code;
//                        updatedata.department_unit_name = data.department_unit_name;
//                        updatedata.department_unit_id = data.department_unit_id;
//                        updatedata.sub_unit_code = data.sub_unit_code;
//                        updatedata.sub_unit_name = data.sub_unit_name;
//                        updatedata.sub_unit_id = data.sub_unit_id;
//                        updatedata.location_code = data.location_code;
//                        updatedata.location_name = data.location_name;
//                        updatedata.location_id = data.location_id;
//                        updatedata.deleted_at = data.deleted_at;
//                    }

//                }
//            }
//        }
//    }
//}
