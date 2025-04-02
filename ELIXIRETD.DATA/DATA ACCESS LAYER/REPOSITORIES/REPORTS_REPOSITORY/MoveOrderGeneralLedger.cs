//using ELIXIRETD.DATA.CORE.ICONFIGURATION;
//using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
//using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
//{
//    [Route("api/etd-gl"), ApiController]
//    [AllowAnonymous]
//    public class ETDGL : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        public ETDGL(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpGet]
//        //[ApiKeyAuth]
//        public async Task<IActionResult> Get([FromQuery] ETDGLQuery query)
//        {
//            var result = await _mediator.Send(query);
//            if (result.IsSuccess)
//            {
//                return Ok(result.Value);
//            }
//            else
//            {
//                return BadRequest(result);
//            }

//        }

//        public class ETDGLQuery : IRequest<Result<List<ETDGLResult>>>
//        {
//            public string adjustment_month { get; set; }
//        }

//        public class ETDGLResult
//        {
//            public string SyncId { get; set; }
//            public string Mark1 { get; set; }
//            public string Mark2 { get; set; }
//            public string AssetCIP { get; set; }
//            public string AccountingTag { get; set; }
//            public string TransactionDate { get; set; }
//            public string ClientSupplier { get; set; }
//            public string AccountTitleCode { get; set; }
//            public string AccountTitle { get; set; }
//            public string CompanyCode { get; set; }
//            public string Company { get; set; }
//            public string DivisionCode { get; set; }
//            public string Division { get; set; }
//            public string DepartmentCode { get; set; }
//            public string Department { get; set; }
//            public string UnitCode { get; set; }
//            public string Unit { get; set; }
//            public string SubUnitCode { get; set; }
//            public string SubUnit { get; set; }
//            public string LocationCode { get; set; }
//            public string Location { get; set; }
//            public string PONumber { get; set; }
//            public string RRNumber { get; set; }
//            public string ReferenceNo { get; set; }
//            public string ItemCode { get; set; }
//            public string ItemDescription { get; set; }
//            public decimal? Quantity { get; set; }
//            public string UOM { get; set; }
//            public decimal? UnitPrice { get; set; }
//            public decimal? LineAmount { get; set; }
//            public string VoucherJournal { get; set; }
//            public string AccountType { get; set; }
//            public string DRCR { get; set; }
//            public string AssetCode { get; set; }
//            public string Asset { get; set; }
//            public string ServiceProviderCode { get; set; }
//            public string ServiceProvider { get; set; }
//            public string BOA { get; set; }
//            public string Allocation { get; set; }
//            public string AccountGroup { get; set; }
//            public string AccountSubGroup { get; set; }
//            public string FinancialStatement { get; set; }
//            public string UnitResponsible { get; set; }
//            public string Batch { get; set; }
//            public string Remarks { get; set; }
//            public string PayrollPeriod { get; set; }
//            public string Position { get; set; }
//            public string PayrollType { get; set; }
//            public string PayrollType2 { get; set; }
//            public string DepreciationDescription { get; set; }
//            public string RemainingDepreciationValue { get; set; }
//            public string UsefulLife { get; set; }
//            public string Month { get; set; }
//            public string Year { get; set; }
//            public string Particulars { get; set; }
//            public string Month2 { get; set; }
//            public string FarmType { get; set; }
//            public string JeanRemarks { get; set; }
//            public string From { get; set; }
//            public string ChangeTo { get; set; }
//            public string Reason { get; set; }
//            public string CheckingRemarks { get; set; }
//            public string BankName { get; set; }
//            public string ChequeNumber { get; set; }
//            public string ChequeVoucherNumber { get; set; }
//            public string BOA2 { get; set; }
//            public string System { get; set; }
//            public string Books { get; set; }
//        }

//        public class Handler : IRequestHandler<ETDGLQuery, Result<List<ETDGLResult>>>
//        {
//            private readonly StoreContext _context;
//            public Handler(StoreContext context)
//            {
//                _context = context;
//            }

//            public async Task<Result<List<ETDGLResult>>> Handle(ETDGLQuery request, CancellationToken cancellationToken)
//            {
//                if (string.IsNullOrWhiteSpace(request.adjustment_month))
//                {
//                    return Result.Success(new List<ETDGLResult>());
//                }

//                if (!DateTime.TryParseExact(request.adjustment_month, "yyyy-MM",
//                                            CultureInfo.InvariantCulture, DateTimeStyles.None,
//                                            out DateTime adjustmentMonth))
//                {
//                    throw new ArgumentException("Adjustment_month must be in the format yyyy-MM");
//                }

//                var startDate = new DateTime(adjustmentMonth.Year, adjustmentMonth.Month, 1);
//                var endDate = startDate.AddMonths(1);

