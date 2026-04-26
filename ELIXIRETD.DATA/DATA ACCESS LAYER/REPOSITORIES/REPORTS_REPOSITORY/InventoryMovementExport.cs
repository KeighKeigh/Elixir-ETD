using ClosedXML.Excel;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.SERVICES;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY.ConsolidateFinanceExport;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY
{
    public class InventoryMovementExport
    {

        public class InventoryMovementExportCommand : IRequest<Unit>
        {

            public string DateFrom { get; set; }
            public string PlusOne { get; set; }
            public UserParams userParams { get; set; }
            public string Search { get; set; }

        }


        public class Handler : IRequestHandler<InventoryMovementExportCommand, Unit>
        {
            private readonly IUnitOfWork _report;

            public Handler(IUnitOfWork report)
            {
                _report = report;
            }

            public async Task<Unit> Handle(InventoryMovementExportCommand command, CancellationToken cancellationToken)
            {

                var inventory = await _report.Reports.InventoryMovementReports(command.userParams, command.DateFrom, command.PlusOne, command.Search);


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Inventory Movement Report");

                    var headers = new List<string>
                    {
                        "Item Code",
                        "Item Description",
                        "Receiving",
                        "Miscellaneous Receipt",
                        "Returned",
                        "Move Order",
                        "Miscellaneous Issue",
                        "Fuel Register",
                        "Borrowed",
                        "Ending",
                        "Current Stock",
                        "Unit Cost",
                        "Amount",

                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.Azure;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }


                    for (var index = 1; index <= inventory.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = inventory[index - 1].ItemCode;
                        row.Cell(2).Value = inventory[index - 1].ItemDescription;
                        row.Cell(3).Value = inventory[index - 1].TotalReceiving;
                        row.Cell(4).Value = inventory[index - 1].TotalReceipt;
                        row.Cell(5).Value = inventory[index - 1].TotalReturned;
                        row.Cell(6).Value = inventory[index - 1].TotalMoveOrder;
                        row.Cell(7).Value = inventory[index - 1].TotalIssue;
                        row.Cell(8).Value = inventory[index - 1].TotalFuelRegister;
                        row.Cell(9).Value = inventory[index - 1].TotalBorrowed;
                        row.Cell(10).Value = inventory[index - 1].Ending;
                        row.Cell(11).Value = inventory[index - 1].CurrentStock;
                        row.Cell(12).Value = inventory[index - 1].UnitCost;
                        row.Cell(13).Value = inventory[index - 1].Amount;

                    }
                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"InventoryMovementReports {command.DateFrom} - {command.PlusOne}.xlsx");
                }

                return Unit.Value;
            }

        }
    }
}
