using ClosedXML.Excel;
using HarvestClient;
using HarvestClient.Model;
using ManyConsole;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HarvestReport
{
    public class ReportCommand : ConsoleCommand
    {
        private readonly HarvestRestClient harvestClient;
        private IDictionary<int, User> userCache = new Dictionary<int, User>();

        public ReportCommand()
        {
            this.IsCommand("report", "Export the time etries report")
                .SkipsCommandSummaryBeforeRunning();

            AllowsAnyAdditionalArguments();

            string token = "1228675.pt.4kbDG5fErBC1TRCGFgsTI6l9PCCYUAFO-7LIT3H9mMAlQeibDIN7oC8MiqiXrfomnUVPq1pya0yhRZdWO0AxFw";
            int accountId = 123361;

            this.harvestClient = new HarvestRestClient(token, accountId);
        }

        public override int Run(string[] remainingArguments)
        {
            string fileName = "HarvestReport.xlsx";

            var projects = harvestClient.Projects.List(32315 /*Pwc*/, null);

            CreateExcelHeader();

            int rowNumber = 2;

            foreach (var project in projects)
            {
                var timeEntries = harvestClient.Timesheets.GetTimeEntries(32315 /*Pwc*/, project.Id, null, null);

                rowNumber = AddEntries(timeEntries, rowNumber);
            }

            FormatExcel(rowNumber);

            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
            {
                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = fi.FullName;

                proc.Start();
                proc.WaitForExit();
                Process.Start(fileName);
            }

            return 0;
        }

        private void FormatExcel(int rowNumber)
        {
            string fileName = "HarvestReport.xlsx";
            var workbook = new XLWorkbook(fileName);
            var worksheet = workbook.Worksheets.Worksheet("Time Entries");

            // Formatting headers            
            var rngTable = worksheet.Range($"A1:K{rowNumber - 1}");
            var rngHeaders = rngTable.Range("A1:K1");
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Fill.BackgroundColor = XLColor.Black;
            rngHeaders.Style.Font.FontColor = XLColor.White;

            worksheet.Columns(1, 11).AdjustToContents();
            workbook.SaveAs(fileName);
        }

        private void CreateExcelHeader()
        {
            string fileName = "HarvestReport.xlsx";

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

            workbook.SaveAs(fileName);
        }

        internal int AddEntries(IEnumerable<TimesheetEntry> entries, int rowId)
        {
            string fileName = "HarvestReport.xlsx";
            var workbook = new XLWorkbook(fileName);
            var worksheet = workbook.Worksheets.Worksheet("Time Entries");

            var i = rowId;
            foreach (var item in entries)
            {
                worksheet.Cell($"A{i}").Value = item.SpentDate;
                worksheet.Cell($"B{i}").Value = item.Project?.Name;
                worksheet.Cell($"C{i}").Value = item.Task?.Name;
                worksheet.Cell($"D{i}").Value = "";
                worksheet.Cell($"E{i}").Value = "";
                worksheet.Cell($"F{i}").Value = item.Notes;
                worksheet.Cell($"G{i}").Value = item.Hours;
                worksheet.Cell($"H{i}").Value = "";
                worksheet.Cell($"I{i}").Value = item.Billable;
                worksheet.Cell($"J{i}").Value = item.User?.Name;
                worksheet.Cell($"K{i}").Value = this.GetUserRoles(item.User.Id);

                i++;
            }

            workbook.SaveAs(fileName);

            return i;
        }

        private string GetUserRoles(int userId)
        {
            User result;

            if (!userCache.TryGetValue(userId, out result))
            {
                result = harvestClient.GetUser(userId);

                userCache.Add(userId, result);
            }            

            return string.Join(',', result.Roles);
        }
    }
}
