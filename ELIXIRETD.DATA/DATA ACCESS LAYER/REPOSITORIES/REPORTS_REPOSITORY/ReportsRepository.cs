using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.Migrations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class ReportsRepository : IReports
    {
        private readonly StoreContext _context;
        public ReportsRepository(StoreContext storeContext)
        {
            _context = storeContext;
        }

        //public async Task<PagedList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(UserParams userParams, string DateFrom, string DateTo , string Search)
        //{

        //    var warehouse = _context.WarehouseReceived.Where(x => x.ReceivingDate.Date >= DateTime.Parse(DateFrom).Date && x.ReceivingDate.Date <= DateTime.Parse(DateTo).Date)
        //                                              .Where(x => x.IsActive == true)
        //                                              .Where(x => x.TransactionType == "Receiving")
        //                                              .Select(x => new DtoWarehouseReceivingReports
        //                                              {
        //                                                  WarehouseId = x.Id,
        //                                                  PoNumber = x.PoNumber,
        //                                                  PR_Year_Number = x.PR_Year_Number,
        //                                                  RRNumber = x.RRNo,
        //                                                  RRDate = x.RRDate.Value.Date,
        //                                                  ReceiveDate = x.ReceivingDate.ToString(),
        //                                                  ItemCode = x.ItemCode,
        //                                                  ItemDescrption = x.ItemDescription,
        //                                                  Uom = x.Uom,
        //                                                  Category = x.LotSection,
        //                                                  Quantity = x.ActualDelivered,
        //                                                  TotalReject = x.TotalReject,
        //                                                  SupplierName = x.Supplier,
        //                                                  TransactionType = x.TransactionType,
        //                                                  ReceivedBy = x.AddedBy,
        //                                                  UnitPrice = x.UnitPrice,
        //                                                  Amount = x.UnitPrice * x.ActualDelivered,
        //                                                  SINumber = x.SINumber

        //                                              });

        //    if(!string.IsNullOrEmpty(Search))
        //    {
        //        warehouse = warehouse.Where(x => Convert.ToString(x.PoNumber).ToLower().Contains(Search.Trim().ToLower())
        //        || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
        //        || Convert.ToString(x.TransactionType).ToLower().Contains(Search.Trim().ToLower())
        //        || Convert.ToString(x.ItemDescrption).ToLower().Contains(Search.Trim().ToLower()));
        //    }


        //    if(userParams.UsePagination)
        //    {
        //        return await PagedList<DtoWarehouseReceivingReports>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
        //    }
        //    else
        //    {

        //    }


        //}

        public async Task<PagedList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var warehouse = _context.WarehouseReceived
                .Where(x => x.ReceivingDate.Date >= DateTime.Parse(DateFrom).Date && x.ReceivingDate.Date <= DateTime.Parse(DateTo).Date)
                .Where(x => x.IsActive)
                .Where(x => x.TransactionType == "Receiving")
                .Select(x => new DtoWarehouseReceivingReports
                {
                    WarehouseId = x.Id,
                    PoNumber = x.PoNumber,
                    PR_Year_Number = x.PR_Year_Number,
                    RRNumber = x.RRNo,
                    RRDate = x.RRDate.Value.Date,
                    ReceiveDate = x.ReceivingDate.ToString(),
                    ActualReceivingDate = x.ActualReceivingDate.ToString(),
                    ItemCode = x.ItemCode,
                    ItemDescrption = x.ItemDescription,
                    Uom = x.Uom,
                    Category = x.LotSection,
                    Quantity = x.ActualDelivered,
                    TotalReject = x.TotalReject,
                    SupplierName = x.Supplier,
                    TransactionType = x.TransactionType,
                    ReceivedBy = x.AddedBy,
                    UnitPrice = (x.PriceWithDecimal == null ? 0m : decimal.Parse(x.PriceWithDecimal)).ToString(),
                    Amount = (x.PriceWithDecimal == null ? 0m : decimal.Parse(x.PriceWithDecimal) * x.ActualDelivered).ToString(),
                    SINumber = x.SINumber
                });

            if (!string.IsNullOrEmpty(Search))
            {
                string searchLower = Search.Trim().ToLower();
                warehouse = warehouse.Where(x =>
                    x.PoNumber.ToLower().Contains(searchLower) ||
                    x.ItemCode.ToLower().Contains(searchLower) ||
                    x.TransactionType.ToLower().Contains(searchLower) ||
                    x.ItemDescrption.ToLower().Contains(searchLower));
            }

            if (userParams.UsePagination)
            {
                return await PagedList<DtoWarehouseReceivingReports>.CreateAsync(warehouse, userParams.PageNumber, userParams.PageSize);
            }
            else
            {
                // ✅ Wrap non-paginated results in PagedList to match return type
                var allResults = await warehouse.ToListAsync();
                return new PagedList<DtoWarehouseReceivingReports>(allResults, allResults.Count, 1, allResults.Count);
            }
        }


        public async Task<PagedList<DtoForTransactedReports>> WarehouseMoveOrderReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var orders = _context.MoveOrders
                        .Where(moveorder => moveorder.PreparedDate.Value.Date >= DateTime.Parse(DateFrom).Date && moveorder.PreparedDate.Value.Date <= DateTime.Parse(DateTo).Date && moveorder.IsActive == true && moveorder.IsPrepared == true && moveorder.IsTransact == false)
                        .GroupJoin(_context.TransactOrder, moveorder => moveorder.OrderNo, transact => transact.OrderNo, (moveorder, transact) => new { moveorder, transact })
                        .SelectMany(x => x.transact.DefaultIfEmpty(), (x, transact) => new { x.moveorder, transact })
                         .Select(x => new DtoForTransactedReports
                         {

                             MIRId = x.moveorder.OrderNo,
                             Requestor = x.moveorder.Requestor,
                             Approver = x.moveorder.Approver,
                             CustomerCode = x.moveorder.Customercode,
                             CustomerName = x.moveorder.CustomerName,
                             ItemCode = x.moveorder.ItemCode,
                             ItemDescription = x.moveorder.ItemDescription,
                             Uom = x.moveorder.Uom,
                             Category = x.moveorder.Category,
                             Quantity = x.moveorder.QuantityOrdered,
                             MoveOrderBy = x.moveorder.PreparedBy,
                             MoveOrderDate = x.moveorder.PreparedDate.ToString(),
                             TransactedDate = x.transact.PreparedDate.ToString(),
                             OrderDate = x.moveorder.OrderDate.ToString(),
                             DateNeeded = x.moveorder.DateNeeded.ToString(),
                             IsActive = x.moveorder.IsActive,
                             CompanyCode = x.moveorder.CompanyCode,
                             CompanyName = x.moveorder.CompanyName,
                             DepartmentCode = x.moveorder.DepartmentCode,
                             DepartmentName = x.moveorder.DepartmentName,
                             LocationCode = x.moveorder.LocationCode,
                             LocationName = x.moveorder.LocationName,
                             AccountCode = x.moveorder.AccountCode,
                             AccountTitles = x.moveorder.AccountTitles,
                             Empid = x.moveorder.EmpId,
                             FullName = x.moveorder.FullName,
                             CustomerType = x.moveorder.CustomerType,
                             ItemRemarks = x.moveorder.ItemRemarks,
                             UnitCost = x.moveorder.UnitPrice,
                             LineAmount = x.moveorder.UnitPrice * x.moveorder.QuantityOrdered,
                             Cip_No = x.moveorder.Cip_No,
                             AssetTag = x.moveorder.AssetTag,
                             HelpdeskNo = x.moveorder.HelpdeskNo,
                             DateApproved = x.moveorder.DateApproved.ToString(),
                             Rush = x.moveorder.Rush,
                             Remarks = x.moveorder.Remarks,
                             OneChargingName = x.moveorder.one_charging_name,
                             BusinessUnitCode = x.moveorder.business_unit_code,
                             BusinessUnitName = x.moveorder.business_unit_name,
                             DepartmentUnitCode = x.moveorder.department_unit_code,
                             DepartmentUnitName = x.moveorder.department_unit_name,
                             SubUnitCode = x.moveorder.sub_unit_code,
                             SubUnitName = x.moveorder.sub_unit_name,

                         });

            if (!string.IsNullOrEmpty(Search))
            {
                orders = orders.Where(x => Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoForTransactedReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoTransactReports>> TransactedMoveOrderReport(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var dateFrom = DateTime.Parse(DateFrom).Date;
            var dateTo = DateTime.Parse(DateTo).Date;

            var materials = _context.Materials
                .Include(x => x.Uom).Where(x => x.IsActive == true)
                .Select(x => new
                {
                    x.ItemCode,
                    x.ItemDescription,
                    x.Uom.UomCode
                });

            var moveOrderConsol = _context.MoveOrders
                .AsNoTracking()
                .GroupJoin(_context.TransactOrder, moveOrder => moveOrder.OrderNo, transMoveOrder => transMoveOrder.OrderNo, (moveOrder, transMoveOrder) => new { moveOrder, transMoveOrder })
                .SelectMany(x => x.transMoveOrder.DefaultIfEmpty(), (x, transMoveOrder) => new { x.moveOrder, transMoveOrder })
                .GroupJoin(_context.Users, trans => trans.transMoveOrder.PreparedBy, user => user.FullName, (trans, user) => new { trans, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.trans.moveOrder, x.trans.transMoveOrder, user })
                .GroupJoin(_context.OneAccountTitles, user => user.moveOrder.AccountCode, account => account.AccountCode, (user, account) => new { user, account })
                .SelectMany(x => x.account.DefaultIfEmpty(), (x, account) => new { x.user.moveOrder, x.user.transMoveOrder, x.user.user, account })
                .GroupJoin(materials, account => account.moveOrder.ItemCode, material => material.ItemCode, (account, material) => new { account, material })
                .SelectMany(x => x.material.DefaultIfEmpty(), (x, material) => new { x.account.moveOrder, x.account.transMoveOrder, x.account.user, x.account.account, material })


                .Where(x => x.moveOrder.IsTransact == true)
                .Where(x => x.transMoveOrder.PreparedDate.Value.Date >= dateFrom && x.transMoveOrder.PreparedDate.Value.Date <= dateTo)

                .Select(x => new DtoTransactReports
                {
                    MIRId = x.moveOrder.OrderNo,
                    Id = x.moveOrder.Id,
                    Requestor = x.moveOrder.Requestor,
                    Approver = x.moveOrder.PreparedBy,
                    CustomerName = x.moveOrder.CustomerName,
                    CustomerCode = x.moveOrder.Customercode,
                    ItemCode = x.material.ItemCode,
                    ItemDescription = x.material.ItemDescription,
                    Uom = x.material.UomCode,
                    Quantity = x.moveOrder.QuantityOrdered,
                    MoveOrderDate = x.moveOrder.PreparedDate.ToString(),
                    MoveOrderBy = x.moveOrder.PreparedBy,
                    TransactedBy = x.transMoveOrder.PreparedBy,
                    TransactionType = "Pick-Up",
                    TransactedDate = x.transMoveOrder.PreparedDate.Value.Date,
                    DeliveryDate = x.transMoveOrder.DeliveryDate.ToString(),
                    CustomerType = x.moveOrder.CustomerType,
                    DateNeeded = x.moveOrder.DateNeeded.ToString(),
                    OrderDate = x.moveOrder.OrderDate.ToString(),
                    CompanyCode = x.moveOrder.CompanyCode,
                    CompanyName = x.moveOrder.CompanyName,
                    DepartmentCode = x.moveOrder.DepartmentCode,
                    DepartmentName = x.moveOrder.DepartmentName,
                    LocationCode = x.moveOrder.LocationCode,
                    LocationName = x.moveOrder.LocationName,
                    AccountCode = x.moveOrder.AccountCode,
                    AccountTitles = x.moveOrder.AccountTitles,
                    EmpId = x.moveOrder.EmpId,
                    FullName = x.moveOrder.FullName,
                    ItemRemarks = x.moveOrder.ItemRemarks,
                    UnitCost = x.moveOrder.UnitPrice,
                    LineAmount = x.moveOrder.UnitPrice * x.moveOrder.QuantityOrdered,
                    Cip_No = x.moveOrder.Cip_No,
                    HelpDesk = x.moveOrder.HelpdeskNo,
                    Remarks = x.moveOrder.Remarks,
                    Status = x.moveOrder.IsTransact == true ? "Transacted" : "For Approval",
                    Category = x.moveOrder.Category,
                    AssetTag = x.moveOrder.AssetTag,
                    DateApproved = x.moveOrder.ApprovedDate.ToString(),
                    PicUp_Date = x.transMoveOrder.DeliveryDate.ToString(),
                    Rush = x.moveOrder.Rush,
                    Reference = x.moveOrder.LocationName.Contains("Common") || x.moveOrder.LocationName.Contains("Head Office") || x.moveOrder.LocationName.Contains("Central Depot") ?
                    $" {x.moveOrder.DepartmentName} {x.moveOrder.OrderNo}" : $" {x.moveOrder.LocationName} {x.moveOrder.OrderNo}",
                    OneChargingName = x.moveOrder.one_charging_name,
                    BusinessUnitCode = x.moveOrder.business_unit_code,
                    BusinessUnitName = x.moveOrder.business_unit_name,
                    DepartmentUnitCode = x.moveOrder.department_unit_code,
                    DepartmentUnitName = x.moveOrder.department_unit_name,
                    SubUnitCode = x.moveOrder.sub_unit_code,
                    SubUnitName = x.moveOrder.sub_unit_name,
                    

                });

            if (!string.IsNullOrEmpty(Search))
            {
                moveOrderConsol = moveOrderConsol.Where(x => Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));

            }

            moveOrderConsol = moveOrderConsol
                .OrderBy(x => x.TransactedDate)
                .ThenBy(x => x.ItemCode);


            return await PagedList<DtoTransactReports>.CreateAsync(moveOrderConsol, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<IReadOnlyList<MoveOrderReportsDto>> MoveOrderReport(string DateFrom, string DateTo, string Search)
        {

            var percentage = 100;

            var moveOrderReports = _context.MoveOrders
                                .AsNoTracking()
                 .Where(x => x.IsActive == true && x.IsPrepared == true)
                 .Where(x => x.ApprovedDate.Value.Date >= DateTime.Parse(DateFrom).Date &&
                          x.ApprovedDate.Value.Date <= DateTime.Parse(DateTo).Date)
                .Where(x => x.IsActive == true && x.IsApprove == true)
                .OrderBy(x => x.ApprovedDate)
                .GroupBy(x => new
                {
                    x.OrderNo,
                    x.ItemCode

                })
                .Select(x => new MoveOrderReportsDto
                {
                    MIRId = x.Key.OrderNo,
                    OrderNoPKey = x.First().OrderNoPkey,
                    CustomerCode = x.First().Customercode,
                    CustomerName = x.First().CustomerName,
                    ItemCode = x.Key.ItemCode,
                    ItemDescription = x.First().ItemDescription,
                    ItemRemarks = x.First().ItemRemarks,
                    Status = x.First().IsTransact != true ? "For Transaction" : "Transacted",
                    ApprovedDate = x.First().ApprovedDate.Value.Date.ToString(),
                    ServedOrder = x.Sum(x => x.QuantityOrdered),
                    CompanyCode = x.First().CompanyCode,
                    CompanyName = x.First().CompanyName,
                    DepartmentCode = x.First().DepartmentCode,
                    DepartmentName = x.First().DepartmentName,
                    LocationCode = x.First().LocationCode,
                    LocationName = x.First().LocationName,
                    AccountCode = x.First().AccountCode,
                    AccountTitles = x.First().AccountTitles,
                    EmpId = x.First().EmpId,
                    FullName = x.First().FullName,
                    AssetTag = x.First().AssetTag,
                    Cip_No = x.First().Cip_No,
                    HelpdeskNo = x.First().HelpdeskNo,
                    IsRush = !string.IsNullOrEmpty(x.First().Rush) ? "Yes" : null,
                    OneChargingName = x.First().one_charging_name,
                    BusinessUnitCode = x.First().business_unit_code,
                    BusinessUnitName = x.First().sub_unit_name,
                    DepartmentUnitCode = x.First().department_unit_code,
                    DepartmentUnitName = x.First().department_unit_name,
                    SubUnitCode = x.First().sub_unit_code,
                    SubUnitName = x.First().sub_unit_name,

                });

            var orderReport = _context.Orders.
                                AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                .Select(x => new
                {
                    TrasactId = x.TrasactId,
                    Id = x.Id,
                    ItemCode = x.ItemCode,
                    Uom = x.Uom,
                    StandardQuantity = x.StandartQuantity,
                    OrderNo = x.OrderNo,
                    Remarks = x.Remarks,
                });

            var transactReport = _context.TransactOrder.
                 AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true);


            var getWarehouseStock = _context.WarehouseReceived
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                                                  .GroupBy(x => new
                                                  {
                                                      x.ItemCode,

                                                  }).Select(x => new WarehouseInventory
                                                  {
                                                      ItemCode = x.Key.ItemCode,
                                                      ActualGood = x.Sum(x => x.ActualGood)

                                                  });

            var getMoveOrderOut = _context.MoveOrders
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                                         .Where(x => x.IsPrepared == true)
                                         .GroupBy(x => new
                                         {

                                             x.ItemCode,

                                         }).Select(x => new MoveOrderInventory
                                         {
                                             ItemCode = x.Key.ItemCode,
                                             QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                         });


            var getIssueOut = _context.MiscellaneousIssueDetail
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                                                 .GroupBy(x => new
                                                 {
                                                     x.ItemCode

                                                 }).Select(x => new DtoMiscIssue
                                                 {

                                                     ItemCode = x.Key.ItemCode,
                                                     Quantity = x.Sum(x => x.Quantity)

                                                 });


            var getBorrowedOut = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new DtoBorrowedIssue
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  Quantity = x.Sum(x => x.Quantity)

                                                              });

            var consumed = _context.BorrowedConsumes
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive)
                               .GroupBy(x => new
                               {
                                   x.ItemCode,
                                   x.BorrowedItemPkey

                               }).Select(x => new ItemStocksDto
                               {
                                   ItemCode = x.Key.ItemCode,
                                   BorrowedItemPkey = x.Key.BorrowedItemPkey,
                                   Consume = x.Sum(x => x.Consume != null ? x.Consume : 0)

                               });

            var getReturned = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.IsActive == true)
                                                            .Where(x => x.IsReturned == true)
                                                            .Where(x => x.IsApprovedReturned == true)
                                                            .GroupJoin(consumed, returned => returned.Id, consume => consume.BorrowedItemPkey, (returned, consume) => new { returned, consume })
                                                            .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.returned, consume })
                                                             .GroupBy(x => new
                                                             {

                                                                 x.returned.ItemCode,

                                                             }).Select(x => new DtoBorrowedIssue
                                                             {

                                                                 ItemCode = x.Key.ItemCode,
                                                                 ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

                                                             });

            var getSOH = (from warehouse in getWarehouseStock
                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ1
                          from moveorder in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedOut
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturned
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,
                          }

                          by new
                          {
                              warehouse.ItemCode
                          }

                          into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)

                          });


            var orders = moveOrderReports
                .GroupJoin(orderReport, moveOrder => moveOrder.OrderNoPKey, order => order.Id, (moveOrder, order) => new { moveOrder, order })
                .SelectMany(x => x.order.DefaultIfEmpty(), (x, order) => new { x.moveOrder, order });

            var ordersWithTransact = orders
                .GroupJoin(_context.TransactOrder, moveOrder => moveOrder.moveOrder.MIRId, transact => transact.OrderNo, (moveOrder, transact) => new { moveOrder, transact })
                .SelectMany(x => x.transact.DefaultIfEmpty(), (x, transact) => new { x.moveOrder, transact });

            var ordersWithSOH = ordersWithTransact
                .GroupJoin(getSOH, moveOrder => moveOrder.moveOrder.moveOrder.ItemCode, soh => soh.ItemCode, (moveOrder, soh) => new { moveOrder, soh })
                .SelectMany(x => x.soh.DefaultIfEmpty(), (x, soh) => new { x.moveOrder, soh });

            var reports = ordersWithSOH
                .AsNoTracking()
                .Select(x => new MoveOrderReportsDto
                {

                    MIRId = x.moveOrder.moveOrder.moveOrder.MIRId,
                    CustomerCode = x.moveOrder.moveOrder.moveOrder.CustomerCode,
                    CustomerName = x.moveOrder.moveOrder.moveOrder.CustomerName,
                    ItemCode = x.moveOrder.moveOrder.moveOrder.ItemCode,
                    ItemDescription = x.moveOrder.moveOrder.moveOrder.ItemDescription,
                    Uom = x.moveOrder.moveOrder.order.Uom,
                    ItemRemarks = x.moveOrder.moveOrder.moveOrder.ItemRemarks,
                    Status = x.moveOrder.moveOrder.moveOrder.Status,
                    ApprovedDate = x.moveOrder.moveOrder.moveOrder.ApprovedDate,
                    DeliveryDate = x.moveOrder.transact.DeliveryDate.Value.Date.ToString(),
                    OrderedQuantity = x.moveOrder.moveOrder.order.StandardQuantity,
                    ServedOrder = x.moveOrder.moveOrder.moveOrder.ServedOrder,
                    UnservedOrder = x.moveOrder.moveOrder.order.StandardQuantity - x.moveOrder.moveOrder.moveOrder.ServedOrder,
                    ServedPercentage = (x.moveOrder.moveOrder.moveOrder.ServedOrder / x.moveOrder.moveOrder.order.StandardQuantity),
                    SOH = x.soh.SOH,
                    CompanyCode = x.moveOrder.moveOrder.moveOrder.CompanyCode,
                    CompanyName = x.moveOrder.moveOrder.moveOrder.CompanyName,
                    DepartmentCode = x.moveOrder.moveOrder.moveOrder.DepartmentCode,
                    DepartmentName = x.moveOrder.moveOrder.moveOrder.DepartmentName,
                    LocationCode = x.moveOrder.moveOrder.moveOrder.LocationCode,
                    LocationName = x.moveOrder.moveOrder.moveOrder.LocationName,
                    AccountCode = x.moveOrder.moveOrder.moveOrder.AccountCode,
                    AccountTitles = x.moveOrder.moveOrder.moveOrder.AccountTitles,
                    EmpId = x.moveOrder.moveOrder.moveOrder.EmpId,
                    FullName = x.moveOrder.moveOrder.moveOrder.FullName,
                    AssetTag = x.moveOrder.moveOrder.moveOrder.AssetTag,
                    Cip_No = x.moveOrder.moveOrder.moveOrder.Cip_No,
                    HelpdeskNo = x.moveOrder.moveOrder.moveOrder.HelpdeskNo,
                    IsRush = x.moveOrder.moveOrder.moveOrder.IsRush,
                    Remarks = x.moveOrder.moveOrder.order.Remarks,
                    BusinessUnitName = x.moveOrder.moveOrder.moveOrder.BusinessUnitName,
                    BusinessUnitCode = x.moveOrder.moveOrder.moveOrder.BusinessUnitCode,
                    DepartmentUnitName = x.moveOrder.moveOrder.moveOrder.DepartmentUnitName,
                    DepartmentUnitCode = x.moveOrder.moveOrder.moveOrder.DepartmentUnitCode,
                    SubUnitCode = x.moveOrder.moveOrder.moveOrder.SubUnitCode,
                    SubUnitName = x.moveOrder.moveOrder.moveOrder.SubUnitName,
                });


            if (!string.IsNullOrEmpty(Search))
            {
                reports = reports.Where(x => Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                                          || x.ItemCode.ToLower().Contains(Search.Trim().ToLower())
                                          || x.ItemDescription.ToLower().Contains(Search.Trim().ToLower())
                                          || x.Status.ToLower().Contains(Search.Trim().ToLower()));
            }


            return await reports.ToListAsync();
        }


        public async Task<PagedList<DtoMiscReports>> MiscReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                            join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId
                            into leftJ
                            from receipt in leftJ.DefaultIfEmpty()

                            where receiptHeader.TransactionDate.Date >= DateTime.Parse(DateFrom).Date && receiptHeader.TransactionDate.Date <= DateTime.Parse(DateTo).Date && receipt.IsActive == true && receipt.TransactionType == "MiscellaneousReceipt"

                            select new DtoMiscReports
                            {

                                ReceiptId = receiptHeader.Id,
                                SupplierCode = receiptHeader.SupplierCode,
                                SupplierName = receiptHeader.supplier,
                                Details = receiptHeader.Details,
                                Remarks = receiptHeader.Remarks,
                                ItemCode = receipt.ItemCode,
                                ItemDescription = receipt.ItemDescription,
                                Uom = receipt.Uom,
                                Category = receipt.LotSection,
                                Quantity = receipt.ActualGood,
                                TransactBy = receiptHeader.PreparedBy,
                                TransactDate = receiptHeader.TransactionDate.ToString(),
                                ReceivingDate = receipt.ReceivingDate.ToString(),
                                CompanyCode = receiptHeader.CompanyCode,
                                CompanyName = receiptHeader.CompanyName,
                                DepartmentCode = receiptHeader.DepartmentCode,
                                DepartmentName = receiptHeader.DepartmentName,
                                LocationCode = receiptHeader.LocationCode,
                                LocationName = receiptHeader.LocationName,
                                AccountCode = receipt.AccountCode,
                                AccountTitles = receipt.AccountTitles,
                                EmpId = receipt.EmpId,
                                FullName = receipt.FullName,

                                UnitCost = (receipt.PriceWithDecimal == null ? receipt.UnitPrice : decimal.Parse(receipt.PriceWithDecimal)).ToString(),
                                TotalCost = (receipt.PriceWithDecimal == null ? receipt.UnitPrice * receipt.ActualDelivered : decimal.Parse(receipt.PriceWithDecimal) * receipt.ActualDelivered).ToString(),
                                OneChargingName = receiptHeader.one_charging_name,
                                BusinessUnitCode = receiptHeader.business_unit_code,
                                BusinessUnitName = receiptHeader.business_unit_name,
                                DepartmentUnitCode = receiptHeader.department_unit_code,
                                DepartmentUnitName = receiptHeader.department_unit_name,
                                SubUnitCode = receiptHeader.sub_unit_code,
                                SubUnitName = receiptHeader.sub_unit_name,


                            });

            if (!string.IsNullOrEmpty(Search))
            {
                receipts = receipts.Where(x => Convert.ToString(x.ReceiptId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.SupplierName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoMiscReports>.CreateAsync(receipts, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<DtoMiscIssue>> MiscIssue(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var issues = _context.MiscellaneousIssues
                       .GroupJoin(_context.MiscellaneousIssueDetail, receipt => receipt.Id, issue => issue.IssuePKey, (receipt, issue) => new { receipt, issue })
                       .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.receipt, issue })
                       .Where(x => x.receipt.TransactionDate.Date >= DateTime.Parse(DateFrom) && x.receipt.TransactionDate.Date <= DateTime.Parse(DateTo) && x.issue.IsActive == true)
                       .Select(x => new DtoMiscIssue
                       {

                           IssueId = x.receipt.Id,
                           CustomerCode = x.receipt.Customercode,
                           CustomerName = x.receipt.Customer,
                           Details = x.receipt.Details,
                           Remarks = x.receipt.Remarks,
                           ItemCode = x.issue != null ? x.issue.ItemCode : null,
                           ItemDescription = x.issue != null ? x.issue.ItemDescription : null,
                           Uom = x.issue != null ? x.issue.Uom : null,
                           Quantity = x.issue != null ? x.issue.Quantity : 0,
                           TransactBy = x.receipt.PreparedBy,
                           TransactDate = x.issue.PreparedDate.ToString(),
                           PreparedDate = x.issue.PreparedDate.ToString(),
                           CompanyCode = x.receipt.CompanyCode,
                           CompanyName = x.receipt.CompanyName,
                           DepartmentCode = x.receipt.DepartmentCode,
                           DepartmentName = x.receipt.DepartmentName,
                           LocationCode = x.receipt.LocationCode,
                           LocationName = x.receipt.LocationName,
                           AccountCode = x.issue.AccountCode,
                           AccountTitles = x.issue.AccountTitles,
                           EmpId = x.issue.EmpId,
                           FullName = x.issue.FullName,
                           UnitCost = x.issue.UnitPrice,
                           TotalCost = x.issue.UnitPrice * x.issue.Quantity,
                           OneChargingName = x.receipt.one_charging_name,
                           BusinessUnitCode = x.receipt.business_unit_code,
                           BusinessUnitName = x.receipt.business_unit_name,
                           DepartmentUnitCode = x.receipt.department_unit_code,
                           DepartmentUnitName = x.receipt.department_unit_name,
                           SubUnitCode = x.receipt.sub_unit_code,
                           SubUnitName = x.receipt.sub_unit_name,

                       });

            if (!string.IsNullOrEmpty(Search))
            {
                issues = issues.Where(x => Convert.ToString(x.IssueId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoMiscIssue>.CreateAsync(issues, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<BorrowedTransactionReportsDto>> BorrowedTransactionReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {

            var borrowed = _context.BorrowedIssues
                .GroupJoin(_context.BorrowedIssueDetails, issue => issue.Id, borrow => borrow.BorrowedPKey, (issue, borrow) => new { issue, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.issue, borrow })
                .Where(x => x.issue.PreparedDate.Date >= DateTime.Parse(DateFrom).Date && x.issue.PreparedDate.Date <= DateTime.Parse(DateTo).Date)
                .Where(x => x.issue.IsActive == true)
                .GroupBy(x => x.issue.Id)
                .Select(x => new BorrowedTransactionReportsDto
                {
                    BorrowedId = x.Key,
                    CustomerCode = x.First().issue.CustomerCode,
                    CustomerName = x.First().issue.CustomerName,
                    EmpId = x.First().issue.EmpId,
                    FullName = x.First().issue.FullName,
                    TransactedBy = x.First().issue.PreparedBy,
                    BorrowedDate = x.First().issue.PreparedDate.ToString(),
                    Details = x.First().issue.Details,
                    AgingDays = x.First().issue.IsApprovedReturnedDate != null ?
                    EF.Functions.DateDiffDay(x.First().issue.IsApprovedDate.Value, x.First().issue.IsApprovedReturnedDate)
                    : x.First().issue.IsApprovedDate == null ? 0 : EF.Functions.DateDiffDay(x.First().issue.IsApprovedDate, DateTime.Now),
                    Remarks = x.First().issue.Remarks,
                    Status = (x.First().issue.IsApproved == true) ? "Approved" :
                            (x.First().issue.IsApproved == false) ? "For Approval" : "Unknown",
                    BorrowedItemPkey = x.First().borrow.Id,
                    ItemCode = x.First().borrow.ItemCode,
                    ItemDescription = x.First().borrow.ItemDescription,
                    Uom = x.First().borrow.Uom,
                    BorrowedQuantity = x.First().borrow.Quantity != null ? x.First().borrow.Quantity : 0,

                });

            if (!string.IsNullOrEmpty(Search))
            {
                borrowed = borrowed.Where(x => Convert.ToString(x.BorrowedId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<BorrowedTransactionReportsDto>.CreateAsync(borrowed, userParams.PageNumber, userParams.PageSize);



        }


        public async Task<PagedList<DtoBorrowedAndReturned>> ReturnBorrowedReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {

            var ConsumeQuantity = _context.BorrowedConsumes.Where(x => x.IsActive == true)
                                   .Select(x => new DtoBorrowedAndReturned
                                   {
                                       Id = x.Id,
                                       BorrowedId = x.BorrowedItemPkey,
                                       ItemCode = x.ItemCode,
                                       ItemDescription = x.ItemDescription,
                                       Consumed = x.Consume,
                                       CompanyCode = x.CompanyCode,
                                       CompanyName = x.CompanyName,
                                       DepartmentCode = x.DepartmentCode,
                                       DepartmentName = x.DepartmentName,
                                       LocationCode = x.LocationCode,
                                       LocationName = x.LocationName,
                                       AccountCode = x.AccountCode,
                                       AccountTitles = x.AccountTitles,
                                       EmpId = x.EmpId,
                                       FullName = x.FullName,
                                       ReportNumber = x.ReportNumber,
                                       OneChargingName = x.OneChargingName,
                                       BusinessUnitCode = x.BusinessUnitCode,
                                       BusinessUnitName = x.BusinessUnitName,
                                       DepartmentUnitCode = x.DepartmentUnitCode,
                                       DepartmentUnitName = x.DepartmentUnitName,
                                       SubUnitCode = x.SubUnitCode,
                                       SubUnitName = x.SubUnitName,

                                   });

            var details = _context.BorrowedIssueDetails
                         .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                         .GroupJoin(ConsumeQuantity, borrow => borrow.Id, consume => consume.BorrowedId, (borrow, consume) => new { borrow, consume })
                         .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrow, consume })
                         .Select(x => new DtoBorrowedAndReturned
                         {

                             Id = x.borrow.Id,
                             BorrowedId = x.borrow.BorrowedPKey,
                             ItemCode = x.borrow.ItemCode,
                             ItemDescription = x.borrow.ItemDescription,
                             Uom = x.borrow.Uom,
                             BorrowedQuantity = x.borrow.Quantity != null ? x.borrow.Quantity : 0,
                             Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                             CompanyCode = x.consume.CompanyCode,
                             CompanyName = x.consume.CompanyName,
                             DepartmentCode = x.consume.DepartmentCode,
                             DepartmentName = x.consume.DepartmentName,
                             LocationCode = x.consume.LocationCode,
                             LocationName = x.consume.LocationName,
                             AccountCode = x.consume.AccountCode,
                             AccountTitles = x.consume.AccountTitles,
                             EmpId = x.consume.EmpId,
                             FullName = x.consume.FullName,
                             ReportNumber = x.consume.ReportNumber,
                             UnitCost = x.borrow.UnitPrice,
                             OneChargingName = x.consume.OneChargingName,
                             BusinessUnitCode = x.consume.BusinessUnitCode,
                             BusinessUnitName = x.consume.BusinessUnitName,
                             DepartmentUnitCode = x.consume.DepartmentUnitCode,
                             DepartmentUnitName = x.consume.DepartmentUnitName,
                             SubUnitCode = x.consume.SubUnitCode,
                             SubUnitName = x.consume.SubUnitName,

                         });


            var borrowIssue = _context.BorrowedIssues
                      .Where(x => x.IsActive == true);

            var Reports = details
                         .GroupJoin(borrowIssue, returned => returned.BorrowedId, borrowed => borrowed.Id, (returned, borrowed) => new { returned, borrowed })
                         .SelectMany(x => x.borrowed.DefaultIfEmpty(), (x, borrowed) => new { x.returned, borrowed })
                         .Where(x => x.borrowed.PreparedDate.Date >= DateTime.Parse(DateFrom).Date && x.borrowed.PreparedDate.Date <= DateTime.Parse(DateTo).Date)
                         .Select(x => new DtoBorrowedAndReturned
                         {

                             BorrowedId = x.borrowed.Id,
                             CustomerCode = x.borrowed.CustomerCode,
                             CustomerName = x.borrowed.CustomerName,
                             EmpIdByIssue = x.borrowed.EmpId,
                             FullNameByIssue = x.borrowed.FullName,
                             ItemCode = x.returned.ItemCode,
                             ItemDescription = x.returned.ItemDescription,
                             Uom = x.returned.Uom,
                             BorrowedQuantity = x.returned.BorrowedQuantity,
                             Consumed = x.returned.Consumed,
                             ReturnedQuantity = x.returned.BorrowedQuantity - x.returned.Consumed,
                             TransactedBy = x.borrowed.PreparedBy,
                             BorrowedDate = x.borrowed.PreparedDate.ToString(),
                             Remarks = x.borrowed.Remarks,
                             Details = x.borrowed.Details,
                             Status = x.borrowed.StatusApproved,
                             IsApproveReturnDate = x.borrowed.IsApprovedReturnedDate.ToString(),
                             CompanyCode = x.returned.CompanyCode,
                             CompanyName = x.returned.CompanyName,
                             DepartmentCode = x.returned.DepartmentCode,
                             DepartmentName = x.returned.DepartmentName,
                             LocationCode = x.returned.LocationCode,
                             LocationName = x.returned.LocationName,
                             AccountCode = x.returned.AccountCode,
                             AccountTitles = x.returned.AccountTitles,
                             EmpId = x.returned.EmpId,
                             FullName = x.returned.FullName,
                             ReportNumber = x.returned.ReportNumber,
                             AgingDays = x.borrowed.IsApprovedReturnedDate != null ? EF.Functions.DateDiffDay(x.borrowed.IsApprovedDate.Value, x.borrowed.IsApprovedReturnedDate.Value)
                             : x.borrowed.IsApprovedDate == null ? 0 : EF.Functions.DateDiffDay(x.borrowed.IsApprovedDate, DateTime.Now),
                             IsActive = x.borrowed.IsActive,
                             UnitCost = x.returned.UnitCost,
                             LineAmount = Math.Round(x.returned.UnitCost.Value * x.returned.Consumed, 2),
                             OneChargingName = x.returned.OneChargingName,
                             BusinessUnitCode = x.returned.BusinessUnitCode,
                             BusinessUnitName = x.returned.BusinessUnitName,
                             DepartmentUnitCode = x.returned.DepartmentUnitCode,
                             DepartmentUnitName = x.returned.DepartmentUnitName,
                             SubUnitCode = x.returned.SubUnitCode,
                             SubUnitName = x.returned.SubUnitName,



                         });

            if (!string.IsNullOrEmpty(Search))
            {
                Reports = Reports.Where(x => Convert.ToString(x.BorrowedId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoBorrowedAndReturned>.CreateAsync(Reports, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<FuelRegisterReportsDto>> FuelRegisterReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {
            var results = _context.FuelRegisterDetails
                .Include(m => m.Material)
                .ThenInclude(id => id.ItemCategory)
                .Include(w => w.Warehouse_Receiving)
                .Where(r => r.FuelRegister.Is_Transact == true)
                .Select(r => new FuelRegisterReportsDto
                {
                    Id = r.Id,
                    Source = "Diesel Tank (Head Office)",
                    RequestorId = r.FuelRegister.RequestorId,
                    RequestorName = r.FuelRegister.RequestorName,
                    Item_Code = r.Material.ItemCode,
                    Item_Description = r.Material.ItemDescription,
                    Uom = r.Material.Uom.UomCode,
                    Item_Categories = r.Material.ItemCategory.ItemCategoryName,
                    Warehouse_ReceivingId = r.Warehouse_ReceivingId,
                    Unit_Cost = (r.Warehouse_Receiving.PriceWithDecimal == null ? r.Warehouse_Receiving.UnitPrice : decimal.Parse(r.Warehouse_Receiving.PriceWithDecimal)).ToString(),
                    Liters = r.Liters.Value,
                    lineAmount = (r.Warehouse_Receiving.PriceWithDecimal == null ? r.Warehouse_Receiving.UnitPrice * r.Liters.Value : decimal.Parse(r.Warehouse_Receiving.PriceWithDecimal) * r.Liters.Value).ToString(),
                    Asset = r.FuelRegister.Asset.AssetCode,
                    Odometer = r.FuelRegister.Odometer,
                    Company_Code = r.FuelRegister.Company_Code,
                    Company_Name = r.FuelRegister.Company_Name,
                    Department_Code = r.FuelRegister.Department_Code,
                    Department_Name = r.FuelRegister.Department_Name,
                    Location_Code = r.FuelRegister.Location_Code,
                    Location_Name = r.FuelRegister.Location_Name,
                    Account_Title_Code = r.FuelRegister.Account_Title_Code,
                    Account_Title_Name = r.FuelRegister.Account_Title_Name,
                    EmpId = r.FuelRegister.EmpId,
                    Fullname = r.FuelRegister.Fullname,
                    Added_By = r.Added_By,
                    Created_At = r.Created_At,
                    Modified_By = r.Modified_By,
                    Approve_By = r.FuelRegister.Approve_By,
                    Transact_At = r.FuelRegister.Transact_At,
                    Remarks = r.FuelRegister.Remarks,
                    cipNo = r.FuelRegister.cipNo,
                    dieselPONumber = r.dieselPONumber,
                    fuelPump = r.fuelPump,
                    issuanceDate = r.FuelRegister.issuanceDate.Value.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                    Month = r.FuelRegister.issuanceDate.Value.ToString("yyMM"),
                    OneChargingName = r.FuelRegister.OneChargingName,
                    BusinessUnitCode = r.FuelRegister.BusinessUnitCode,
                    BusinessUnitName = r.FuelRegister.BusinessUnitName,
                    DepartmentUnitCode = r.FuelRegister.DepartmentUnitCode,
                    DepartmentUnitName = r.FuelRegister.DepartmentUnitName,
                    SubUnitCode = r.FuelRegister.SubUnitCode,
                    SubUnitName = r.FuelRegister.SubUnitName,
                });


            //DateTime from = DateTime.ParseExact(DateFrom, "yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
            //DateTime to = DateTime.ParseExact(DateTo, "yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(DateTo) && !string.IsNullOrEmpty(DateFrom))
            {
                results = results
                    .Where(r => r.Transact_At.Value.Date >= Convert.ToDateTime(DateFrom).Date && r.Transact_At.Value.Date <= Convert.ToDateTime(DateTo).Date);
            }

            if (!string.IsNullOrEmpty(Search))
                results = results.Where(r => r.RequestorName.Contains(Search) && r.Id.ToString().Contains(Search));

            return await PagedList<FuelRegisterReportsDto>.CreateAsync(results, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoCancelledReports>> CancelledReports(UserParams userParams, string DateFrom, string DateTo, string Search)
        {

            var cancelled = _context.Orders.Where(x => x.IsCancel == true || x.QuantityOrdered != x.StandartQuantity)
                                        .GroupBy(x => new
                                        {
                                            x.TrasactId,
                                            x.Id,
                                            x.OrderNo,
                                            x.DateNeeded,
                                            x.OrderDate,
                                            x.Customercode,
                                            x.CustomerName,
                                            x.ItemCode,
                                            x.ItemdDescription,
                                            Remarks = x.Remarks,


                                        }).Select(x => new DtoCancelledReports
                                        {
                                            MIRId = x.Key.TrasactId,
                                            OrderId = x.Key.Id,
                                            OrderNo = x.Key.OrderNo,
                                            DateNeeded = x.Key.DateNeeded.ToString(/*"yyyy-MM-dd"*/),
                                            DateOrdered = x.Key.OrderDate.ToString(/*"yyyy-MM-dd"*/),
                                            CustomerCode = x.Key.Customercode,
                                            CustomerName = x.Key.CustomerName,
                                            ItemCode = x.Key.ItemCode,

                                            CancelledDate = x.First().CancelDate != null
                                            ? x.First().CancelDate : x.First().Modified_Date == null
                                            ? x.First().OrderDate : x.First().Modified_Date,

                                            CancelledBy = x.First().IsCancelBy != null ? x.First().IsCancelBy : x.First().Modified_By,
                                            ItemDescription = x.Key.ItemdDescription,
                                            QuantityOrdered = x.Sum(x => x.QuantityOrdered),
                                            Reason = x.Key.Remarks,

                                        });


            var orders = _context.Orders
                       .GroupJoin(cancelled, order => order.Id, cancel => cancel.OrderId, (order, cancel) => new { order, cancel })
                       .SelectMany(x => x.cancel.DefaultIfEmpty(), (x, cancel) => new { x.order, cancel })

                       .Where(x => x.cancel.CancelledDate.Value.Date >= DateTime.Parse(DateFrom).Date
                       && x.cancel.CancelledDate.Value.Date <= DateTime.Parse(DateTo).Date)

                       .GroupBy(x => new
                       {
                           x.order.TrasactId,
                           x.order.Id,
                           x.order.OrderNo,
                           x.order.OrderDate,
                           x.order.DateNeeded,
                           x.order.Customercode,
                           x.order.CustomerName,
                           x.order.ItemCode,
                           x.order.ItemdDescription,

                       }).Select(x => new DtoCancelledReports
                       {
                           MIRId = x.Key.TrasactId,
                           OrderId = x.Key.Id,
                           OrderNo = x.Key.OrderNo,
                           DateNeeded = x.Key.DateNeeded.ToString(/*"yyyy-MM-dd hh:mm tt"*/),
                           DateOrdered = x.Key.OrderDate.ToString(/*"yyyy-MM-dd hh:mm tt"*/),
                           CustomerCode = x.Key.Customercode,
                           CustomerName = x.Key.CustomerName,
                           ItemCode = x.Key.ItemCode,
                           ItemDescription = x.Key.ItemdDescription,
                           CancelledBy = x.First().cancel.CancelledBy,
                           CancelledDate = x.First().cancel.CancelledDate,
                           Reason = x.First().cancel.Reason != null ? x.First().cancel.Reason : x.First().order.Remarks,
                           QuantityOrdered = x.Sum(x => x.cancel.QuantityOrdered) != null && x.Sum(x => x.order.StandartQuantity) - x.Sum(x => x.order.QuantityOrdered) != 0 ?
                           x.Sum(x => x.order.StandartQuantity) - x.Sum(x => x.order.QuantityOrdered) : x.Sum(x => x.order.StandartQuantity),

                           DepartmentCode = x.First().order.DepartmentCode,
                           Department = x.First().order.Department,
                           CompanyCode = x.First().order.CompanyCode,
                           CompanyName = x.First().order.CompanyName,
                           LocationCode = x.First().order.LocationCode,
                           LocationName = x.First().order.LocationName,
                           AccountCode = x.First().order.AccountCode,
                           AccountTitles = x.First().order.AccountTitles,
                           ItemRemarks = x.First().order.ItemRemarks,
                           OneChargingName = x.First().order.one_charging_name,
                           BusinessUnitCode = x.First().order.business_unit_code,
                           BusinessUnitName = x.First().order.business_unit_name,
                           DepartmentUnitCode = x.First().order.department_unit_code,
                           DepartmentUnitName = x.First().order.department_unit_name,
                           SubUnitCode = x.First().order.sub_unit_code,
                           SubUnitName = x.First().order.sub_unit_name,


                       });


            if (!string.IsNullOrEmpty(Search))
            {
                orders = orders.Where(x => Convert.ToString(x.OrderId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.MIRId).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.CustomerName).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            return await PagedList<DtoCancelledReports>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoInventoryMovement>> InventoryMovementReports(UserParams userParams, string DateFrom, string PlusOne, string Search)
        {
            var DateToday = DateTime.Today.AddDays(1);//dito

            var getWarehouseStock = _context.WarehouseReceived
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new WarehouseInventory
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(x => x.ActualGood)
                });

            var getMoveOrdersOutByDate = _context.MoveOrders
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .Where(x => x.PreparedDate.Value >= DateTime.Parse(DateFrom) && x.PreparedDate.Value <= DateTime.Parse(PlusOne).AddDays(1))//dito
               .GroupBy(x => new
               {

                   x.ItemCode,

               }).Select(x => new MoveOrderInventory
               {

                   ItemCode = x.Key.ItemCode,
                   QuantityOrdered = x.Sum(x => x.QuantityOrdered)

               });

            var getMoveOrdersOutbyDatePlus = _context.MoveOrders
                 .AsNoTracking()
                 .Where(x => x.IsActive == true)
                 .Where(x => x.IsPrepared == true)
                 .Where(x => x.PreparedDate.Value >= DateTime.Parse(PlusOne).AddDays(1) && x.PreparedDate.Value <= DateToday)
                 .GroupBy(x => new
                 {

                     x.ItemCode,

                 }).Select(x => new MoveOrderInventory
                 {

                     ItemCode = x.Key.ItemCode,
                     QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                 });


            var getIssueOutByDate = _context.MiscellaneousIssueDetail
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoMiscIssue
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });

            var getIssueOutByDatePlus = _context.MiscellaneousIssueDetail
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne).AddDays(1) && x.PreparedDate <= (DateToday))
                .GroupBy(x => new
                {

                    x.ItemCode,

                }).Select(x => new DtoMiscIssue
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });

            var getBorrowedOutByDate = _context.BorrowedIssueDetails
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.BorrowedDate >= DateTime.Parse(DateFrom) && x.BorrowedDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoBorrowedIssue
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });


            var getBorrowedOutByDatePlus = _context.BorrowedIssueDetails
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.BorrowedDate >= DateTime.Parse(PlusOne).AddDays(1)
                 && x.BorrowedDate <= (DateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoBorrowedIssue
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });



            var consumed = _context.BorrowedConsumes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.BorrowedItemPkey

                }).Select(x => new ItemStocksDto
                {
                    ItemCode = x.Key.ItemCode,
                    BorrowedItemPkey = x.Key.BorrowedItemPkey,
                    Consume = x.Sum(x => x.Consume != null ? x.Consume : 0)

                });

            var getReturnedOutByDate = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsReturned == true)
                .Where(x => x.IsApprovedReturned == true)
                .GroupJoin(consumed, returned => returned.Id, consume => consume.BorrowedItemPkey, (returned, consume) => new { returned, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.returned, consume })
                .Where(x => x.returned.IsApprovedReturnedDate.Value >= DateTime.Parse(DateFrom)
                 && x.returned.IsApprovedReturnedDate.Value <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.returned.ItemCode,

                }).Select(x => new DtoBorrowedIssue
                {

                    ItemCode = x.Key.ItemCode,
                    ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

                });

            var getReturnedOutByDatePlus = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsReturned == true)
                .Where(x => x.IsApprovedReturned == true)
                .GroupJoin(consumed, returned => returned.Id, consume => consume.BorrowedItemPkey, (returned, consume) => new { returned, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.returned, consume })
                .Where(x => x.returned.IsApprovedReturnedDate.Value >= DateTime.Parse(PlusOne).AddDays(1)
                 && x.returned.IsApprovedReturnedDate.Value <= (DateToday))
                 .GroupBy(x => new
                 {
                     x.returned.ItemCode,

                 }).Select(x => new DtoBorrowedIssue
                 {

                     ItemCode = x.Key.ItemCode,
                     ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

                 });

            var fuelRegisterByDate = _context.FuelRegisterDetails
                .AsNoTracking()
                .Where(fr => fr.Is_Active == true)
                .Where(x => x.Created_At.Date >= DateTime.Parse(DateFrom).Date && x.Created_At.Date <= DateTime.Parse(PlusOne).Date.AddDays(1))
                .GroupBy(fr => new
                {
                    fr.Material.ItemCode,

                }).Select(fr => new
                {
                    itemCode = fr.Key.ItemCode,
                    Quantity = fr.Sum(fr => fr.Liters != null ? fr.Liters : 0)

                });


            var fuelRegisterByDatePlus = _context.FuelRegisterDetails
                .AsNoTrackingWithIdentityResolution()
                .Include(m => m.Material)
                .AsSplitQuery()
                .Where(fr => fr.Is_Active == true)
                .Where(x => x.Created_At.Date >= DateTime.Parse(PlusOne).AddDays(1).Date && x.Created_At.Date <= (DateToday).Date)
                .GroupBy(fr => new
                {
                    fr.Material.ItemCode,

                }).Select(fr => new
                {
                    itemCode = fr.Key.ItemCode,
                    Quantity = fr.Sum(fr => fr.Liters != null ? fr.Liters : 0)

                });

            var getReceiveIn = _context.WarehouseReceived
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Receiving")
                .Where(x => x.ActualReceivingDate >= DateTime.Parse(DateFrom) && x.ActualReceivingDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoReceiveIn
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)

                });

            var getReceiveInPlus = _context.WarehouseReceived
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Receiving")
                .Where(x => x.ActualReceivingDate >= DateTime.Parse(PlusOne).AddDays(1) && x.ActualReceivingDate <= DateToday)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoReceiveIn
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)

                });

            var getReceiptIn = _context.WarehouseReceived
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "MiscellaneousReceipt")
                .Where(x => x.ActualReceivingDate >= DateTime.Parse(DateFrom) && x.ActualReceivingDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {

                    x.ItemCode,

                }).Select(x => new DtoRecieptIn
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)

                });


            var getReceiptInPlus = _context.WarehouseReceived.AsNoTracking()
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "MiscellaneousReceipt")
                .Where(x => x.ActualReceivingDate >= DateTime.Parse(PlusOne).AddDays(1) && x.ActualReceivingDate <= DateToday)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoRecieptIn
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)

                });


            var getMoveOrderOut = _context.MoveOrders
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
                {

                    x.ItemCode,

                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                });


            var getIssueOut = _context.MiscellaneousIssueDetail
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {

                    x.ItemCode

                }).Select(x => new DtoMiscIssue
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });


            var getBorrowedOut = _context.BorrowedIssueDetails
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new DtoBorrowedIssue
                {

                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)

                });

            var getReturned = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsReturned == true)
                .Where(x => x.IsApprovedReturned == true)
                .GroupJoin(consumed, returned => returned.Id, consume => consume.BorrowedItemPkey, (returned, consume) => new { returned, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.returned, consume })
                .GroupBy(x => new
                {

                    x.returned.ItemCode,

                }).Select(x => new DtoBorrowedIssue
                {

                    ItemCode = x.Key.ItemCode,
                    ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume)

                });


            var fuelRegisterOut = _context.FuelRegisterDetails
               .AsNoTrackingWithIdentityResolution()
               .Include(m => m.Material)
               .AsSplitQuery()
               .Where(fr => fr.Is_Active == true)
               .GroupBy(fr => new
               {
                   fr.Material.ItemCode,

               }).Select(fr => new
               {
                   itemCode = fr.Key.ItemCode,
                   Quantity = fr.Sum(fr => fr.Liters != null ? fr.Liters : 0)

               });


            var getSOH = (from warehouse in getWarehouseStock
                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ1
                          from moveorder in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedOut
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturned
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          join fuel in fuelRegisterOut
                          on warehouse.ItemCode equals fuel.itemCode
                          into leftJ5
                          from fuel in leftJ5.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,
                              fuel,

                          }

                          by new
                          {
                              warehouse.ItemCode
                          }

                          into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0) -
                             total.Sum(x => x.fuel.Quantity.Value)

                          });

            var wareHouseUnitCost = _context.WarehouseReceived
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.ActualReceivingDate >= DateTime.Parse(DateFrom) && x.ActualReceivingDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .Select(x => new
                {
                    x.Id,
                    x.ItemCode,
                    x.ActualGood,
                    x.PriceWithDecimal,
                    x.UnitPrice,
                });

            var moveOrderUnitCost = _context.MoveOrders
                .AsNoTracking()
                 .Where(x => x.IsActive == true)
                 .Where(x => x.IsPrepared == true)
                 .Where(x => x.PreparedDate.Value >= DateTime.Parse(DateFrom) && x.PreparedDate.Value <= DateTime.Parse(PlusOne).AddDays(1))//dito
                 .GroupBy(x => new { x.WarehouseId, x.ItemCode, })
                 .Select(x => new
                 {

                     x.Key.WarehouseId,
                     x.Key.ItemCode,
                     Quantity = x.Sum(x => x.QuantityOrdered)

                 });

            var issueUnitCost = _context.MiscellaneousIssueDetail
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.WarehouseId,
                    x.ItemCode,

                }).Select(x => new
                {
                    x.Key.WarehouseId,
                    x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity),

                });


            var borrowedUnitCost = _context.BorrowedIssueDetails
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                 .Where(x => x.BorrowedDate >= DateTime.Parse(DateFrom) && x.BorrowedDate <= DateTime.Parse(PlusOne).AddDays(1))//dito
                 .GroupBy(x => new
                 {
                     x.WarehouseId,
                     x.ItemCode,

                 }).Select(x => new
                 {
                     x.Key.WarehouseId,
                     x.Key.ItemCode,
                     Quantity = x.Sum(x => x.Quantity),


                 });


            var returnUnitCost = _context.BorrowedIssueDetails
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(x => x.IsActive == true)
                .Where(x => x.IsReturned == true)
                .Where(x => x.IsApprovedReturned == true)
                .GroupJoin(consumed, returned => returned.Id, consume => consume.BorrowedItemPkey, (returned, consume) => new { returned, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.returned, consume })
                .Where(x => x.returned.IsApprovedReturnedDate.Value >= DateTime.Parse(DateFrom)
                        && x.returned.IsApprovedReturnedDate.Value <= DateTime.Parse(PlusOne).AddDays(1))//dito
                .GroupBy(x => new
                {
                    x.returned.WarehouseId,
                    x.returned.ItemCode,

                }).Select(x => new
                {

                    x.Key.WarehouseId,
                    x.Key.ItemCode,
                    ReturnQuantity = x.Sum(x => x.returned.Quantity) - x.Sum(x => x.consume.Consume),

                });

            var fuelRegisterCost = _context.FuelRegisterDetails
             .AsNoTrackingWithIdentityResolution()
             .Include(m => m.Material)
             .AsSplitQuery()
             .Where(fr => fr.Is_Active == true)
             .Where(x => x.Created_At.Date >= DateTime.Parse(DateFrom).AddDays(1).Date && x.Created_At.Date <= DateTime.Parse(PlusOne).Date.AddDays(1).AddDays(1))//dito
            .GroupBy(fr => new
            {
                fr.Material.ItemCode,
                fr.Warehouse_ReceivingId,

            }).Select(fr => new
            {
                itemCode = fr.Key.ItemCode,
                WarehouseId = fr.Key.Warehouse_ReceivingId,
                Quantity = fr.Sum(fr => fr.Liters != null ? fr.Liters : 0)

            });

            var getUnitPrice = (from warehouse in wareHouseUnitCost
                                join moveorder in moveOrderUnitCost
                                on warehouse.Id equals moveorder.WarehouseId
                                into leftJ1
                                from moveorder in leftJ1.DefaultIfEmpty()

                                join issue in issueUnitCost
                                on warehouse.Id equals issue.WarehouseId
                                into leftJ2
                                from issue in leftJ2.DefaultIfEmpty()

                                join borrow in borrowedUnitCost
                                on warehouse.Id equals borrow.WarehouseId
                                into leftJ3
                                from borrow in leftJ3.DefaultIfEmpty()

                                join returned in returnUnitCost
                                on warehouse.Id equals returned.WarehouseId
                                into leftJ4
                                from returned in leftJ4.DefaultIfEmpty()

                                join fuel in fuelRegisterCost
                                on warehouse.Id equals fuel.WarehouseId
                                into leftJ5
                                from fuel in leftJ5.DefaultIfEmpty()
                                group new
                                {
                                    warehouse,
                                    moveorder,
                                    issue,
                                    borrow,
                                    returned,
                                    fuel,

                                }

                              by new
                              {
                                  warehouse.Id,
                                  warehouse.ItemCode,

                              }
                               into x
                                select new WarehouseInventory
                                {

                                    WarehouseId = x.Key.Id,
                                    ItemCode = x.Key.ItemCode,
                                    UnitPrice = Math.Round(x.Sum(x => x.warehouse.UnitPrice) * (x.First().warehouse.ActualGood + x.Sum(x => x.returned.ReturnQuantity) - x.Sum(x => x.moveorder.Quantity) - x.Sum(x => x.issue.Quantity) - x.Sum(x => x.borrow.Quantity)), 2),
                                    ActualGood = x.First().warehouse.ActualGood + (x.Sum(x => x.returned.ReturnQuantity)
                                    - x.Sum(x => x.moveorder.Quantity) - x.Sum(x => x.issue.Quantity) - x.Sum(x => x.borrow.Quantity) -
                                    x.Sum(x => x.fuel.Quantity.Value)),

                                });


            var getUnitprice = getUnitPrice
                 .Where(x => x.UnitPrice > 0)
                 .GroupBy(x => new
                 {
                     x.ItemCode,

                 }).Select(x => new WarehouseInventory
                 {
                     ItemCode = x.Key.ItemCode,
                     UnitPrice = x.Sum(x => x.UnitPrice != null ? x.UnitPrice : 0) / x.Sum(x => x.ActualGood),
                     ActualGood = x.Sum(x => x.ActualGood),

                 });

            var getUnitpriceTotal = getUnitprice
                 .GroupBy(x => new
                 {
                     x.ItemCode,

                 }).Select(x => new WarehouseInventory
                 {
                     ItemCode = x.Key.ItemCode,
                     UnitPrice = x.First().UnitPrice,
                     TotalUnitPrice = x.First().UnitPrice * x.First().ActualGood,

                 });


            var movementInventory = (from material in _context.Materials
                                     where material.IsActive == true
                                     join moveorder in getMoveOrdersOutByDate
                                     on material.ItemCode equals moveorder.ItemCode
                                     into leftJ1
                                     from moveorder in leftJ1.DefaultIfEmpty()

                                     join issue in getIssueOutByDate
                                     on material.ItemCode equals issue.ItemCode
                                     into leftJ2
                                     from issue in leftJ2.DefaultIfEmpty()

                                     join borrowed in getBorrowedOutByDate
                                     on material.ItemCode equals borrowed.ItemCode
                                     into leftJ3
                                     from borrowed in leftJ3.DefaultIfEmpty()

                                     join returned in getReturnedOutByDate
                                     on material.ItemCode equals returned.ItemCode
                                     into leftJ4
                                     from returned in leftJ4.DefaultIfEmpty()

                                     join receiveIn in getReceiveIn
                                     on material.ItemCode equals receiveIn.ItemCode
                                     into leftJ5
                                     from receiveIn in leftJ5.DefaultIfEmpty()

                                     join receipt in getReceiptIn
                                     on material.ItemCode equals receipt.ItemCode
                                     into leftJ6
                                     from receipt in leftJ6.DefaultIfEmpty()

                                     join SOH in getSOH
                                     on material.ItemCode equals SOH.ItemCode
                                     into leftJ7
                                     from SOH in leftJ7.DefaultIfEmpty()

                                     join moveorderPlus in getMoveOrdersOutbyDatePlus
                                     on material.ItemCode equals moveorderPlus.ItemCode
                                     into leftJ8
                                     from moverorderPlus in leftJ8.DefaultIfEmpty()

                                     join issuePlus in getIssueOutByDatePlus
                                     on material.ItemCode equals issuePlus.ItemCode
                                     into leftJ9
                                     from issuePlus in leftJ9.DefaultIfEmpty()

                                     join borrowedPlus in getBorrowedOutByDatePlus
                                     on material.ItemCode equals borrowedPlus.ItemCode
                                     into leftJ10
                                     from borrowedPlus in leftJ10.DefaultIfEmpty()

                                     join returnedPlus in getReturnedOutByDatePlus
                                     on material.ItemCode equals returnedPlus.ItemCode
                                     into leftJ11
                                     from returnedPlus in leftJ11.DefaultIfEmpty()

                                     join receiveInPlus in getReceiveInPlus
                                     on material.ItemCode equals receiveInPlus.ItemCode
                                     into leftJ12
                                     from receiveInPlus in leftJ12.DefaultIfEmpty()

                                     join receiptInPlus in getReceiptInPlus
                                     on material.ItemCode equals receiptInPlus.ItemCode
                                     into leftJ13
                                     from receiptInPlus in leftJ13.DefaultIfEmpty()

                                     join unit in getUnitpriceTotal
                                     on material.ItemCode equals unit.ItemCode
                                     into leftJ14
                                     from unit in leftJ14.DefaultIfEmpty()

                                     join fuel in fuelRegisterByDate
                                     on material.ItemCode equals fuel.itemCode
                                     into leftJ15
                                     from fuel in leftJ15.DefaultIfEmpty()

                                     join fuelPlus in fuelRegisterByDatePlus
                                     on material.ItemCode equals fuelPlus.itemCode
                                     into leftJ16
                                     from fuelPlus in leftJ16.DefaultIfEmpty()

                                     group new
                                     {

                                         material,
                                         moveorder,
                                         issue,
                                         borrowed,
                                         returned,
                                         receiveIn,
                                         receipt,
                                         SOH,
                                         moverorderPlus,
                                         issuePlus,
                                         borrowedPlus,
                                         returnedPlus,
                                         receiveInPlus,
                                         receiptInPlus,
                                         unit,
                                         fuel,
                                         fuelPlus,
                                     }

                                     by new
                                     {
                                         material.ItemCode,
                                         material.ItemDescription,
                                     }
                                     into total

                                     select new DtoInventoryMovement
                                     {

                                         ItemCode = total.Key.ItemCode,
                                         ItemDescription = total.Key.ItemDescription,
                                         TotalReceiving = total.Sum(x => x.receiveIn.Quantity),
                                         TotalMoveOrder = total.Sum(x => x.moveorder.QuantityOrdered),
                                         TotalReceipt = total.Sum(x => x.receipt.Quantity),
                                         TotalIssue = total.Sum(x => x.issue.Quantity),
                                         TotalBorrowed = total.Sum(x => x.borrowed.Quantity),
                                         TotalReturned = total.Sum(x => x.returned.ReturnQuantity),
                                         TotalFuelRegister = total.Sum(x => x.fuel.Quantity.Value),
                                         Ending = (total.Sum(x => x.receipt.Quantity) + total.Sum(x => x.receiveIn.Quantity) + total.Sum(x => x.returned.ReturnQuantity)) -
                                         (total.Sum(x => x.borrowed.Quantity) + total.Sum(x => x.moveorder.QuantityOrdered) + total.Sum(x => x.issue.Quantity) + total.Sum(x => x.fuel.Quantity.Value)),
                                         UnitCost = total.Sum(x => x.unit.UnitPrice),
                                         Amount = total.Sum(x => x.unit.TotalUnitPrice),
                                         CurrentStock = total.Sum(x => x.SOH.SOH),
                                         PurchaseOrder = total.Sum(x => x.receiveInPlus.Quantity) + total.Sum(x => x.receiptInPlus.Quantity) + total.Sum(x => x.returnedPlus.ReturnQuantity),
                                         OtherPlus = total.Sum(x => x.moverorderPlus.QuantityOrdered) + total.Sum(x => x.issuePlus.Quantity) + total.Sum(x => x.borrowedPlus.Quantity) + total.Sum(x => x.fuelPlus.Quantity.Value),

                                     });

            if (!string.IsNullOrEmpty(Search))
            {
                movementInventory = movementInventory
                    .Where(x => Convert.ToString(x.ItemCode).ToLower().Contains(Search.Trim().ToLower())
                || Convert.ToString(x.ItemDescription).ToLower().Contains(Search.Trim().ToLower()));
            }

            movementInventory = movementInventory
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .OrderBy(x => x.ItemCode);


            return await PagedList<DtoInventoryMovement>.CreateAsync(movementInventory, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IReadOnlyList<ConsolidateFinanceReportDto>> ConsolidateFinanceReport(string DateFrom, string DateTo, string Search)
        {


            var dateStart = DateTime.Parse(DateFrom).Date;
            var dateEnd = DateTime.Parse(DateTo).Date.AddDays(1);


            var materials = _context.Materials
                .AsNoTracking()
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory).Where(x => x.IsActive == true);

            var receivingConsol = _context.WarehouseReceived
                .AsNoTracking()
                .Join(materials, ware => ware.ItemCode, material => material.ItemCode, (ware, material) => new { ware, material })
                .Where(x => x.ware.TransactionType == "Receiving" && x.ware.IsActive == true)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.ware.Id,
                    TransactionDate = x.ware.ActualReceivingDate.Date,
                    ItemCode = x.ware.ItemCode,
                    ItemDescription = x.ware.ItemDescription,
                    Uom = x.ware.Uom,
                    Category = x.material.ItemCategory.ItemCategoryName,
                    Quantity = x.ware.ActualGood,
                    UnitCost = (x.ware.PriceWithDecimal == null ? x.ware.UnitPrice : decimal.Parse(x.ware.PriceWithDecimal)).ToString(),
                    LineAmount = (x.ware.PriceWithDecimal == null ? x.ware.UnitPrice * x.ware.ActualGood : decimal.Parse(x.ware.PriceWithDecimal) * x.ware.ActualGood).ToString(),
                    Source = x.ware.PoNumber,
                    TransactionType = "Receiving",
                    Reason = "",
                    Reference = x.ware.SINumber,
                    SupplierName = x.ware.Supplier,
                    EncodedBy = x.ware.AddedBy,
                    CompanyCode = "10",
                    CompanyName = "RDF Corporate Services",
                    DepartmentCode = "0010",
                    DepartmentName = "Corporate Common",
                    LocationCode = "0001",
                    LocationName = "Head Office",
                    AccountTitleCode = "117701",
                    AccountTitle = "Materials & Supplies Inventory",
                    EmpId = "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = ""

                }).ToList();


            var moveOrderConsol = _context.MoveOrders
                .AsNoTracking()
                .GroupJoin(_context.TransactOrder, moveOrder => moveOrder.OrderNo, transMoveOrder => transMoveOrder.OrderNo, (moveOrder, transMoveOrder) => new { moveOrder, transMoveOrder })
                .SelectMany(x => x.transMoveOrder.DefaultIfEmpty(), (x, transMoveOrder) => new { x.moveOrder, transMoveOrder })
                .GroupJoin(_context.Users, trans => trans.transMoveOrder.PreparedBy, user => user.FullName, (trans, user) => new { trans, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.trans.moveOrder, x.trans.transMoveOrder, user })
                .GroupJoin(_context.OneAccountTitles, user => user.moveOrder.AccountCode, account => account.AccountCode, (user, account) => new { user, account })
                .SelectMany(x => x.account.DefaultIfEmpty(), (x, account) => new { x.user.moveOrder, x.user.transMoveOrder, x.user.user, account })
                .GroupJoin(materials, account => account.moveOrder.ItemCode, material => material.ItemCode, (account, material) => new {account, material })
                .SelectMany(x => x.material.DefaultIfEmpty(), (x, material) => new { x.account.moveOrder, x.account.transMoveOrder, x.account.user, x.account.account, material})
                
                
                .Where(x => x.moveOrder.IsTransact == true)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.moveOrder.Id,
                    TransactionDate = x.transMoveOrder.PreparedDate.Value.Date,
                    ItemCode = x.moveOrder.ItemCode  ,
                    ItemDescription = x.moveOrder.ItemDescription  ,
                    Uom = x.material.Uom.UomCode ,
                    Category = x.moveOrder.Category ,
                    Quantity = Math.Round(x.moveOrder.QuantityOrdered, 2),
                    UnitCost = x.moveOrder.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.moveOrder.UnitPrice * x.moveOrder.QuantityOrdered, 2)).ToString(),
                    Source = Convert.ToString(x.transMoveOrder.OrderNo),
                    TransactionType = "Move Order",
                    Reason = "",
                    Reference = x.moveOrder.ItemRemarks,
                    SupplierName = "",
                    EncodedBy = x.transMoveOrder.PreparedBy ,
                    CompanyCode = x.moveOrder.CompanyCode ,
                    CompanyName = x.moveOrder.CompanyName ,
                    ServiceProvider = x.user.FullName,
                    ServiceProviderCode = x.user.EmpId,
                    DepartmentCode = x.moveOrder.DepartmentCode ,
                    DepartmentName = x.moveOrder.DepartmentName ,
                    LocationCode = x.moveOrder.LocationCode ,
                    LocationName = x.moveOrder.LocationName ,
                    AccountTitleCode = x.moveOrder.AccountCode != null ? x.moveOrder.AccountCode : "537620",
                    AccountTitle = x.moveOrder.AccountTitles != null ? x.moveOrder.AccountTitles : "SE - R & M - Transport Vehicles",
                    EmpId = x.moveOrder.EmpId,
                    Fullname = x.moveOrder.FullName,
                    AssetTag = x.moveOrder.AssetTag ,
                    CIPNo = x.moveOrder.Cip_No ,
                    Helpdesk = x.moveOrder.HelpdeskNo ,
                    Rush = x.moveOrder.Rush ,
                    OneChargingName = x.moveOrder.one_charging_name ,
                    BusinessUnitName = x.moveOrder.business_unit_name ,
                    BusinessUnitCode = x.moveOrder.business_unit_code,
                    DepartmentUnitName = x.moveOrder.department_unit_name ,
                    DepartmentUnitCode = x.moveOrder.department_unit_code ,
                    SubUnitCode = x.moveOrder.sub_unit_code ,
                    SubUnitName = x.moveOrder.sub_unit_name ,

                    FinancialStatement = x.account.FinancialStatement,
                    UnitResponsible = x.account.Unit ,
                });


            var receiptConsol = _context.MiscellaneousReceipts
                .AsNoTracking()
                .GroupJoin(_context.WarehouseReceived, receipt => receipt.Id, warehouse => warehouse.MiscellaneousReceiptId, (receipt, warehouse) => new { receipt, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.receipt, warehouse }).Join(_context.Users, x => x.receipt.PreparedBy, user => user.FullName,
            (x, user) => new { x.receipt, x.warehouse, user })
                .GroupJoin(_context.OneAccountTitles, user => user.warehouse.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.receipt, x.user.warehouse, x.user.user, accountTitle })
               .Join(materials, account => account.warehouse.ItemCode, material => material.ItemCode, (x, material) => new { x.receipt, x.warehouse, x.user, x.accountTitle, material })
                .Where(x => x.warehouse.IsActive == true && x.warehouse.TransactionType == "MiscellaneousReceipt")
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.warehouse.Id,
                    TransactionDate = x.receipt.TransactionDate,
                    ItemCode = x.warehouse.ItemCode,
                    ItemDescription = x.warehouse.ItemDescription,
                    Uom = x.warehouse.Uom,
                    Category = "",
                    Quantity = x.warehouse.ActualGood,
                    UnitCost =  (x.warehouse.PriceWithDecimal == null ? x.warehouse.UnitPrice : decimal.Parse(x.warehouse.PriceWithDecimal)).ToString(),
                    LineAmount = (x.warehouse.PriceWithDecimal == null ? x.warehouse.UnitPrice * x.warehouse.ActualGood  : decimal.Parse(x.warehouse.PriceWithDecimal) * x.warehouse.ActualGood).ToString(),
                    Source = Convert.ToString(x.receipt.Id),
                    TransactionType = "Miscellaneous Receipt",
                    Reason = x.receipt.Remarks,
                    Reference = x.receipt.Details,
                    SupplierName = x.receipt.supplier,
                    EncodedBy = x.receipt.PreparedBy,
                    CompanyCode = x.receipt.CompanyCode,
                    CompanyName = x.receipt.CompanyName,
                    DepartmentCode = x.receipt.DepartmentCode,
                    DepartmentName = x.receipt.DepartmentName,
                    LocationCode = x.receipt.LocationCode,
                    LocationName = x.receipt.LocationName,
                    AccountTitleCode = x.warehouse.AccountCode,
                    AccountTitle = x.warehouse.AccountTitles,
                    EmpId = x.warehouse.EmpId,
                    Fullname = x.warehouse.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    //Remarks = x.receipt.Remarks,
                    Rush = "",
                    OneChargingName = x.receipt.one_charging_name,
                    BusinessUnitName = x.receipt.business_unit_name,
                    BusinessUnitCode = x.receipt.business_unit_code,
                    DepartmentUnitName = x.receipt.department_unit_name,
                    DepartmentUnitCode = x.receipt.department_unit_code,
                    SubUnitCode = x.receipt.sub_unit_code,
                    SubUnitName = x.receipt.sub_unit_name,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,
                }).Distinct();



            var issueConsol = _context.MiscellaneousIssues
                .AsNoTracking()
                .Join(_context.MiscellaneousIssueDetail, miscDatail => miscDatail.Id, issue => issue.IssuePKey,
                (miscDetail, issue) => new { miscDetail, issue }).Join(_context.Users, x => x.miscDetail.PreparedBy, user => user.FullName,
            (x, user) => new { x.miscDetail, x.issue, user })
                .GroupJoin(_context.OneAccountTitles, user => user.issue.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.miscDetail, x.user.issue, x.user.user, accountTitle })
                .Join(materials, account => account.issue.ItemCode, material => material.ItemCode, (x, material) => new { x.miscDetail, x.issue, x.user, x.accountTitle, material })
                .Where(x => x.issue.IsActive == true)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.issue.Id,
                    TransactionDate = x.miscDetail.TransactionDate.Date,
                    ItemCode = x.issue.ItemCode,
                    ItemDescription = x.issue.ItemDescription,
                    Uom = x.issue.Uom,
                    Category = "",
                    Quantity = Math.Round(x.issue.Quantity, 2),
                    UnitCost = x.issue.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.issue.UnitPrice * x.issue.Quantity, 2)).ToString(),
                    Source = Convert.ToString(x.issue.Id),
                    TransactionType = "Miscellaneous Issue",
                    Reason = x.issue.Remarks,
                    Reference = x.miscDetail.Details,
                    SupplierName = "",
                    EncodedBy = x.issue.PreparedBy,
                    CompanyCode = x.miscDetail.CompanyCode,
                    CompanyName = x.miscDetail.CompanyName,
                    DepartmentCode = x.miscDetail.DepartmentCode,
                    DepartmentName = x.miscDetail.DepartmentName,
                    LocationCode = x.miscDetail.LocationCode,
                    LocationName = x.miscDetail.LocationName,
                    AccountTitleCode = x.issue.AccountCode,
                    AccountTitle = x.issue.AccountTitles,
                    EmpId = x.issue.EmpId,
                    Fullname = x.issue.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.miscDetail.one_charging_name,
                    BusinessUnitName = x.miscDetail.business_unit_name,
                    BusinessUnitCode = x.miscDetail.business_unit_code,
                    DepartmentUnitName = x.miscDetail.department_unit_name,
                    DepartmentUnitCode = x.miscDetail.department_unit_code,
                    SubUnitCode = x.miscDetail.sub_unit_code,
                    SubUnitName = x.miscDetail.sub_unit_name,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,

                }).Distinct();

            var borrowedConsol = _context.BorrowedIssues
                .AsNoTracking()
                .Join(_context.BorrowedIssueDetails, borrow => borrow.Id, borrowDetail => borrowDetail.BorrowedPKey,
                (borrow, borrowDetail) => new { borrow, borrowDetail })
                .Where(x => x.borrowDetail.IsActive == true && x.borrow.IsReturned == false)
                .Select(x => new ConsolidateFinanceReportDto
                {
                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrowDetail.PreparedDate.Date,
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = Math.Round(x.borrowDetail.Quantity, 2),
                    UnitCost = x.borrowDetail.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.borrowDetail.UnitPrice * x.borrowDetail.Quantity, 2)).ToString(),
                    Source = Convert.ToString(x.borrow.Id),
                    TransactionType = "Borrow",
                    Reason = x.borrow.Remarks,
                    Reference = x.borrow.Details,
                    SupplierName = "",
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = "",
                    CompanyName = "",
                    DepartmentCode = "",
                    DepartmentName = "",
                    LocationCode = "",
                    LocationName = "",
                    AccountTitleCode = "",
                    AccountTitle = "",
                    EmpId = "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    //Remarks = x.borrow.Remarks,
                    Rush = ""


                });

            var consumeList = _context.BorrowedConsumes
                .Where(x => x.IsActive == true)
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.Id,
                    BorrowedId = x.BorrowedItemPkey,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Uom = x.Uom,
                    Consumed = x.Consume,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    EmpId = x.EmpId,
                    FullName = x.FullName,
                    ReportNumber = x.ReportNumber,
                    OneChargingName = x.OneChargingName,
                    BusinessUnitName = x.BusinessUnitName,
                    BusinessUnitCode = x.BusinessUnitCode,
                    DepartmentUnitName = x.DepartmentUnitName,
                    DepartmentUnitCode = x.DepartmentUnitCode,
                    SubUnitCode = x.SubUnitCode,
                    SubUnitName = x.SubUnitName

                });

            var returnList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                .GroupJoin(consumeList, borrowDetails => borrowDetails.Id, consume => consume.BorrowedId
                , (borrowDetails, consume) => new { borrowDetails, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrowDetails, consume })
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.borrowDetails.Id,
                    BorrowedId = x.borrowDetails.BorrowedPKey,
                    ItemCode = x.borrowDetails.ItemCode,
                    ItemDescription = x.borrowDetails.ItemDescription,
                    Uom = x.borrowDetails.Uom,
                    BorrowedQuantity = x.borrowDetails.Quantity != null ? x.borrowDetails.Quantity : 0,
                    Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                    CompanyCode = x.consume.CompanyCode,
                    CompanyName = x.consume.CompanyName,
                    DepartmentCode = x.consume.DepartmentCode,
                    DepartmentName = x.consume.DepartmentName,
                    LocationCode = x.consume.LocationCode,
                    LocationName = x.consume.LocationName,
                    AccountCode = x.consume.AccountCode,
                    AccountTitles = x.consume.AccountTitles,
                    EmpId = x.consume.EmpId,
                    FullName = x.consume.FullName,
                    ReportNumber = x.consume.ReportNumber,
                    UnitPrice = x.borrowDetails.UnitPrice,
                    OneChargingName = x.consume.OneChargingName,
                    BusinessUnitName = x.consume.BusinessUnitName,
                    BusinessUnitCode = x.consume.BusinessUnitCode,
                    DepartmentUnitName = x.consume.DepartmentUnitName,
                    DepartmentUnitCode = x.consume.DepartmentUnitCode,
                    SubUnitCode = x.consume.SubUnitCode,
                    SubUnitName = x.consume.SubUnitName

                });

            var borrowedIssueList = _context.BorrowedIssues
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.IsReturned == true);

            var returnedConsol = returnList
                .GroupJoin(borrowedIssueList, borrowDetail => borrowDetail.BorrowedId, borrow => borrow.Id,
                (borrowDetail, borrow) => new { borrowDetail, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.borrowDetail, borrow })
                .Select(x => new ConsolidateFinanceReportDto
                {

                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrow.PreparedDate.Date,
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed,
                    UnitCost = x.borrowDetail.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.borrowDetail.UnitPrice.Value * x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed, 2)).ToString(),
                    Source = Convert.ToString(x.borrow.Id),
                    TransactionType = "Returned",
                    Reason = x.borrowDetail.Remarks,
                    Reference = x.borrowDetail.Details,
                    SupplierName = "",
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = x.borrowDetail.CompanyCode,
                    CompanyName = x.borrowDetail.CompanyName,
                    DepartmentCode = x.borrowDetail.DepartmentCode,
                    DepartmentName = x.borrowDetail.DepartmentName,
                    LocationCode = x.borrowDetail.LocationCode,
                    LocationName = x.borrowDetail.LocationName,
                    AccountTitleCode = x.borrowDetail.AccountCode,
                    AccountTitle = x.borrowDetail.AccountTitles,
                    EmpId = x.borrowDetail.EmpId,
                    Fullname = x.borrowDetail.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.borrowDetail.OneChargingName,
                    BusinessUnitName = x.borrowDetail.BusinessUnitName,
                    BusinessUnitCode = x.borrowDetail.BusinessUnitCode,
                    DepartmentUnitName = x.borrowDetail.DepartmentUnitName,
                    DepartmentUnitCode = x.borrowDetail.DepartmentUnitCode,
                    SubUnitCode = x.borrowDetail.SubUnitCode,
                    SubUnitName = x.borrowDetail.SubUnitName

                });


            var fuelRegisterConsol = _context.FuelRegisterDetails
                .Include(m => m.Material)
                .ThenInclude(id => id.ItemCategory)
                .Include(w => w.Warehouse_Receiving)
                .GroupJoin(_context.OneAccountTitles, fuel => fuel.FuelRegister.Account_Title_Code, accountTitle => accountTitle.AccountCode, (fuel, accountTitle) => new { fuel, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.fuel, accountTitle })
                .Where(r => r.fuel.FuelRegister.Is_Transact == true)
                .Select(x => new ConsolidateFinanceReportDto
                {

                    Id = x.fuel.Id,
                    TransactionDate = x.fuel.FuelRegister.issuanceDate.Value.Date,
                    ItemCode = x.fuel.Material.ItemCode,
                    ItemDescription = x.fuel.Material.ItemDescription,
                    Uom = x.fuel.Material.Uom.UomCode,
                    Category = "GASOLINE, OIL, LUBRICANT",
                    Quantity = x.fuel.Liters != null ? x.fuel.Liters : 0,
                    UnitCost = (x.fuel.Warehouse_Receiving.PriceWithDecimal == null ? x.fuel.Warehouse_Receiving.UnitPrice : decimal.Parse(x.fuel.Warehouse_Receiving.PriceWithDecimal)).ToString(),
                    LineAmount = (x.fuel.Warehouse_Receiving.PriceWithDecimal == null ? x.fuel.Warehouse_Receiving.UnitPrice * x.fuel.Liters.Value : decimal.Parse(x.fuel.Warehouse_Receiving.PriceWithDecimal) * x.fuel.Liters.Value).ToString(),
                    Source = Convert.ToString(x.fuel.FuelRegisterId),
                    TransactionType = "Fuel Register",
                    Reason = "",
                    Reference = x.fuel.FuelRegister.Remarks,
                    SupplierName = x.fuel.FuelRegister.Company_Name,
                    EncodedBy = x.fuel.FuelRegister.Added_By,
                    CompanyCode = x.fuel.FuelRegister.Company_Code,
                    CompanyName = x.fuel.FuelRegister.Company_Name,
                    DepartmentCode = x.fuel.FuelRegister.Department_Code,
                    DepartmentName = x.fuel.FuelRegister.Department_Name,
                    LocationCode = x.fuel.FuelRegister.Location_Code,
                    LocationName = x.fuel.FuelRegister.Location_Name,
                    AccountTitleCode = x.fuel.FuelRegister.Account_Title_Code,
                    AccountTitle = x.fuel.FuelRegister.Account_Title_Name,
                    EmpId = x.fuel.FuelRegister.EmpId,
                    Fullname = x.fuel.FuelRegister.Fullname,
                    AssetTag = x.fuel.FuelRegister.Asset.AssetCode,
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.fuel.FuelRegister.OneChargingName,
                    BusinessUnitName = x.fuel.FuelRegister.BusinessUnitName,
                    BusinessUnitCode = x.fuel.FuelRegister.BusinessUnitCode,
                    DepartmentUnitName = x.fuel.FuelRegister.DepartmentUnitName,
                    DepartmentUnitCode = x.fuel.FuelRegister.DepartmentUnitCode,
                    SubUnitCode = x.fuel.FuelRegister.SubUnitCode,
                    SubUnitName = x.fuel.FuelRegister.SubUnitName,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,
                    DieselPoNumber = x.fuel.dieselPONumber,

                });


            if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
            {
                var dateFrom = DateTime.Parse(DateFrom).Date;
                var dateTo = DateTime.Parse(DateTo).Date;

                receivingConsol = receivingConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    .ToList()
                    ;

                moveOrderConsol = moveOrderConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

                receiptConsol = receiptConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

                issueConsol = issueConsol
                     .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

                borrowedConsol = borrowedConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

                returnedConsol = returnedConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

                fuelRegisterConsol = fuelRegisterConsol
                    .Where(x => x.TransactionDate.Date >= dateFrom && x.TransactionDate.Date <= dateTo)
                    ;

            }

            var consolidateList = receivingConsol
                .Concat(await moveOrderConsol.ToListAsync())
                .Concat(await receiptConsol.ToListAsync())
                .Concat(await issueConsol.ToListAsync())
                .Concat(await borrowedConsol.ToListAsync())
                .Concat(await returnedConsol.ToListAsync())
                .Concat(await fuelRegisterConsol.ToListAsync());







            //        var reports = consolidateList.GroupJoin(materials, consol => consol.ItemCode, material => material.ItemCode, (consol, material) => new { consol, material })
            //.SelectMany(x => x.material.DefaultIfEmpty(), (x, material) => new { x.consol, material })
            //.Select(x => new ConsolidateFinanceReportDto
            //{
            //    Id = x.consol.Id,
            //    TransactionDate = x.consol.TransactionDate,
            //    ItemCode = x.consol.ItemCode,
            //    ItemDescription = x.consol.ItemDescription,
            //    Uom = x.consol.Uom,
            //    Category = x.material.ItemCategory.ItemCategoryName,
            //    Quantity = x.consol.Quantity,
            //    UnitCost = x.consol.UnitCost,
            //    LineAmount = x.consol.LineAmount,
            //    Source = x.consol.Source,
            //    TransactionType = x.consol.TransactionType,
            //    Reason = x.consol.Reason,
            //    Reference = x.consol.Reference,
            //    SupplierName = x.consol.SupplierName,
            //    EncodedBy = x.consol.EncodedBy,
            //    EmpId = x.consol.EmpId,
            //    Fullname = x.consol.Fullname,
            //    AssetTag = x.consol.AssetTag,
            //    CIPNo = x.consol.CIPNo,
            //    Helpdesk = x.consol.Helpdesk,
            //    Rush = x.consol.Rush,

            //});


            var reports = consolidateList
                .Select(consol => new ConsolidateFinanceReportDto
                {
                    Id = consol.Id,
                    TransactionDate = consol.TransactionDate,
                    ItemCode = consol.ItemCode,
                    ItemDescription = consol.ItemDescription,
                    Uom = consol.Uom,
                    Category = consol.Category,
                    Quantity = consol.Quantity,
                    UnitCost = consol.UnitCost,
                    LineAmount = consol.LineAmount,
                    Source = consol.Source,
                    TransactionType = consol.TransactionType,
                    Reason = consol.Reason,
                    Reference = consol.Reference,
                    SupplierName = consol.SupplierName,
                    EncodedBy = consol.EncodedBy,
                    CompanyCode = consol.CompanyCode,
                    CompanyName = consol.CompanyName,
                    DepartmentCode = consol.DepartmentCode,
                    DepartmentName = consol.DepartmentName,
                    LocationCode = consol.LocationCode,
                    LocationName = consol.LocationName,
                    AccountTitleCode = consol.AccountTitleCode,
                    AccountTitle = consol.AccountTitle,
                    EmpId = consol.EmpId,
                    Fullname = consol.Fullname,
                    AssetTag = consol.AssetTag,
                    CIPNo = consol.CIPNo,
                    Helpdesk = consol.Helpdesk,
                    Rush = consol.Rush,
                    OneChargingName = consol.OneChargingName,
                    BusinessUnitName = consol.BusinessUnitName,
                    BusinessUnitCode = consol.BusinessUnitCode,
                    DepartmentUnitName = consol.DepartmentUnitName,
                    DepartmentUnitCode = consol.DepartmentUnitCode,
                    SubUnitCode = consol.SubUnitCode,
                    SubUnitName = consol.SubUnitName,
                    FinancialStatement = consol.FinancialStatement,
                    UnitResponsible = consol.UnitResponsible,
                    DieselPoNumber = consol.DieselPoNumber

                });



            if (!string.IsNullOrEmpty(Search))
            {
                reports = reports.Where(x => x.ItemCode.ToLower().Contains(Search.ToLower())
                || x.ItemDescription.ToLower().Contains(Search.ToLower())
                || x.Source.ToString().Contains(Search)
                || x.TransactionType.ToLower().Contains(Search.ToLower()))
                      ;
            }

            reports = reports
                .OrderBy(x => x.TransactionDate.Date)
                .ThenBy(x => x.ItemCode);




            return reports.ToList();
        }

        public async Task<IReadOnlyList<ConsolidateAuditReportDto>> ConsolidateAuditReport(string DateFrom, string DateTo, string Search)
        {

            var dateFrom = DateTime.Parse(DateFrom).Date;
            var dateTo = DateTime.Parse(DateTo).Date;

           

            var materials = _context.Materials
                .AsNoTracking()
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory).Where(x => x.IsActive == true);

            var receivingConsol = _context.WarehouseReceived
                .AsNoTracking()
                .Join(materials, ware => ware.ItemCode, material => material.ItemCode, (ware, material) => new { ware, material })
                .Where(x => x.ware.TransactionType == "Receiving" && x.ware.IsActive == true)
                .Where(x => x.ware.ActualReceivingDate.Date >= dateFrom && x.ware.ActualReceivingDate.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {
                    Id = x.ware.Id,
                    TransactionDate = x.ware.ActualReceivingDate.Date.ToString(),
                    ItemCode = x.ware.ItemCode,
                    ItemDescription = x.ware.ItemDescription,
                    Uom = x.ware.Uom,
                    Category = x.material.ItemCategory.ItemCategoryName,
                    Quantity = x.ware.ActualGood,
                    UnitCost = (x.ware.PriceWithDecimal == null ? x.ware.UnitPrice : decimal.Parse(x.ware.PriceWithDecimal)).ToString(),
                    LineAmount = (x.ware.PriceWithDecimal == null ? x.ware.UnitPrice * x.ware.ActualGood : decimal.Parse(x.ware.PriceWithDecimal) * x.ware.ActualGood).ToString(),
                    Source = x.ware.PoNumber,
                    TransactionType = "Receiving",
                    Reason = "",
                    Reference = x.ware.SINumber,
                    SupplierName = x.ware.Supplier,
                    EncodedBy = x.ware.AddedBy,
                    CompanyCode = "01",
                    CompanyName = "RDF",
                    DepartmentCode = "0010",
                    DepartmentName = "Corporate Common",
                    LocationCode = "0001",
                    LocationName = "Head Office",
                    AccountTitleCode = "117701",
                    AccountTitle = "Materials & Supplies Inventory",
                    EmpId = "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = ""

                }).ToList();

           


            var moveOrderConsol = _context.MoveOrders
                .AsNoTracking()
                .GroupJoin(_context.TransactOrder, moveOrder => moveOrder.OrderNo, transMoveOrder => transMoveOrder.OrderNo, (moveOrder, transMoveOrder) => new { moveOrder, transMoveOrder })
                .SelectMany(x => x.transMoveOrder.DefaultIfEmpty(), (x, transMoveOrder) => new { x.moveOrder, transMoveOrder })
                .GroupJoin(_context.Users, trans => trans.transMoveOrder.PreparedBy, user => user.FullName, (trans, user) => new { trans, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.trans.moveOrder, x.trans.transMoveOrder, user })
                .GroupJoin(_context.OneAccountTitles, user => user.moveOrder.AccountCode, account => account.AccountCode, (user, account) => new { user, account })
                .SelectMany(x => x.account.DefaultIfEmpty(), (x, account) => new { x.user.moveOrder, x.user.transMoveOrder, x.user.user, account })
                .GroupJoin(materials, account => account.moveOrder.ItemCode, material => material.ItemCode, (account, material) => new { account, material })
                .SelectMany(x => x.material.DefaultIfEmpty(), (x, material) => new { x.account.moveOrder, x.account.transMoveOrder, x.account.user, x.account.account, material })


                .Where(x => x.moveOrder.IsTransact == true)
                .Where(x => x.transMoveOrder.PreparedDate.Value.Date >= dateFrom && x.transMoveOrder.PreparedDate.Value.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {
                    Id = x.moveOrder.Id,
                    TransactionDate = x.transMoveOrder.PreparedDate.Value.Date.ToString(),
                    ItemCode = x.moveOrder.ItemCode,
                    ItemDescription = x.moveOrder.ItemDescription,
                    Uom = x.material.Uom.UomCode,
                    Category = x.moveOrder.Category,
                    Quantity = Math.Round(x.moveOrder.QuantityOrdered, 2),
                    UnitCost = x.moveOrder.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.moveOrder.UnitPrice * x.moveOrder.QuantityOrdered, 2)).ToString(),
                    Source = Convert.ToString(x.transMoveOrder.OrderNo),
                    TransactionType = "Move Order",
                    Reason = "",
                    Reference = x.moveOrder.ItemRemarks,
                    SupplierName = "",
                    EncodedBy = x.transMoveOrder.PreparedBy,
                    CompanyCode = x.moveOrder.CompanyCode,
                    CompanyName = x.moveOrder.CompanyName,
                    ServiceProvider = x.user.FullName,
                    ServiceProviderCode = x.user.EmpId,
                    DepartmentCode = x.moveOrder.DepartmentCode,
                    DepartmentName = x.moveOrder.DepartmentName,
                    LocationCode = x.moveOrder.LocationCode,
                    LocationName = x.moveOrder.LocationName,
                    AccountTitleCode = x.moveOrder.AccountCode != null ? x.moveOrder.AccountCode : "537620",
                    AccountTitle = x.moveOrder.AccountTitles != null ? x.moveOrder.AccountTitles : "SE - R & M - Transport Vehicles",
                    EmpId = x.moveOrder.EmpId,
                    Fullname = x.moveOrder.FullName,
                    AssetTag = x.moveOrder.AssetTag,
                    CIPNo = x.moveOrder.Cip_No,
                    Helpdesk = x.moveOrder.HelpdeskNo,
                    Rush = x.moveOrder.Rush,
                    OneChargingName = x.moveOrder.one_charging_name,
                    BusinessUnitName = x.moveOrder.business_unit_name,
                    BusinessUnitCode = x.moveOrder.business_unit_code,
                    DepartmentUnitName = x.moveOrder.department_unit_name,
                    DepartmentUnitCode = x.moveOrder.department_unit_code,
                    SubUnitCode = x.moveOrder.sub_unit_code,
                    SubUnitName = x.moveOrder.sub_unit_name,
                    FinancialStatement = x.account.FinancialStatement,
                    UnitResponsible = x.account.Unit,
                }).ToList();
            //

            
            var receiptConsol = _context.MiscellaneousReceipts
                .AsNoTracking()
                .GroupJoin(_context.WarehouseReceived, receipt => receipt.Id, warehouse => warehouse.MiscellaneousReceiptId, (receipt, warehouse) => new { receipt, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.receipt, warehouse }).Join(_context.Users, x => x.receipt.PreparedBy, user => user.FullName,
            (x, user) => new { x.receipt, x.warehouse, user })
                .GroupJoin(_context.OneAccountTitles, user => user.warehouse.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.receipt, x.user.warehouse, x.user.user, accountTitle })
               .Join(materials, account => account.warehouse.ItemCode, material => material.ItemCode, (x, material) => new { x.receipt, x.warehouse, x.user, x.accountTitle, material })
                .Where(x => x.warehouse.IsActive == true && x.warehouse.TransactionType == "MiscellaneousReceipt")
                .Where(x => x.receipt.TransactionDate.Date >= dateFrom && x.receipt.TransactionDate.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {
                    Id = x.warehouse.Id,
                    TransactionDate = x.receipt.TransactionDate.ToString(),
                    ItemCode = x.warehouse.ItemCode,
                    ItemDescription = x.warehouse.ItemDescription,
                    Uom = x.warehouse.Uom,
                    Category = x.material.ItemCategoryName,
                    Quantity = x.warehouse.ActualGood,
                    UnitCost = (x.warehouse.PriceWithDecimal == null ? x.warehouse.UnitPrice : decimal.Parse(x.warehouse.PriceWithDecimal)).ToString(),
                    LineAmount = (x.warehouse.PriceWithDecimal == null ? x.warehouse.UnitPrice * x.warehouse.ActualGood : decimal.Parse(x.warehouse.PriceWithDecimal) * x.warehouse.ActualGood).ToString(),
                    Source = Convert.ToString(x.receipt.Id),
                    TransactionType = "Miscellaneous Receipt",
                    Reason = x.receipt.Remarks,
                    Reference = x.receipt.Details,
                    SupplierName = x.receipt.supplier,
                    EncodedBy = x.receipt.PreparedBy,
                    CompanyCode = x.receipt.CompanyCode,
                    CompanyName = x.receipt.CompanyName,
                    DepartmentCode = x.receipt.DepartmentCode,
                    DepartmentName = x.receipt.DepartmentName,
                    LocationCode = x.receipt.LocationCode,
                    LocationName = x.receipt.LocationName,
                    AccountTitleCode = x.warehouse.AccountCode,
                    AccountTitle = x.warehouse.AccountTitles,
                    EmpId = x.warehouse.EmpId,
                    Fullname = x.warehouse.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    //Remarks = x.receipt.Remarks,
                    Rush = "",
                    OneChargingName = x.receipt.one_charging_name,
                    BusinessUnitName = x.receipt.business_unit_name,
                    BusinessUnitCode = x.receipt.business_unit_code,
                    DepartmentUnitName = x.receipt.department_unit_name,
                    DepartmentUnitCode = x.receipt.department_unit_code,
                    SubUnitCode = x.receipt.sub_unit_code,
                    SubUnitName = x.receipt.sub_unit_name,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,
                }).Distinct().ToList();

            

            var issueConsol = _context.MiscellaneousIssues
                .AsNoTracking()
                .Join(_context.MiscellaneousIssueDetail, miscDatail => miscDatail.Id, issue => issue.IssuePKey,
                (miscDetail, issue) => new { miscDetail, issue }).Join(_context.Users, x => x.miscDetail.PreparedBy, user => user.FullName,
            (x, user) => new { x.miscDetail, x.issue, user })
                .GroupJoin(_context.OneAccountTitles, user => user.issue.AccountCode, accountTitle => accountTitle.AccountCode, (user, accountTitle) => new { user, accountTitle })
                .SelectMany(x => x.accountTitle.DefaultIfEmpty(), (x, accountTitle) => new { x.user.miscDetail, x.user.issue, x.user.user, accountTitle })
                .Join(materials, account => account.issue.ItemCode, material => material.ItemCode, (x, material) => new { x.miscDetail, x.issue, x.user, x.accountTitle, material })
                .Where(x => x.issue.IsActive == true)
                .Where(x => x.miscDetail.TransactionDate.Date >= dateFrom && x.miscDetail.TransactionDate.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {
                    Id = x.issue.Id,
                    TransactionDate = x.miscDetail.TransactionDate.Date.ToString(),
                    ItemCode = x.issue.ItemCode,
                    ItemDescription = x.issue.ItemDescription,
                    Uom = x.issue.Uom,
                    Category = x.material.ItemCategoryName,
                    Quantity = Math.Round(x.issue.Quantity, 2),
                    UnitCost = x.issue.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.issue.UnitPrice * x.issue.Quantity, 2)).ToString(),
                    Source = Convert.ToString(x.issue.Id),
                    TransactionType = "Miscellaneous Issue",
                    Reason = x.issue.Remarks,
                    Reference = x.miscDetail.Details,
                    SupplierName = "",
                    EncodedBy = x.issue.PreparedBy,
                    CompanyCode = x.miscDetail.CompanyCode,
                    CompanyName = x.miscDetail.CompanyName,
                    DepartmentCode = x.miscDetail.DepartmentCode,
                    DepartmentName = x.miscDetail.DepartmentName,
                    LocationCode = x.miscDetail.LocationCode,
                    LocationName = x.miscDetail.LocationName,
                    AccountTitleCode = x.issue.AccountCode,
                    AccountTitle = x.issue.AccountTitles,
                    EmpId = x.issue.EmpId,
                    Fullname = x.issue.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.miscDetail.one_charging_name,
                    BusinessUnitName = x.miscDetail.business_unit_name,
                    BusinessUnitCode = x.miscDetail.business_unit_code,
                    DepartmentUnitName = x.miscDetail.department_unit_name,
                    DepartmentUnitCode = x.miscDetail.department_unit_code,
                    SubUnitCode = x.miscDetail.sub_unit_code,
                    SubUnitName = x.miscDetail.sub_unit_name,
                    FinancialStatement = x.accountTitle.FinancialStatement,
                    UnitResponsible = x.accountTitle.Unit,

                }).Distinct().ToList();

            var borrowedConsol = _context.BorrowedIssues
                .Join(_context.BorrowedIssueDetails, borrow => borrow.Id, borrowDetail => borrowDetail.BorrowedPKey,
                (borrow, borrowDetail) => new { borrow, borrowDetail })
                .Where(x => x.borrowDetail.IsActive == true)
                .Where(x => x.borrowDetail.PreparedDate.Date >= dateFrom && x.borrowDetail.PreparedDate.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {

                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrowDetail.PreparedDate.Date.ToString(),
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = Math.Round(x.borrowDetail.Quantity, 2),
                    UnitCost = x.borrowDetail.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.borrowDetail.UnitPrice * x.borrowDetail.Quantity, 2)).ToString(),
                    Source = x.borrow.Id.ToString(),
                    TransactionType = "Borrow",
                    Status = "",
                    Reason = x.borrow.Remarks,
                    Reference = x.borrow.Details,
                    SupplierName = "",
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = "",
                    CompanyName = "",
                    DepartmentCode = "",
                    DepartmentName = "",
                    LocationCode = "",
                    LocationName = "",
                    AccountTitleCode = "",
                    AccountTitle = "",
                    EmpId = "",
                    Fullname = "",
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = ""

                }).ToList();



            var consumeList = _context.BorrowedConsumes
                .Where(x => x.IsActive == true)
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.Id,
                    BorrowedId = x.BorrowedItemPkey,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Uom = x.Uom,
                    Consumed = x.Consume,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    EmpId = x.EmpId,
                    FullName = x.FullName,
                    ReportNumber = x.ReportNumber,
                    OneChargingName = x.OneChargingName,
                    BusinessUnitName = x.BusinessUnitName,
                    BusinessUnitCode = x.BusinessUnitCode,
                    DepartmentUnitName = x.DepartmentUnitName,
                    DepartmentUnitCode = x.DepartmentUnitCode,
                    SubUnitCode = x.SubUnitCode,
                    SubUnitName = x.SubUnitName

                });

            var returnList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                .GroupJoin(consumeList, borrowDetails => borrowDetails.Id, consume => consume.BorrowedId
                , (borrowDetails, consume) => new { borrowDetails, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrowDetails, consume })
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.borrowDetails.Id,
                    BorrowedId = x.borrowDetails.BorrowedPKey,
                    ItemCode = x.borrowDetails.ItemCode,
                    ItemDescription = x.borrowDetails.ItemDescription,
                    Uom = x.borrowDetails.Uom,
                    BorrowedQuantity = x.borrowDetails.Quantity != null ? x.borrowDetails.Quantity : 0,
                    Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                    CompanyCode = x.consume.CompanyCode,
                    CompanyName = x.consume.CompanyName,
                    DepartmentCode = x.consume.DepartmentCode,
                    DepartmentName = x.consume.DepartmentName,
                    LocationCode = x.consume.LocationCode,
                    LocationName = x.consume.LocationName,
                    AccountCode = x.consume.AccountCode,
                    AccountTitles = x.consume.AccountTitles,
                    EmpId = x.consume.EmpId,
                    FullName = x.consume.FullName,
                    ReportNumber = x.consume.ReportNumber,
                    UnitPrice = x.borrowDetails.UnitPrice,
                    OneChargingName = x.consume.OneChargingName,
                    BusinessUnitName = x.consume.BusinessUnitName,
                    BusinessUnitCode = x.consume.BusinessUnitCode,
                    DepartmentUnitName = x.consume.DepartmentUnitName,
                    DepartmentUnitCode = x.consume.DepartmentUnitCode,
                    SubUnitCode = x.consume.SubUnitCode,
                    SubUnitName = x.consume.SubUnitName

                });

            var borrowedIssueList = _context.BorrowedIssues
                .Where(x => x.IsActive == true);

            var returnedConsol = returnList
                .GroupJoin(borrowedIssueList, borrowDetail => borrowDetail.BorrowedId, borrow => borrow.Id,
                (borrowDetail, borrow) => new { borrowDetail, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.borrowDetail, borrow })
                .Where(x => x.borrow.PreparedDate.Date >= dateFrom && x.borrow.PreparedDate.Date <= dateTo)
                .Select(x => new ConsolidateAuditReportDto
                {

                    Id = x.borrowDetail.Id,
                    TransactionDate = x.borrow.PreparedDate.Date.ToString(),
                    ItemCode = x.borrowDetail.ItemCode,
                    ItemDescription = x.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.Uom,
                    Category = "",
                    Quantity = x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed,
                    UnitCost = x.borrowDetail.UnitPrice.ToString(),
                    LineAmount = (Math.Round(x.borrowDetail.UnitPrice.Value * x.borrowDetail.BorrowedQuantity - x.borrowDetail.Consumed, 2)).ToString(),
                    Source = x.borrow.Id.ToString(),
                    TransactionType = "Returned",
                    Status = "",
                    Reason = x.borrowDetail.Remarks,
                    Reference = x.borrowDetail.Details,
                    SupplierName = "",
                    EncodedBy = x.borrow.PreparedBy,
                    CompanyCode = x.borrowDetail.CompanyCode,
                    CompanyName = x.borrowDetail.CompanyName,
                    DepartmentCode = x.borrowDetail.DepartmentCode,
                    DepartmentName = x.borrowDetail.DepartmentName,
                    LocationCode = x.borrowDetail.LocationCode,
                    LocationName = x.borrowDetail.LocationName,
                    AccountTitleCode = x.borrowDetail.AccountCode,
                    AccountTitle = x.borrowDetail.AccountTitles,
                    EmpId = x.borrowDetail.EmpId,
                    Fullname = x.borrowDetail.FullName,
                    AssetTag = "",
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.borrowDetail.OneChargingName,
                    BusinessUnitName = x.borrowDetail.BusinessUnitName,
                    BusinessUnitCode = x.borrowDetail.BusinessUnitCode,
                    DepartmentUnitName = x.borrowDetail.DepartmentUnitName,
                    DepartmentUnitCode = x.borrowDetail.DepartmentUnitCode,
                    SubUnitCode = x.borrowDetail.SubUnitCode,
                    SubUnitName = x.borrowDetail.SubUnitName

                }).ToList();



            var cancelledConsol = _context.Orders
                 .Where(x => x.IsCancel == true || x.StandartQuantity != x.QuantityOrdered)
                .Where(x => (x.CancelDate.Value.Date >= DateTime.Parse(DateFrom).Date
                       && x.CancelDate.Value.Date <= DateTime.Parse(DateTo).Date)
                       || (x.Modified_Date != null && x.Modified_Date.Value.Date >= DateTime.Parse(DateFrom).Date
                && x.Modified_Date.Value.Date <= DateTime.Parse(DateTo).Date))

                .Select(x => new ConsolidateAuditReportDto
                {
                    Id = x.Id,
                    TransactionDate = x.CancelDate.Value.Date.ToString() != null ? x.CancelDate.Value.Date.ToString() : x.Modified_Date.Value.Date.ToString(),
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemdDescription,
                    Uom = x.Uom,
                    Category = "",
                    Quantity = x.StandartQuantity != x.QuantityOrdered ? x.StandartQuantity - x.QuantityOrdered : x.StandartQuantity,
                    UnitCost = "",
                    LineAmount = "",
                    Source = x.TrasactId.ToString(),
                    TransactionType = "",
                    Status = "Cancelled",
                    Reason = x.Remarks,
                    Reference = "",
                    SupplierName = "",
                    EncodedBy = x.PreparedBy,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.Department,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountTitleCode = x.AccountCode,
                    AccountTitle = x.AccountTitles,
                    EmpId = x.EmpId,
                    Fullname = x.FullName,
                    AssetTag = x.AssetTag,
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = x.Rush,
                    OneChargingName = x.one_charging_name,
                    BusinessUnitName = x.business_unit_name,
                    BusinessUnitCode = x.business_unit_code,
                    DepartmentUnitName = x.department_unit_name,
                    DepartmentUnitCode = x.department_unit_code,
                    SubUnitCode = x.sub_unit_code,
                    SubUnitName = x.sub_unit_name

                }).ToList();

            var fuelRegisterConsol = _context.FuelRegisterDetails
                .Include(m => m.Material)
                .ThenInclude(id => id.ItemCategory)
                .Include(m => m.FuelRegister)
                .Include(w => w.Warehouse_Receiving)
                .Where(r => r.FuelRegister.Is_Transact == true)
                .Select(x => new ConsolidateAuditReportDto
                {

                    Id = x.Id,
                    TransactionDate = x.FuelRegister.Transact_At.Value.Date.ToString(),
                    ItemCode = x.Material.ItemCode,
                    ItemDescription = x.Material.ItemDescription,
                    Uom = x.Material.Uom.UomCode,
                    Category = "",
                    Quantity = x.Liters,
                    UnitCost = (x.Warehouse_Receiving.PriceWithDecimal == null ? x.Warehouse_Receiving.UnitPrice :  decimal.Parse(x.Warehouse_Receiving.PriceWithDecimal)).ToString(),
                    LineAmount = (x.Warehouse_Receiving.PriceWithDecimal == null ? x.Warehouse_Receiving.UnitPrice * x.Liters.Value : decimal.Parse(x.Warehouse_Receiving.PriceWithDecimal) * x.Liters.Value).ToString(),
                    Source = Convert.ToString(x.FuelRegisterId),
                    TransactionType = "Fuel Register",
                    Reason = x.FuelRegister.Remarks,
                    Reference = "",
                    SupplierName = "",
                    EncodedBy = x.FuelRegister.Transact_By,
                    CompanyCode = x.FuelRegister.Company_Code,
                    CompanyName = x.FuelRegister.Company_Name,
                    DepartmentCode = x.FuelRegister.Department_Code,
                    DepartmentName = x.FuelRegister.Department_Name,
                    LocationCode = x.FuelRegister.Location_Code,
                    LocationName = x.FuelRegister.Location_Name,
                    AccountTitleCode = x.FuelRegister.Account_Title_Code,
                    AccountTitle = x.FuelRegister.Account_Title_Name,
                    EmpId = x.FuelRegister.EmpId,
                    Fullname = x.FuelRegister.Fullname,
                    AssetTag = x.FuelRegister.Asset.AssetCode,
                    CIPNo = "",
                    Helpdesk = "",
                    Rush = "",
                    OneChargingName = x.FuelRegister.OneChargingName,
                    BusinessUnitName = x.FuelRegister.BusinessUnitName,
                    BusinessUnitCode = x.FuelRegister.BusinessUnitCode,
                    DepartmentUnitName = x.FuelRegister.DepartmentUnitName,
                    DepartmentUnitCode = x.FuelRegister.DepartmentUnitCode,
                    SubUnitCode = x.FuelRegister.SubUnitCode,
                    SubUnitName = x.FuelRegister.SubUnitName

                });


            var consolidateList = receivingConsol
                .Concat(moveOrderConsol)
                .Concat(receiptConsol)
                .Concat(issueConsol)
                .Concat(borrowedConsol)
                .Concat(returnedConsol)
                .Concat(cancelledConsol)
                .Concat(fuelRegisterConsol)
                .ToList();

           

            var reports = consolidateList
                .Join(materials,
                 consol => consol.ItemCode, material => material.ItemCode,
                 (consol, material) => new ConsolidateAuditReportDto
                 {
                     Id = consol.Id,
                     TransactionDate = consol.TransactionDate,
                     ItemCode = consol.ItemCode,
                     ItemDescription = consol.ItemDescription,
                     Uom = consol.Uom,
                     Category = consol.Category,
                     Quantity = consol.Quantity,
                     UnitCost = consol.UnitCost,
                     LineAmount = consol.LineAmount,
                     Source = consol.Source,
                     TransactionType = consol.TransactionType,
                     Status = consol.Status,
                     Reason = consol.Reason,
                     Reference = consol.Reference,
                     SupplierName = consol.SupplierName,
                     EncodedBy = consol.EncodedBy,
                     CompanyCode = consol.CompanyCode,
                     CompanyName = consol.CompanyName,
                     DepartmentCode = consol.DepartmentCode,
                     DepartmentName = consol.DepartmentName,
                     LocationCode = consol.LocationCode,
                     LocationName = consol.LocationName,
                     AccountTitleCode = consol.AccountTitleCode,
                     AccountTitle = consol.AccountTitle,
                     EmpId = consol.EmpId,
                     Fullname = consol.Fullname,
                     AssetTag = consol.AssetTag,
                     CIPNo = consol.CIPNo,
                     Helpdesk = consol.Helpdesk,
                     Rush = consol.Rush,
                     OneChargingName = consol.OneChargingName,
                     BusinessUnitName = consol.BusinessUnitName,
                     BusinessUnitCode = consol.BusinessUnitCode,
                     DepartmentUnitName = consol.DepartmentUnitName,
                     DepartmentUnitCode = consol.DepartmentUnitCode,
                     SubUnitCode = consol.SubUnitCode,
                     SubUnitName = consol.SubUnitName

                 }).ToList();

            if (!string.IsNullOrEmpty(Search))
            {
                reports = reports.Where(x => x.ItemCode.ToLower().Contains(Search.ToLower())
                || x.ItemDescription.ToLower().Contains(Search.ToLower())
                || x.Source.ToString().Contains(Search)
                || x.TransactionType.ToLower().Contains(Search.ToLower())
                || x.Status.ToLower().Contains(Search.ToLower()))
                    .ToList();
            }

            reports = reports
                .OrderBy(x => x.TransactionDate)
                .ToList();


            return reports;

        }

        public async Task<IReadOnlyList<GeneralLedgerReportDto>> GeneralLedgerReport(string DateFrom, string DateTo)
        {

            var userList = _context.Users
                .Where(x => x.IsActive == true)
                .Select(x => new
                {
                    x.Id,
                    x.FullName,
                    x.EmpId

                });


            var moveOrderConsol = _context.TransactOrder
                .AsNoTracking()
                .GroupJoin(_context.MoveOrders, transact => transact.OrderNo,
                moveOrder => moveOrder.OrderNo, (transact, moveOrder) => new { transact, moveOrder })
                .SelectMany(x => x.moveOrder.DefaultIfEmpty(), (x, moveOrder) => new { x.transact, moveOrder })
                .GroupJoin(userList, transact => transact.transact.PreparedBy, user => user.FullName, (transact, user) => new { transact, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.transact, user })
               .Where(x => x.transact.transact.IsTransact == true && x.transact.transact.IsActive == true && x.transact.moveOrder.IsActive == true)
                .Select(x => new GeneralLedgerReportDto
                {
                    SyncId = x.transact.moveOrder.Id,
                    Transaction_Date = x.transact.transact.PreparedDate.Value,
                    Item_Code = x.transact.moveOrder.ItemCode,
                    Description = x.transact.moveOrder.ItemDescription,
                    Uom = x.transact.moveOrder.Uom,
                    Category = x.transact.moveOrder.Category,
                    Quantity = Math.Round(x.transact.moveOrder.QuantityOrdered, 2),
                    Unit_Price = x.transact.moveOrder.UnitPrice,
                    Line_Amount = Math.Round(x.transact.moveOrder.UnitPrice * x.transact.moveOrder.QuantityOrdered, 2),
                    Po = "N/a",
                    Service_Provider_Code = x.user.EmpId,
                    Service_Provider = x.transact.transact.PreparedBy,
                    Reason = "",
                    Reference_No = Convert.ToString(x.transact.transact.OrderNo),
                    Sub_Unit = "",
                    Supplier = x.transact.moveOrder.CustomerName,
                    Company_Code = x.transact.moveOrder.CompanyCode,
                    Company_Name = x.transact.moveOrder.CompanyName,
                    Department_Code = x.transact.moveOrder.DepartmentCode,
                    Department_Name = x.transact.moveOrder.DepartmentName,
                    Location_Code = x.transact.moveOrder.LocationCode,
                    Location = x.transact.moveOrder.LocationName,
                    Account_Title_Code = x.transact.moveOrder.AccountCode,
                    Account_Title_Name = x.transact.moveOrder.AccountTitles,
                    Asset = $"{x.transact.moveOrder.AssetTag} {x.transact.moveOrder.Cip_No}",
                    Asset_Cip = $"{x.transact.moveOrder.AssetTag} {x.transact.moveOrder.Cip_No}",
                    System = "ElixirETD_MoveOrder",

                }).ToList();

            var receiptConsol = _context.MiscellaneousReceipts
                .AsNoTracking()
                .GroupJoin(_context.WarehouseReceived, receipt => receipt.Id, warehouse => warehouse.MiscellaneousReceiptId, (receipt, warehouse) => new { receipt, warehouse })
                .SelectMany(x => x.warehouse.DefaultIfEmpty(), (x, warehouse) => new { x.receipt, warehouse })
                 .GroupJoin(userList, receipt => receipt.receipt.PreparedBy, user => user.FullName, (receipt, user) => new { receipt, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.receipt, user })
                .Where(x => x.receipt.warehouse.IsActive == true && x.receipt.warehouse.TransactionType == "MiscellaneousReceipt")
                .Select(x => new GeneralLedgerReportDto
                {
                    SyncId = x.receipt.warehouse.Id,
                    Transaction_Date = x.receipt.receipt.TransactionDate.Date,
                    Item_Code = x.receipt.warehouse.ItemCode,
                    Description = x.receipt.warehouse.ItemDescription,
                    Uom = x.receipt.warehouse.Uom,
                    Category = "",
                    Quantity = x.receipt.warehouse.ActualGood,
                    Unit_Price = x.receipt.warehouse.UnitPrice,
                    Line_Amount = Math.Round(x.receipt.warehouse.UnitPrice * x.receipt.warehouse.ActualGood, 2),
                    Po = "N/a",
                    System = "ElixirETD_MiscellaneousReceipt",
                    Service_Provider_Code = x.user.EmpId,
                    Service_Provider = x.receipt.receipt.PreparedBy,
                    Reason = x.receipt.receipt.Remarks,
                    Reference_No = Convert.ToString(x.receipt.receipt.Id),
                    Supplier = x.receipt.receipt.supplier,
                    Company_Code = x.receipt.receipt.CompanyCode,
                    Company_Name = x.receipt.receipt.CompanyName,
                    Department_Code = x.receipt.receipt.DepartmentCode,
                    Department_Name = x.receipt.receipt.DepartmentName,
                    Location_Code = x.receipt.receipt.LocationCode,
                    Location = x.receipt.receipt.LocationName,
                    Account_Title_Code = x.receipt.warehouse.AccountCode,
                    Account_Title_Name = x.receipt.warehouse.AccountTitles,
                    Asset = "",

                });

            var issueConsol = _context.MiscellaneousIssues
                .AsNoTracking()
                .GroupJoin(_context.MiscellaneousIssueDetail, miscDatail => miscDatail.Id, issue => issue.IssuePKey,
                (miscDetail, issue) => new { miscDetail, issue })
                .SelectMany(x => x.issue.DefaultIfEmpty(), (x, issue) => new { x.miscDetail, issue })
                .GroupJoin(userList, miscDatail => miscDatail.miscDetail.PreparedBy, user => user.FullName,
                (miscDetail, user) => new { miscDetail, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.miscDetail, user })
                .Where(x => x.miscDetail.issue.IsActive == true)
                .Select(x => new GeneralLedgerReportDto
                {
                    SyncId = x.miscDetail.issue.Id,
                    Transaction_Date = x.miscDetail.miscDetail.TransactionDate.Date,
                    Item_Code = x.miscDetail.issue.ItemCode,
                    Description = x.miscDetail.issue.ItemDescription,
                    Uom = x.miscDetail.issue.Uom,
                    Category = "",
                    Quantity = Math.Round(x.miscDetail.issue.Quantity, 2),
                    Unit_Price = x.miscDetail.issue.UnitPrice,
                    Line_Amount = Math.Round(x.miscDetail.issue.UnitPrice * x.miscDetail.issue.Quantity, 2),
                    Po = "N/a",
                    System = "ElixirETD_MiscellaneousIssue",
                    Service_Provider_Code = x.miscDetail.miscDetail.PreparedBy,
                    Service_Provider = x.user.FullName,
                    Reason = x.miscDetail.issue.Remarks,
                    Reference_No = Convert.ToString(x.miscDetail.miscDetail.Id),
                    Supplier = x.miscDetail.issue.Customer,
                    Company_Code = x.miscDetail.miscDetail.CompanyCode,
                    Company_Name = x.miscDetail.miscDetail.CompanyName,
                    Department_Code = x.miscDetail.miscDetail.DepartmentCode,
                    Department_Name = x.miscDetail.miscDetail.DepartmentName,
                    Location_Code = x.miscDetail.miscDetail.LocationCode,
                    Location = x.miscDetail.miscDetail.LocationName,
                    Account_Title_Code = x.miscDetail.issue.AccountCode,
                    Account_Title_Name = x.miscDetail.issue.AccountTitles,
                    Asset = "",
                    Asset_Cip = "",

                });

            var borrowedConsol = _context.BorrowedIssues
                .AsNoTracking()
                .GroupJoin(_context.BorrowedIssueDetails, borrow => borrow.Id, borrowDetail => borrowDetail.BorrowedPKey,
                (borrow, borrowDetail) => new { borrow, borrowDetail })
                .SelectMany(x => x.borrowDetail.DefaultIfEmpty(), (x, borrowDetail) => new { x.borrow, borrowDetail })
                .GroupJoin(userList, borrow => borrow.borrow.ApproveBy, user => user.FullName, (borrow, user) => new { borrow, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.borrow, user })
                .Where(x => x.borrow.borrowDetail.IsActive == true)
                .Select(x => new GeneralLedgerReportDto
                {
                    SyncId = x.borrow.borrowDetail.Id,
                    Transaction_Date = x.borrow.borrowDetail.PreparedDate.Date,
                    Item_Code = x.borrow.borrowDetail.ItemCode,
                    Description = x.borrow.borrowDetail.ItemDescription,
                    Uom = x.borrow.borrowDetail.Uom,
                    Category = "",
                    Quantity = Math.Round(x.borrow.borrowDetail.Quantity, 2),
                    Unit_Price = x.borrow.borrowDetail.UnitPrice,
                    Line_Amount = Math.Round(x.borrow.borrowDetail.UnitPrice * x.borrow.borrowDetail.Quantity, 2),
                    Po = "N/a",
                    System = "ElixirETD_Borrow",
                    Service_Provider_Code = x.user.EmpId,
                    Service_Provider = x.borrow.borrow.ApproveBy,
                    Reason = x.borrow.borrow.Remarks,
                    Reference_No = Convert.ToString(x.borrow.borrow.Id),
                    Supplier = x.borrow.borrow.CustomerName,
                    Company_Code = "",
                    Company_Name = "",
                    Department_Code = "",
                    Department_Name = "",
                    Location_Code = "",
                    Location = "",
                    Account_Title_Code = "",
                    Account_Title_Name = "",
                    Asset = "",
                    Asset_Cip = ""

                });

            var consumeList = _context.BorrowedConsumes
                .Where(x => x.IsActive == true)
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.Id,
                    BorrowedId = x.BorrowedItemPkey,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Uom = x.Uom,
                    Consumed = x.Consume,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    AccountCode = x.AccountCode,
                    AccountTitles = x.AccountTitles,
                    EmpId = x.EmpId,
                    FullName = x.FullName,
                    ReportNumber = x.ReportNumber,

                });

            var returnList = _context.BorrowedIssueDetails
                .Where(x => x.IsActive == true && x.IsApprovedReturned == true)
                .GroupJoin(consumeList, borrowDetails => borrowDetails.Id, consume => consume.BorrowedId
                , (borrowDetails, consume) => new { borrowDetails, consume })
                .SelectMany(x => x.consume.DefaultIfEmpty(), (x, consume) => new { x.borrowDetails, consume })
                .Select(x => new BorrowedConsolidatedDto
                {
                    Id = x.borrowDetails.Id,
                    BorrowedId = x.borrowDetails.BorrowedPKey,
                    ItemCode = x.borrowDetails.ItemCode,
                    ItemDescription = x.borrowDetails.ItemDescription,
                    Uom = x.borrowDetails.Uom,
                    BorrowedQuantity = x.borrowDetails.Quantity != null ? x.borrowDetails.Quantity : 0,
                    Consumed = x.consume.Consumed != null ? x.consume.Consumed : 0,
                    CompanyCode = x.consume.CompanyCode,
                    CompanyName = x.consume.CompanyName,
                    DepartmentCode = x.consume.DepartmentCode,
                    DepartmentName = x.consume.DepartmentName,
                    LocationCode = x.consume.LocationCode,
                    LocationName = x.consume.LocationName,
                    AccountCode = x.consume.AccountCode,
                    AccountTitles = x.consume.AccountTitles,
                    EmpId = x.consume.EmpId,
                    FullName = x.consume.FullName,
                    ReportNumber = x.consume.ReportNumber,
                    UnitPrice = x.borrowDetails.UnitPrice,
                    TransactedBy = x.borrowDetails.ApprovedReturnedBy

                });

            var borrowedIssueList = _context.BorrowedIssues
                .AsNoTracking()
                .Where(x => x.IsActive == true);

            var returnedConsol = returnList
                .GroupJoin(borrowedIssueList, borrowDetail => borrowDetail.BorrowedId, borrow => borrow.Id,
                (borrowDetail, borrow) => new { borrowDetail, borrow })
                .SelectMany(x => x.borrow.DefaultIfEmpty(), (x, borrow) => new { x.borrowDetail, borrow })
                .GroupJoin(userList, borrowDetail => borrowDetail.borrowDetail.TransactedBy, user => user.FullName,
                (borrowDetail, user) => new { borrowDetail, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, user) => new { x.borrowDetail, user })
                .Select(x => new GeneralLedgerReportDto
                {

                    SyncId = x.borrowDetail.borrowDetail.Id,
                    Transaction_Date = x.borrowDetail.borrow.PreparedDate.Date,
                    Item_Code = x.borrowDetail.borrowDetail.ItemCode,
                    Description = x.borrowDetail.borrowDetail.ItemDescription,
                    Uom = x.borrowDetail.borrowDetail.Uom,
                    Category = "",
                    Quantity = x.borrowDetail.borrowDetail.BorrowedQuantity - x.borrowDetail.borrowDetail.Consumed,
                    Unit_Price = x.borrowDetail.borrowDetail.UnitPrice,
                    Line_Amount = Math.Round(x.borrowDetail.borrowDetail.UnitPrice.Value * x.borrowDetail.borrowDetail.BorrowedQuantity - x.borrowDetail.borrowDetail.Consumed, 2),
                    Po = "N/a",
                    System = "ElixirETD_Returned",

                    Service_Provider_Code = x.user.EmpId,
                    Service_Provider = x.borrowDetail.borrowDetail.TransactedBy,
                    Reason = x.borrowDetail.borrowDetail.Remarks,
                    Reference_No = Convert.ToString(x.borrowDetail.borrowDetail.Id),
                    Supplier = x.borrowDetail.borrow.CustomerName,
                    Company_Code = x.borrowDetail.borrowDetail.CompanyCode,
                    Company_Name = x.borrowDetail.borrowDetail.CompanyName,
                    Department_Code = x.borrowDetail.borrowDetail.DepartmentCode,
                    Department_Name = x.borrowDetail.borrowDetail.DepartmentName,
                    Location_Code = x.borrowDetail.borrowDetail.LocationCode,
                    Location = x.borrowDetail.borrowDetail.LocationName,
                    Account_Title_Code = x.borrowDetail.borrowDetail.AccountCode,
                    Account_Title_Name = x.borrowDetail.borrowDetail.AccountTitles,
                    Asset = "",
                    Asset_Cip = "",


                });

            var fuelRegisterConsol = _context.FuelRegisterDetails
              .Include(m => m.Material)
              .ThenInclude(id => id.ItemCategory)
              .Include(w => w.Warehouse_Receiving)
              .Where(r => r.FuelRegister.Is_Transact == true)
              .Select(x => new GeneralLedgerReportDto
              {

                  SyncId = x.Id,
                  Transaction_Date = x.FuelRegister.Transact_At.Value.Date,
                  Item_Code = x.Material.ItemCode,
                  Description = x.Material.ItemDescription,
                  Uom = x.Material.Uom.UomCode,
                  Category = "",
                  Quantity = x.Liters,
                  Unit_Price = x.Warehouse_Receiving.UnitPrice,
                  Line_Amount = Math.Round(x.Warehouse_Receiving.UnitPrice * x.Liters.Value, 2),
                  Po = "N/a",
                  System = "ElixirETD_Fuel_Register",
                  Service_Provider_Code = "",
                  Service_Provider = x.FuelRegister.Transact_By,
                  Reason = x.FuelRegister.Remarks,
                  Reference_No = Convert.ToString(x.FuelRegisterId),
                  Supplier = $"{x.FuelRegister.RequestorId}:{x.FuelRegister.RequestorName}",
                  Company_Code = x.FuelRegister.Company_Code,
                  Company_Name = x.FuelRegister.Company_Name,
                  Department_Code = x.FuelRegister.Department_Code,
                  Department_Name = x.FuelRegister.Department_Name,
                  Location_Code = x.FuelRegister.Location_Code,
                  Location = x.FuelRegister.Location_Name,
                  Account_Title_Code = x.FuelRegister.Account_Title_Code,
                  Account_Title_Name = x.FuelRegister.Account_Title_Name,
                  Asset = x.FuelRegister.Asset.AssetCode,
                  Asset_Cip = "",

              });


            if (!string.IsNullOrEmpty(DateFrom) && !string.IsNullOrEmpty(DateTo))
            {
                var dateFrom = DateTime.Parse(DateFrom).Date;
                var dateTo = DateTime.Parse(DateTo).Date;

                //receivingConsol = receivingConsol
                //    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                //    .ToList()
                //    ;

                moveOrderConsol = moveOrderConsol
                    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    .ToList()
                    ;

                receiptConsol = receiptConsol
                    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    ;

                issueConsol = issueConsol
                     .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    ;

                borrowedConsol = borrowedConsol
                    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    ;

                returnedConsol = returnedConsol
                    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    ;

                returnedConsol = returnedConsol
                    .Where(x => x.Transaction_Date.Date >= dateFrom && x.Transaction_Date.Date <= dateTo)
                    ;
            }
            else
            {
                moveOrderConsol = moveOrderConsol.Where(x => x.SyncId == null).ToList();
                receiptConsol = receiptConsol.Where(x => x.SyncId == null);
                issueConsol = issueConsol.Where(x => x.SyncId == null);
                borrowedConsol = borrowedConsol.Where(x => x.SyncId == null);
                returnedConsol = returnedConsol.Where(x => x.SyncId == null);
                //fuelRegisterConsol = fuelRegisterConsol.Where(x => x.SyncId == null);

            }


            var consolidateList = moveOrderConsol
                .Concat(await receiptConsol.ToListAsync())
                .Concat(await issueConsol.ToListAsync())
                .Concat(await borrowedConsol.ToListAsync())
                .Concat(await returnedConsol.ToListAsync())
                .Concat(await fuelRegisterConsol.ToListAsync());


            var materials = _context.Materials
                .AsNoTracking()
                .Include(x => x.Uom)
                .Include(x => x.ItemCategory);


            var creditConsol = consolidateList
                .Join(materials,
                 consol => consol.Item_Code, material => material.ItemCode,
                 (consol, material) => new GeneralLedgerReportDto
                 {
                     SyncId = consol.SyncId,
                     Transaction_Date = consol.Transaction_Date,
                     Item_Code = material.ItemCode,
                     Description = material.ItemDescription,
                     Uom = material.Uom.UomCode,
                     Category = material.ItemCategory.ItemCategoryName,
                     Quantity = consol.Quantity,
                     Unit_Price = consol.Unit_Price,
                     Line_Amount = -Math.Abs(consol.Line_Amount.Value),
                     Po = consol.Po,
                     Service_Provider_Code = consol.Service_Provider_Code,
                     Service_Provider = consol.Service_Provider,
                     Reason = consol.Reason,
                     Reference_No = consol.Reference_No,
                     Supplier = consol.Supplier,
                     Company_Code = consol.Company_Code,
                     Company_Name = consol.Company_Name,
                     Department_Code = consol.Department_Code,
                     Department_Name = consol.Department_Name,
                     Location_Code = consol.Location_Code,
                     Location = consol.Location,
                     Account_Title_Code = consol.Account_Title_Code,
                     Account_Title_Name = consol.Account_Title_Name,
                     Asset = consol.Asset,
                     Asset_Cip = consol.Asset_Cip,
                     System = consol.System,
                     DR_CR = "Credit"

                 });

            creditConsol = creditConsol.OrderBy(x => x.Transaction_Date.Date)
                .ThenBy(x => x.Item_Code);

            var debitConsol = consolidateList
                .Join(materials,
                 consol => consol.Item_Code, material => material.ItemCode,
                 (consol, material) => new GeneralLedgerReportDto
                 {
                     SyncId = consol.SyncId,
                     Transaction_Date = consol.Transaction_Date,
                     Item_Code = material.ItemCode,
                     Description = material.ItemDescription,
                     Uom = material.Uom.UomCode,
                     Category = material.ItemCategory.ItemCategoryName,
                     Quantity = consol.Quantity,
                     Unit_Price = consol.Unit_Price,
                     Line_Amount = consol.Line_Amount,
                     Po = consol.Po,
                     Service_Provider_Code = consol.Service_Provider_Code,
                     Service_Provider = consol.Service_Provider,
                     System = consol.System,
                     Reason = consol.Reason,
                     Reference_No = consol.Reference_No,
                     Supplier = consol.Supplier,
                     Company_Code = consol.Company_Code,
                     Company_Name = consol.Company_Name,
                     Department_Code = consol.Department_Code,
                     Department_Name = consol.Department_Name,
                     Location_Code = consol.Location_Code,
                     Location = consol.Location,
                     Account_Title_Code = consol.Account_Title_Code,
                     Account_Title_Name = consol.Account_Title_Name,
                     Asset = consol.Asset,
                     Asset_Cip = consol.Asset_Cip,
                     DR_CR = "Debit"


                 });

            debitConsol = debitConsol.OrderBy(x => x.Transaction_Date.Date)
                      .ThenBy(x => x.Item_Code);

            var forReports = creditConsol
                .Concat(debitConsol);


            var reports = forReports
                .Select(x => new GeneralLedgerReportDto
                {

                    SyncId = x.SyncId,
                    Asset_Cip = x.Asset_Cip,
                    Transaction_Date = x.Transaction_Date,
                    Supplier = x.Supplier,
                    Company_Code = x.Company_Code,
                    Company_Name = x.Company_Name,
                    Department_Code = x.Department_Code,
                    Department_Name = x.Department_Name,
                    Location_Code = x.Location_Code,
                    Location = x.Location,
                    Account_Title_Code = x.Account_Title_Code,
                    Account_Title_Name = x.Account_Title_Name,
                    Reference_No = x.Reference_No,
                    Po = x.Po,
                    Item_Code = x.Item_Code,
                    Description = x.Description,
                    Category = x.Category,
                    Quantity = x.Quantity,
                    Uom = x.Uom,
                    Unit_Price = x.Unit_Price,
                    Line_Amount = x.Line_Amount,
                    Asset = x.Asset,
                    Service_Provider_Code = x.Service_Provider_Code,
                    Service_Provider = x.Service_Provider,
                    System = x.System,
                    Reason = x.Reason,
                    Month = x.Transaction_Date.Date.Month,
                    Year = x.Transaction_Date.Date.Year,
                    DR_CR = x.DR_CR

                });


            return reports.ToList();
        }


    }

}
