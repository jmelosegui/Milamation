using ClosedXML.Excel;
using HarvestClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Milamation
{
    public static class TimeEntriesReport
    {
        public static string FileName => "HarvestReport.xlsx";
        
        public static void CreateExcelHeader()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Time Entries");
            worksheet.Cell("A1").Value = "Date";
            worksheet.Cell("B1").Value = "Project";
            worksheet.Cell("C1").Value = "Task";
            worksheet.Cell("D1").Value = "PBI";
            worksheet.Cell("E1").Value = "Reviewers Comment";
            worksheet.Cell("F1").Value = "Notes";
            worksheet.Cell("G1").Value = "Hours";
            worksheet.Cell("H1").Value = "Hours Rounded";
            worksheet.Cell("I1").Value = "Billable?";
            worksheet.Cell("J1").Value = "Full Name";
            worksheet.Cell("K1").Value = "Role";

            workbook.SaveAs(FileName);
        }

        public static int AddEntries(IEnumerable<TimesheetEntry> entries, int rowId)
        {   
            var workbook = new XLWorkbook(FileName);
            var worksheet = workbook.Worksheets.Worksheet("Time Entries");

            var i = rowId;
            foreach (var item in entries)
            {
                worksheet.Cell($"A{i}").Value = item.SpentDate;
                worksheet.Cell($"B{i}").Value = $"[{item.Project?.Code}] {item.Project?.Name}";
                worksheet.Cell($"C{i}").Value = item.Task?.Name;
                worksheet.Cell($"D{i}").Value = item.PBI;
                //worksheet.Cell($"D{i}").FormulaA1 = $"=IF(ISNUMBER(SEARCH(\"planning\",C{i})),\"Planning\",IF(ISNUMBER(SEARCH(\"Scrum\",F{i})), \"Scrum\", LEFT(F{i},6)))";
                worksheet.Cell($"E{i}").Value = item.ReviewersComment;
                worksheet.Cell($"F{i}").Value = item.Notes;
                worksheet.Cell($"G{i}").Value = item.Hours;
                worksheet.Cell($"H{i}").Value = item.RoundedHours;
                worksheet.Cell($"I{i}").Value = item.Billable;
                worksheet.Cell($"J{i}").Value = item.User?.Name;
                worksheet.Cell($"K{i}").Value = item.UserRoles;

                i++;
            }

            workbook.SaveAs(FileName);

            return i;
        }

        public static void FormatExcel(int rowNumber)
        {   
            var workbook = new XLWorkbook(FileName);
            var worksheet = workbook.Worksheets.Worksheet("Time Entries");

            // Formatting headers            
            var rngTable = worksheet.Range($"A1:K{rowNumber - 1}");
            var rngHeaders = rngTable.Range("A1:K1");
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Fill.BackgroundColor = XLColor.Black;
            rngHeaders.Style.Font.FontColor = XLColor.White;

            worksheet.Range($"E2:E{rowNumber - 1}")
                     .AddConditionalFormat()
                     .WhenNotBlank()
                     .Fill.SetBackgroundColor(XLColor.FromArgb(255, 199, 206))
                     .Font.SetFontColor(XLColor.DarkRed);

            worksheet.Columns(1, 11).AdjustToContents();
            workbook.SaveAs(FileName);
        }

        internal static void OpenReport()
        {
            FileInfo fi = new FileInfo(FileName);

            if (fi.Exists)
            {
                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = fi.FullName;

                proc.Start();                
            }
        }
    }
}
