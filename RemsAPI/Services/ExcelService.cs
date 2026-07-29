using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class ExcelService(RemsDbContext context, IMapper mapper) : IExcelService
{
  public async Task<MemoryStream> ExportLogsToExcelAsync()
  {
    var stream = new MemoryStream();

    using var workbook = new XLWorkbook();
    {
      var worksheet = workbook.AddWorksheet();
      int cellNumber = 2;


      worksheet.Cell("A1").Value = "Username";
      worksheet.Cell("B1").Value = "Status";
      worksheet.Cell("C1").Value = "OperationType";
      worksheet.Cell("D1").Value = "Description";
      worksheet.Cell("E1").Value = "Timestamp";
      worksheet.Cell("F1").Value = "IpAddress";

      var headerRange = worksheet.Range("A1:F1");
      headerRange.Style.Font.Bold = true;
      headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

      var logs = await context.Logs.AsNoTracking().ToListAsync();
      var userIds = logs.Select(i => i.UserId).Distinct().ToList();
      var users = await context.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Name);

      var logsDto = logs.Select(l => {
          var logDto = mapper.Map<LogDto>(l);
          var userName = users.ContainsKey(l.UserId) ? users[l.UserId] : "Unknown";
          logDto.UserName = userName;
          if (logDto.Description != null && logDto.Description.Contains(l.UserId)) {
              logDto.Description = logDto.Description.Replace(l.UserId, userName);
          }
          return logDto;
      }).ToList();

      foreach (var log in logsDto)
      {
        worksheet.Cell("A" + cellNumber).Value = log.UserName ?? "Unknown";
        worksheet.Cell("B" + cellNumber).Value = log.Status;
        worksheet.Cell("C" + cellNumber).Value = log.OperationType;
        worksheet.Cell("D" + cellNumber).Value = log.Description;
        worksheet.Cell("E" + cellNumber).Value = log.Timestamp.ToString();
        worksheet.Cell("F" + cellNumber).Value = log.IpAddress;
        cellNumber++;
      }

      worksheet.Columns().AdjustToContents();
      workbook.SaveAs(stream);
    }
    stream.Position = 0;
    return stream;
  }
}
