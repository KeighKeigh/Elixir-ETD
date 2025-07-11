using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using ELIXIRETD.API.Authentication;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.Migrations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDF.Arcana.API.Features.Authenticate.AuthXApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ELIXIRETD.API.Controllers.ETDGL_CONTROLLER
{
    [Route("api/etd-gl"), ApiController]
    [AllowAnonymous]
    public class ETDGL : ControllerBase
    {
        private readonly IMediator _mediator;
        public ETDGL(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ApiKeyAuth]
        public async Task<IActionResult> Get([FromQuery] ETDGLQuery query)
        {
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result);
            }

        }

        public class ETDGLQuery : IRequest<Result<List<ETDGLResult>>>
        {
            public string adjustment_month { get; set; }
          
        }

        public class ETDGLResult
        {
            public string SyncId { get; set; }
            public string Mark1 { get; set; }
            public string Mark2 { get; set; }
            public string AssetCIP { get; set; }
            public string AccountingTag { get; set; }
            public DateTime? TransactionDate { get; set; }
            public string ClientSupplier { get; set; }
            public string AccountTitleCode { get; set; }
            public string AccountTitle { get; set; }
            public string CompanyCode { get; set; }
            public string Company { get; set; }
            public string DivisionCode { get; set; }
            public string Division { get; set; }
            public string DepartmentCode { get; set; }
            public string Department { get; set; }
            public string UnitCode { get; set; }
            public string Unit { get; set; }
            public string SubUnitCode { get; set; }
            public string SubUnit { get; set; }
            public string LocationCode { get; set; }
            public string Location { get; set; }
            public string PONumber { get; set; }
            public string RRNumber { get; set; }
            public string ReferenceNo { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public decimal? Quantity { get; set; }
            public string UOM { get; set; }
            public decimal? UnitPrice { get; set; }
            public decimal? LineAmount { get; set; }
            public string VoucherJournal { get; set; }
            public string AccountType { get; set; }
            public string DRCR { get; set; }
            public string AssetCode { get; set; }
            public string Asset { get; set; }
            public string ServiceProviderCode { get; set; }
            public string ServiceProvider { get; set; }
            public string BOA { get; set; }
            public string Allocation { get; set; }
            public string AccountGroup { get; set; }
            public string AccountSubGroup { get; set; }
            public string FinancialStatement { get; set; }
            public string UnitResponsible { get; set; }
            public string Batch { get; set; }
            public string Remarks { get; set; }
            public string PayrollPeriod { get; set; }
            public string Position { get; set; }
            public string PayrollType { get; set; }
            public string PayrollType2 { get; set; }
            public string DepreciationDescription { get; set; }
            public string RemainingDepreciationValue { get; set; }
            public string UsefulLife { get; set; }
            public string Month { get; set; }
            public string Year { get; set; }
            public string Particulars { get; set; }
            public string Month2 { get; set; }
            public string FarmType { get; set; }
            public string Adjustment { get; set; }
            public string From { get; set; }
            public string ChangeTo { get; set; }
            public string Reason { get; set; }
            public string CheckingRemarks { get; set; }
            public string BankName { get; set; }
            public string ChequeNumber { get; set; }
            public string ChequeVoucherNumber { get; set; }
            public string ChequeDate { get; set; }
            public string ReleasedDate { get; set; }
            public string BOA2 { get; set; }
            public string System { get; set; }
            public string Books { get; set; }
            public string ChargingCode {  get; set; }
            public string ChargingName { get; set; }
        }

        public class Handler : IRequestHandler<ETDGLQuery, Result<List<ETDGLResult>>>
        {
            private readonly StoreContext _context;
            
            public Handler(StoreContext context)
            {
                _context = context;
                
            }
           // Type desc
          
          
            public async Task<Result<List<ETDGLResult>>> Handle(ETDGLQuery request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.adjustment_month))
                {
                    return Result.Success(new List<ETDGLResult>());
                }

                if (!DateTime.TryParseExact(request.adjustment_month, "yyyy-MM",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None,
                                            out DateTime adjustmentMonth))
                {
                    throw new ArgumentException("Adjustment_month must be in the format yyyy-MM");
                }

                var startDate = new DateTime(adjustmentMonth.Year, adjustmentMonth.Month, 1);
                var endDate = startDate.AddMonths(1);


                var moveOrderTask = await MoveOrderTransactions(startDate, endDate);
                var receiptTask = await ReceiptTransactions(startDate, endDate);
                var issueTask = await IssueTransactions(startDate, endDate);
                var fuelTask = await FuelTransactions(startDate, endDate);

                 //Task.WhenAll(moveOrderTask, receiptTask, issueTask, borrowedTask, returnedTask, fuelTask);


                var consolidateList = moveOrderTask.Concat(receiptTask).Concat(issueTask)
                    .Concat(fuelTask);

                var result =  consolidateList.SelectMany(x => new List<ETDGLResult>
                {
                    
                    //debit
                    new ETDGLResult
                    {
                        SyncId = "ETD-" + (x.SyncId ?? string.Empty) + "-D",
                        Mark1 = string.Empty,
                        Mark2 = string.Empty,
                        AssetCIP = x.AssetCIP ?? string.Empty,
                        AccountingTag = string.Empty,
                        TransactionDate = x.TransactionDate,
                        ClientSupplier = x.ClientSupplier ?? string.Empty,
                        AccountTitleCode = x.CheckingRemarks == "Miscellaneous Receipt" ? "115998" : x.AccountTitleCode ?? string.Empty,
                        AccountTitle = x.CheckingRemarks == "Miscellaneous Receipt" ? "Materials & Supplies Inventory" : x.AccountTitle ?? string.Empty,
                        CompanyCode = x.CompanyCode ?? string.Empty,
                        Company = x.Company ?? string.Empty,
                        DivisionCode = x.DivisionCode ?? string.Empty,
                        Division = x.Division ?? string.Empty,
                        DepartmentCode = x.CheckingRemarks == "Miscellaneous Receipt" ? "0703" : x.DepartmentCode ?? string.Empty,
                        Department = x.CheckingRemarks == "Miscellaneous Receipt" ? "Engineering Services & Warehousing" : x.Department ?? string.Empty,
                        UnitCode = x.UnitCode ?? string.Empty,
                        Unit = x.Unit ?? string.Empty,
                        SubUnitCode = x.SubUnitCode ?? string.Empty,
                        SubUnit = x.SubUnit ?? string.Empty,
                        LocationCode = x.LocationCode ?? string.Empty,
                        Location = x.Location ?? string.Empty,
                        PONumber = x.PONumber ?? string.Empty,
                        RRNumber = x.RRNumber,
                        ReferenceNo = x.ReferenceNo ?? string.Empty,
                        ItemCode = x.ItemCode ?? string.Empty,
                        ItemDescription = x.ItemDescription ?? string.Empty,
                        Quantity = x?.Quantity ?? 0,
                        UOM = x.UOM ?? string.Empty,
                        UnitPrice = x?.UnitPrice ?? 0,
                        LineAmount = x?.LineAmount ?? 0,
                        VoucherJournal = string.Empty,
                        AccountType = x.AccountType ?? string.Empty,
                        DRCR = "Debit",
                        AssetCode = string.Empty,
                        Asset= string.Empty,
                        ServiceProviderCode = x.ServiceProviderCode ?? string.Empty,
                        ServiceProvider = x.ServiceProvider ?? string.Empty,
                        BOA = "Inventoriable",
                        Allocation = string.Empty,
                        AccountGroup = x.AccountGroup ?? string.Empty,
                        AccountSubGroup = x.AccountSubGroup ?? string.Empty,
                        FinancialStatement = "Balance Sheet",
                        UnitResponsible = "MAU",
                        Batch = x.Reason ?? string.Empty,
                        Remarks = x.Remarks ?? string.Empty,
                        PayrollPeriod = string.Empty,
                        Position = x.Position ?? string.Empty,
                        PayrollType = string.Empty,
                        PayrollType2 = string.Empty,
                        DepreciationDescription = string.Empty,
                        RemainingDepreciationValue = string.Empty,
                        UsefulLife = string.Empty,
                        Month = x.TransactionDate.Value.ToString("MMM") ?? string.Empty,
                        Year = x.TransactionDate.Value.ToString("yyyy") ?? string.Empty,
                        Particulars = string.Empty,
                        Month2 = x.TransactionDate.Value.ToString("yyyyMM") ?? string.Empty,
                        FarmType = string.Empty,
                        Adjustment = string.Empty,
                        From = string.Empty,
                        ChangeTo = string.Empty,
                        Reason = string.Empty,
                        CheckingRemarks = x.CheckingRemarks ?? string.Empty,
                        BankName = string.Empty,
                        ChequeNumber = string.Empty,
                        ChequeVoucherNumber = string.Empty,
                        ReleasedDate = string.Empty,
                        ChequeDate = string.Empty,
                        BOA2 = "Inventoriable",
                        System = "Elixir ETD",
                        Books = "Journal Book",
                        ChargingCode = x.ChargingCode ?? string.Empty,
                        ChargingName = x.ChargingName ?? string.Empty,
                    },
                    //credit
                    new ETDGLResult
                    {
                        SyncId = "ETD-" + (x.SyncId ?? string.Empty) + "-C",
                        Mark1 = string.Empty,
                        Mark2 = string.Empty,
                        AssetCIP = x.AssetCIP ?? string.Empty,
                        AccountingTag = string.Empty,
                        TransactionDate = x.TransactionDate,
                        ClientSupplier = x.ClientSupplier ?? string.Empty,
                           AccountTitleCode = x.CheckingRemarks == "Miscellaneous Receipt" ? x.AccountTitleCode ?? string.Empty :  "115998",
                        AccountTitle = x.CheckingRemarks == "Miscellaneous Receipt" ? x.AccountTitle ?? string.Empty : "Materials & Supplies Inventory",
                        CompanyCode = x.CompanyCode ?? string.Empty,
                        Company = x.Company ?? string.Empty,
                        DivisionCode = x.DivisionCode ?? string.Empty,
                        Division = x.Division ?? string.Empty,
                        DepartmentCode = x.CheckingRemarks == "Miscellaneous Issue" ? "0703" : x.DepartmentCode ?? string.Empty,
                        Department = x.CheckingRemarks == "Miscellaneous Issue" ? "Engineering Services & Warehousing" : x.Department ?? string.Empty,
                        UnitCode = x.UnitCode ?? string.Empty,
                        Unit = x.Unit ?? string.Empty,
                        SubUnitCode = x.SubUnitCode ?? string.Empty,
                        SubUnit = x.SubUnit ?? string.Empty,
                        LocationCode = x.LocationCode ?? string.Empty,
                        Location = x.Location ?? string.Empty,
                        PONumber = x.PONumber ?? string.Empty,
                        RRNumber = x.RRNumber,
                        ReferenceNo = x.ReferenceNo ?? string.Empty,
                        ItemCode = x.ItemCode ?? string.Empty,
                        ItemDescription = x.ItemDescription ?? string.Empty,
                        Quantity = x?.Quantity ?? 0,
                        UOM = x.UOM ?? string.Empty,
                        UnitPrice = x?.UnitPrice ?? 0,
                        LineAmount = -(x?.LineAmount) ?? 0,
                        VoucherJournal = string.Empty,
                        AccountType = x.AccountType ?? string.Empty,
                        DRCR = "Credit",  
                        AssetCode = x.AssetCode ?? string.Empty,
                        Asset= string.Empty,
                        ServiceProviderCode = x.ServiceProviderCode ?? string.Empty,
                        ServiceProvider = x.ServiceProvider ?? string.Empty,
                        BOA = "Inventoriable",
                        Allocation = string.Empty,
                        AccountGroup = x.AccountGroup ?? string.Empty,
                        AccountSubGroup = x.AccountSubGroup ?? string.Empty,
                        FinancialStatement = "Balance Sheet",
                        UnitResponsible = "MAU",
                        Batch = x.Reason ?? string.Empty,
                        Remarks = x.Remarks ?? string.Empty,
                        PayrollPeriod = string.Empty,
                        Position = string.Empty,
                        PayrollType = string.Empty,
                        PayrollType2 =  string.Empty,
                        DepreciationDescription =  string.Empty,
                        RemainingDepreciationValue =  string.Empty,
                        UsefulLife =  string.Empty,
                        Month = x.TransactionDate.Value.ToString("MMM") ?? string.Empty,
                        Year = x.TransactionDate.Value.ToString("yyyy") ?? string.Empty,
                        Particulars = string.Empty,
                        Month2 = x.TransactionDate.Value.ToString("yyyyMM") ?? string.Empty,
                        FarmType =  string.Empty,
                        Adjustment =  string.Empty,
                        From =  string.Empty,
                        ChangeTo =  string.Empty,
                        Reason = string.Empty,
                        CheckingRemarks = x.CheckingRemarks ?? string.Empty,
                        BankName =  string.Empty,
                        ChequeNumber =  string.Empty,
                        ChequeVoucherNumber =  string.Empty,
                        ReleasedDate = string.Empty,
                        ChequeDate = string.Empty,
                        BOA2 = "Inventoriable",
                        System = "Elixir ETD",
                        Books = "Journal Book",
                        ChargingCode = x.ChargingCode ?? string.Empty,
                        ChargingName = x.ChargingName ?? string.Empty,
                    }
                }).ToList();

                return Result.Success(result);
            }

            private async Task<List<ETDGLResult>> MoveOrderTransactions(DateTime startDate, DateTime endDate)
            {


                var result =  (from m in _context.MoveOrders
                              join t in _context.TransactOrder on m.OrderNo equals t.OrderNo
                              join w in _context.WarehouseReceived on m.WarehouseId equals w.Id
                              join u in _context.Users on t.PreparedBy equals u.FullName
                               join title in _context.OneAccountTitles on m.AccountCode equals title.AccountCode into titleGroup
                               from title in titleGroup.DefaultIfEmpty()
                               where t.PreparedDate >= startDate && t.PreparedDate <= endDate && m.IsTransact == true && m.IsActive == true 
                               select new ETDGLResult
                              {
                                  //SyncId = "MO-" + m.Id.ToString() ,
                                  SyncId = m.Id.ToString(),
                                  TransactionDate = t.PreparedDate.Value.Date,
                                  ClientSupplier = m.CustomerName,
                                  PONumber = m.Category,
                                  RRNumber = m.HelpdeskNo,
                                  ItemCode = w.ItemCode,
                                  ItemDescription = w.ItemDescription,
                                  Quantity = m.QuantityOrdered,
                                  UnitPrice = w.UnitPrice,
                                  LineAmount = (w.UnitPrice * m.QuantityOrdered),
                                  UOM = w.Uom,
                                  CheckingRemarks = "Move Order",
                                  Reason = m.ItemRemarks,
                                  DivisionCode = m.business_unit_code,
                                  Division = m.business_unit_name,
                                  LocationCode = m.LocationCode,
                                  Location = m.LocationName,
                                  AccountTitle = m.AccountTitles != null ? m.AccountTitles : "SE - R & M - Transport Vehicles",
                                  AccountTitleCode = m.AccountCode != null ? m.AccountCode : "537620",
                                  DepartmentCode = m.DepartmentCode,
                                  Department = m.DepartmentName,
                                  AssetCIP = m.Cip_No,
                                  Batch = "",
                                  ServiceProvider = t.PreparedBy,
                                  ServiceProviderCode = u.EmpId,
                                  ReferenceNo = m.EmpId != null ? (m.OrderNo.ToString() ?? "") + (m.EmpId ?? "") : m.OrderNo.ToString(),
                                  Remarks = m.ItemRemarks,
                                  Company = m.CompanyName,
                                  CompanyCode = m.CompanyCode,
                                  Unit = m.department_unit_name,
                                  UnitCode = m.department_unit_code,
                                  SubUnit = m.sub_unit_name,
                                  SubUnitCode = m.sub_unit_code,
                                  ChargingCode = m.One_Charging,
                                  ChargingName = m.one_charging_name,
                                  AccountType = title.AccountType,
                                  AccountGroup = title.AccountGroup,
                                  AccountSubGroup = title.AccountSubgroup,
                                  FinancialStatement = title.FinancialStatement,
                                  UnitResponsible = title.Unit,





                               });

                return await result.ToListAsync();
            }
            private async Task<List<ETDGLResult>> ReceiptTransactions(DateTime startDate, DateTime endDate)
            {

                var materials = _context.Materials
                .AsNoTracking()
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory).Where(x => x.IsActive == true);

                var result = _context.MiscellaneousReceipts
                .AsNoTracking()
                .GroupJoin(_context.WarehouseReceived, receipt => receipt.Id, warehouse => warehouse.MiscellaneousReceiptId, (receipt, warehouse) => new { receipt, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.receipt, warehouse }).Join(_context.Users,x => x.receipt.PreparedBy, user => user.FullName,       
            (x, user) => new { x.receipt, x.warehouse, user })
                .GroupJoin(_context.OneAccountTitles, user => user.warehouse.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.receipt, x.user.warehouse, x.user.user, accountTitle })
                .Join(materials, account => account.warehouse.ItemCode, material => material.ItemCode, (x, material) => new {x.receipt, x.warehouse,x.user, x.accountTitle, material })
                .Where(x => x.warehouse.IsActive == true && x.warehouse.TransactionType == "MiscellaneousReceipt"
                && x.receipt.TransactionDate >= startDate && x.receipt.TransactionDate <= endDate)
                .Select(x => new ETDGLResult
                {
                    SyncId = "MR-" + x.warehouse.Id.ToString(),
                    TransactionDate = x.receipt.TransactionDate,
                    ItemCode = x.warehouse.ItemCode,
                    ItemDescription = x.warehouse.ItemDescription,
                    UOM = x.warehouse.Uom,
                    PONumber = x.material.ItemCategory.ItemCategoryName,
                    Quantity = x.warehouse.ActualGood,
                    UnitPrice = x.warehouse.UnitPrice,
                    LineAmount = Math.Round(x.warehouse.UnitPrice * x.warehouse.ActualGood, 2),
                    CheckingRemarks = "Miscellaneous Receipt",
                    Reason = x.receipt.Remarks,
                    Remarks = x.receipt.Details,
                    ReferenceNo = x.receipt.Id.ToString(),
                    DivisionCode = x.receipt.business_unit_code,
                    Division = x.receipt.business_unit_name,
                    DepartmentCode = x.receipt.DepartmentCode,
                    Department = x.receipt.DepartmentName,
                    LocationCode = x.receipt.LocationCode,
                    Location = x.receipt.LocationName,
                    AccountTitleCode = x.warehouse.AccountCode,
                    AccountTitle = x.warehouse.AccountTitles,
                    ServiceProvider = x.user.FullName,
                    ServiceProviderCode = x.user.EmpId,
                    RRNumber = 0.ToString(),
                    AssetCIP = "",
                    Company = x.receipt.CompanyName,
                    CompanyCode = x.receipt.CompanyCode,
                    Unit = x.receipt.department_unit_name,
                    UnitCode = x.receipt.department_unit_code,
                    SubUnit = x.receipt.sub_unit_name,
                    SubUnitCode = x.receipt.sub_unit_code,
                    ChargingCode = x.receipt.OneChargingCode,
                    ChargingName = x.receipt.one_charging_name,
                    AccountType = x.accountTitle.AccountType,
                    AccountGroup = x.accountTitle.AccountGroup,
                    AccountSubGroup = x.accountTitle.AccountSubgroup,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,

                });

                return await result.ToListAsync();
            }
            private async Task<List<ETDGLResult>> IssueTransactions(DateTime startDate, DateTime endDate)
            {

                var materials = _context.Materials
                .AsNoTracking()
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory).Where(x => x.IsActive == true);

                var result =  _context.MiscellaneousIssues
                .AsNoTracking()
                .Join( _context.MiscellaneousIssueDetail, miscDatail => miscDatail.Id, issue => issue.IssuePKey,
                (miscDetail, issue) => new { miscDetail, issue }).Join(_context.Users, x => x.miscDetail.PreparedBy,  user => user.FullName,   
            (x, user) => new { x.miscDetail, x.issue, user })
                .GroupJoin(_context.OneAccountTitles, user => user.issue.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.miscDetail, x.user.issue, x.user.user, accountTitle })
                .Join(materials, account => account.issue.ItemCode, material => material.ItemCode, (x, material) => new { x.miscDetail, x.issue, x.user, x.accountTitle, material })
                .Where(x => x.issue.IsActive == true && x.miscDetail.TransactionDate >= startDate && x.miscDetail.TransactionDate <= endDate)
                .Select(x => new ETDGLResult
                {
                    SyncId = "MI-" + x.issue.Id.ToString(),
                    TransactionDate = x.miscDetail.TransactionDate.Date,
                    ItemCode = x.issue.ItemCode,
                    ItemDescription = x.issue.ItemDescription,
                    UOM = x.issue.Uom,
                    PONumber = x.material.ItemCategory.ItemCategoryName,
                    Quantity = Math.Round(x.issue.Quantity, 2),
                    UnitPrice = x.issue.UnitPrice,
                    LineAmount = Math.Round(x.issue.UnitPrice * x.issue.Quantity, 2),

                    CheckingRemarks = "Miscellaneous Issue",
                    Reason = x.issue.Remarks,
                    ReferenceNo = x.issue.Id.ToString(),
                    DivisionCode = x.miscDetail.business_unit_code,
                    Division = x.miscDetail.business_unit_name,
                    DepartmentCode = x.miscDetail.DepartmentCode,
                    Department = x.miscDetail.DepartmentName,
                    LocationCode = x.miscDetail.LocationCode,
                    Location = x.miscDetail.LocationName,
                    AccountTitleCode = x.issue.AccountCode,
                    AccountTitle = x.issue.AccountTitles,
                    ServiceProvider = x.user.FullName,
                    ServiceProviderCode = x.user.EmpId,
                    AssetCIP = "",
                    RRNumber = 0.ToString(),
                    Company = x.miscDetail.CompanyName,
                    CompanyCode = x.miscDetail.CompanyCode,
                    Unit = x.miscDetail.department_unit_name,
                    UnitCode = x.miscDetail.department_unit_code,
                    SubUnit = x.miscDetail.sub_unit_name,
                    SubUnitCode = x.miscDetail.sub_unit_code,
                    ChargingCode = x.miscDetail.OneChargingCode,
                    ChargingName = x.miscDetail.one_charging_name,
                    AccountType = x.accountTitle.AccountType,
                    AccountGroup = x.accountTitle.AccountGroup,
                    AccountSubGroup = x.accountTitle.AccountSubgroup,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,

                });

                return await result.ToListAsync();
            }
            //private async Task<List<ETDGLResult>> BorrowedTransactions(DateTime startDate, DateTime endDate)
            //{
            //    var result =  _context.BorrowedIssues
            //        .AsNoTracking()
            //        .GroupJoin(_context.BorrowedIssueDetails, borrow => borrow.Id, borrowDetail => borrowDetail.BorrowedPKey,
            //        (borrow, borrowDetail) => new { borrow, borrowDetail })
            //        .SelectMany(x => x.borrowDetail.DefaultIfEmpty(), (x, borrowDetail) => new {x.borrow, borrowDetail })
            //        .GroupJoin(_context.Users, x => x.borrow.PreparedBy, user => user.FullName, (x, user) => new { x.borrow, x.borrowDetail, user })
            //        .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new {x.borrow, x.borrowDetail, user })
            //        .Where(x => x.borrowDetail.IsActive == true && x.borrowDetail.PreparedDate >= startDate && x.borrowDetail.PreparedDate <= endDate && x.borrow.IsReturned == false)
            //        .Select(x => new ETDGLResult
            //        {
            //            SyncId = x.borrowDetail.Id.ToString(),
            //            TransactionDate = x.borrowDetail.PreparedDate.Date,
            //            ItemCode = x.borrowDetail.ItemCode,
            //            ItemDescription = x.borrowDetail.ItemDescription,
            //            UOM = x.borrowDetail.Uom,
            //            PONumber = "",
            //            Quantity = Math.Round(x.borrowDetail.Quantity, 2),
            //            UnitPrice = x.borrowDetail.UnitPrice,
            //            LineAmount = Math.Round(x.borrowDetail.UnitPrice * x.borrowDetail.Quantity, 2),

            //            CheckingRemarks = "Borrow",
            //            Reason = x.borrow.Remarks,
            //            Remarks = x.borrow.Details,

            //            DivisionCode = "",
            //            Division = "",
            //            DepartmentCode = "",
            //            Department = "",
            //            LocationCode = "",
            //            Location = "",
            //            AccountTitleCode = "",
            //            AccountTitle = "",
            //            ServiceProvider = x.user.FullName,
            //            ServiceProviderCode = x.user.EmpId,
            //            AssetCIP = "",
            //            RRNumber = 0.ToString(),
            //            //Remarks = x.borrow.Remarks,



            //        });
            //    return await result.ToListAsync();
            //}
            //private async Task<List<ETDGLResult>> ReturnedTransactions(DateTime startDate, DateTime endDate)
            //{
            //    var consumeList = _context.BorrowedConsumes
            //        .Where(x => x.IsActive == true )
            //        .Select(x => new BorrowedConsolidatedDto
            //        {
            //            Id = x.Id,
            //            BorrowedId = x.BorrowedItemPkey,
            //            ItemCode = x.ItemCode,
            //            ItemDescription = x.ItemDescription,
            //            Uom = x.Uom,
            //            Consumed = x.Consume,
            //            CompanyCode = x.CompanyCode,
            //            CompanyName = x.CompanyName,
            //            DepartmentCode = x.DepartmentCode,
            //            DepartmentName = x.DepartmentName,
            //            LocationCode = x.LocationCode,
            //            LocationName = x.LocationName,
            //            AccountCode = x.AccountCode,
            //            AccountTitles = x.AccountTitles,
            //            EmpId = x.EmpId,
            //            FullName = x.FullName,
            //            ReportNumber = x.ReportNumber,

            //        });

            //    var returnList = _context.BorrowedIssueDetails
            //        .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
            //        .GroupJoin(consumeList, borrowDetails => borrowDetails.Id, consume => consume.BorrowedId
            //        , (borrowDetails, consume) => new { borrowDetails, consume })
            //        .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrowDetails, consume })
            //        .Select(x => new BorrowedConsolidatedDto
            //        {
            //            Id = x.borrowDetails.Id,
            //            BorrowedId = x.borrowDetails.BorrowedPKey,
            //            ItemCode = x.borrowDetails.ItemCode,
            //            ItemDescription = x.borrowDetails.ItemDescription,
            //            Uom = x.borrowDetails.Uom,
            //            BorrowedQuantity = x.borrowDetails.Quantity != null ? x.borrowDetails.Quantity : 0,
            //            Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
            //            CompanyCode = x.consume.CompanyCode,
            //            CompanyName = x.consume.CompanyName,
            //            DepartmentCode = x.consume.DepartmentCode,
            //            DepartmentName = x.consume.DepartmentName,
            //            LocationCode = x.consume.LocationCode,
            //            LocationName = x.consume.LocationName,
            //            AccountCode = x.consume.AccountCode,
            //            AccountTitles = x.consume.AccountTitles,
            //            EmpId = x.consume.EmpId,
            //            FullName = x.consume.FullName,
            //            ReportNumber = x.consume.ReportNumber,
            //            UnitPrice = x.borrowDetails.UnitPrice

            //        });

            //    var borrowedIssueList = _context.BorrowedIssues
            //        .AsNoTracking()
            //        .Where(x => x.IsActive == true && x.IsReturned == true);

            //    var result =  returnList
            //        .GroupJoin(borrowedIssueList, borrowDetail => borrowDetail.BorrowedId, borrow => borrow.Id,
            //        (borrowDetail, borrow) => new { borrowDetail, borrow })
            //        .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.borrowDetail, borrow }).Where(x => x.borrow.PreparedDate >= startDate && x.borrow.PreparedDate <= endDate)
            //        .Select(x => new ETDGLResult
            //        {

            //            SyncId = x.borrowDetail.Id.ToString(),
            //            TransactionDate = x.borrow.PreparedDate.Date,
            //            ItemCode = x.borrowDetail.ItemCode,
            //            ItemDescription = x.borrowDetail.ItemDescription,
            //            UOM = x.borrowDetail.Uom,
            //            PONumber = "",
            //            Quantity = x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed,
            //            UnitPrice = x.borrowDetail.UnitPrice,
            //            LineAmount = Math.Round(x.borrowDetail.UnitPrice.Value * x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed, 2),

            //            CheckingRemarks = "Returned",
            //            Reason = "",
            //            Remarks = "",

            //            DivisionCode = x.borrowDetail.CompanyCode,
            //            Division = x.borrowDetail.CompanyName,
            //            DepartmentCode = x.borrowDetail.DepartmentCode,
            //            Department = x.borrowDetail.DepartmentName,
            //            LocationCode = x.borrowDetail.LocationCode,
            //            Location = x.borrowDetail.LocationName,
            //            AccountTitleCode = x.borrowDetail.AccountCode,
            //            AccountTitle = x.borrowDetail.AccountTitles,
            //            ServiceProvider = x.borrowDetail.FullName,
            //            ServiceProviderCode = x.borrowDetail.EmpId,
            //            AssetCIP = "",
            //            RRNumber = 0.ToString(),

            //        });
            //    return await result.ToListAsync();
            //}
            private async Task<List<ETDGLResult>> FuelTransactions(DateTime startDate, DateTime endDate)
            {
                var result =  _context.FuelRegisterDetails
                    .Include(m => m.Material)
                    .ThenInclude(id => id.ItemCategory)
                    .Include(w => w.Warehouse_Receiving)
                    .GroupJoin(_context.OneAccountTitles, fuel => fuel.FuelRegister.Account_Title_Code, accountTitle => accountTitle.AccountCode, (fuel, accountTitle) => new { fuel, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.fuel, accountTitle })
                    .Where(r => r.fuel.FuelRegister.Is_Transact == true && r.fuel.FuelRegister.Transact_At >= startDate && r.fuel.FuelRegister.Transact_At <= endDate)

                    .Select(x => new ETDGLResult
                    {

                        SyncId = "R-" + x.fuel.Id.ToString(),
                        TransactionDate = x.fuel.FuelRegister.Transact_At.Value.Date,
                        ItemCode = x.fuel.Material.ItemCode,
                        ItemDescription = x.fuel.Material.ItemDescription,
                        UOM = x.fuel.Material.Uom.UomCode,
                        PONumber = "",
                        Quantity = x.fuel.Liters != null ? x.fuel.Liters : 0,
                        UnitPrice = x.fuel.Warehouse_Receiving.UnitPrice,
                        LineAmount = Math.Round(x.fuel.Warehouse_Receiving.UnitPrice * x.fuel.Liters.Value, 2),

                        CheckingRemarks = "Fuel",
                        Reason = x.fuel.FuelRegister.Remarks,
                        Remarks = "",

                        DivisionCode = x.fuel.FuelRegister.BusinessUnitCode,
                        Division = x.fuel.FuelRegister.BusinessUnitName,
                        DepartmentCode = x.fuel.FuelRegister.Department_Code,
                        Department = x.fuel.FuelRegister.Department_Name,
                        LocationCode = x.fuel.FuelRegister.Location_Code,
                        Location = x.fuel.FuelRegister.Location_Name,
                        AccountTitleCode = x.fuel.FuelRegister.Account_Title_Code,
                        AccountTitle = x.fuel.FuelRegister.Account_Title_Code,

                        AssetCIP = "",
                        RRNumber = 0.ToString(),
                        Company = x.fuel.FuelRegister.Company_Name,
                        CompanyCode = x.fuel.FuelRegister.Company_Code,
                        Unit = x.fuel.FuelRegister.DepartmentUnitName,
                        UnitCode = x.fuel.FuelRegister.DepartmentUnitCode,
                        SubUnit = x.fuel.FuelRegister.SubUnitName,
                        SubUnitCode = x.fuel.FuelRegister.SubUnitCode,
                        ChargingCode = x.fuel.FuelRegister.OneChargingCode,
                        ChargingName = x.fuel.FuelRegister.OneChargingName,
                        AccountType = x.accountTitle.AccountType,
                        AccountGroup = x.accountTitle.AccountGroup,
                        AccountSubGroup = x.accountTitle.AccountSubgroup,
                        FinancialStatement = x.accountTitle.FinancialStatement,
                        UnitResponsible = x.accountTitle.Unit,

                    });
                return await result.ToListAsync();
            }

            
        }
    }
}