//                var transaction = await _context.MoveOrders
//                    .Where(x => x.ApprovedDate >= startDate && x.ApprovedDate < endDate)
//                    .ToListAsync(cancellationToken);

//                var result = transaction.SelectMany(x => new List<ETDGLResult>
//                {
//                    //credit
//                    new ETDGLResult
//                    {
//                        SyncId = "ETD" + (x?.Id.ToString() ?? string.Empty),
//                        Mark1 = "",
//                        Mark2 = "",
//                        AssetCIP = "",
//                        AccountingTag = "",
//                        TransactionDate = "",
//                        ClientSupplier = "",
//                        AccountTitleCode = "",
//                        AccountTitle = "",
//                        CompanyCode = "",
//                        Company = "",
//                        DivisionCode = "",
//                        Division = "",
//                        DepartmentCode = "",
//                        Department = "",
//                        UnitCode = "",
//                        Unit = "",
//                        SubUnitCode = "",
//                        SubUnit = "",
//                        LocationCode = "",
//                        Location = "",
//                        PONumber = "",
//                        RRNumber = "",
//                        ReferenceNo = "",
//                        ItemCode = "",
//                        ItemDescription = "",
//                        Quantity = 0,
//                        UOM = "",
//                        UnitPrice = 0,
//                        LineAmount = 0,
//                        VoucherJournal = "",
//                        AccountType = "",
//                        DRCR = "",
//                        AssetCode = "",
//                        Asset= "",
//                        ServiceProviderCode = "",
//                        ServiceProvider = "",
//                        BOA = "",
//                        Allocation = "",
//                        AccountGroup = "",
//                        AccountSubGroup = "",
//                        FinancialStatement = "",
//                        UnitResponsible = "",
//                        Batch = "",
//                        Remarks = "",
//                        PayrollPeriod = "",
//                        Position = "",
//                        PayrollType = "",
//                        PayrollType2 = "",
//                        DepreciationDescription = "",
//                        RemainingDepreciationValue = "",
//                        UsefulLife = "",
//                        Month = "",
//                        Year = "",
//                        Particulars = "",
//                        Month2 = "",
//                        FarmType = "",
//                        JeanRemarks = "",
//                        From = "",
//                        ChangeTo = "",
//                        Reason = "",
//                        CheckingRemarks = "",
//                        BankName = "",
//                        ChequeNumber = "",
//                        ChequeVoucherNumber = "",
//                        BOA2 = "",
//                        System = "",
//                        Books = "",
//                    },

//                    //debit
//                    new ETDGLResult
//                    {
//                        SyncId = "ETD" + (x?.Id.ToString() ?? string.Empty),
//                        Mark1 = "",
//                        Mark2 = "",
//                        AssetCIP = "",
//                        AccountingTag = "",
//                        TransactionDate = "",
//                        ClientSupplier = "",
//                        AccountTitleCode = "",
//                        AccountTitle = "",
//                        CompanyCode = "",
//                        Company = "",
//                        DivisionCode = "",
//                        Division = "",
//                        DepartmentCode = "",
//                        Department = "",
//                        UnitCode = "",
//                        Unit = "",
//                        SubUnitCode = "",
//                        SubUnit = "",
//                        LocationCode = "",
//                        Location = "",
//                        PONumber = "",
//                        RRNumber = "",
//                        ReferenceNo = "",
//                        ItemCode = "",
//                        ItemDescription = "",
//                        Quantity = 0,
//                        UOM = "",
//                        UnitPrice = 0,
//                        LineAmount = 0,
//                        VoucherJournal = "",
//                        AccountType = "",
//                        DRCR = "",
//                        AssetCode = "",
//                        Asset= "",
//                        ServiceProviderCode = "",
//                        ServiceProvider = "",
//                        BOA = "",
//                        Allocation = "",
//                        AccountGroup = "",
//                        AccountSubGroup = "",
//                        FinancialStatement = "",
//                        UnitResponsible = "",
//                        Batch = "",
//                        Remarks = "",
//                        PayrollPeriod = "",
//                        Position = "",
//                        PayrollType = "",
//                        PayrollType2 = "",
//                        DepreciationDescription = "",
//                        RemainingDepreciationValue = "",
//                        UsefulLife = "",
//                        Month = "",
//                        Year = "",
//                        Particulars = "",
//                        Month2 = "",
//                        FarmType = "",
//                        JeanRemarks = "",
//                        From = "",
//                        ChangeTo = "",
//                        Reason = "",
//                        CheckingRemarks = "",
//                        BankName = "",
//                        ChequeNumber = "",
//                        ChequeVoucherNumber = "",
//                        BOA2 = "",
//                        System = "",
//                        Books = "",
//                    }
//                }).ToList();

//                return Result.Success(result);
//            }
//        }
//    }
//}
