using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class PdfService(RemsDbContext context, IMapper mapper) : IPdfService
{
  public async Task<MemoryStream> ExportLogsToPdfAsync()
  {
    QuestPDF.Settings.License = LicenseType.Community;

    var stream = new MemoryStream();

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

    Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(2, Unit.Centimetre);
        page.DefaultTextStyle(x => x.FontSize(9));

        page.Header()
          .Text("Log records")
          .FontSize(24)
          .FontColor(Colors.Blue.Medium)
          .Bold();
        page.Content().Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(); // id
            columns.ConstantColumn(110); // userId
            columns.RelativeColumn(); // status
            columns.ConstantColumn(90); // operationType
            columns.RelativeColumn(3); // description
            columns.ConstantColumn(90); // timestamp
            columns.RelativeColumn(); // ipAddress
          });

          table.Header(header =>
          {
            header.Cell().BorderBottom(2).Padding(8).Text("#");
            header.Cell().BorderBottom(2).Padding(8).Text("Username");
            header.Cell().BorderBottom(2).Padding(8).Text("Status");
            header.Cell().BorderBottom(2).Padding(8).Text("OperationType");
            header.Cell().BorderBottom(2).Padding(8).Text("Description");
            header.Cell().BorderBottom(2).Padding(8).Text("Timestamp");
            header.Cell().BorderBottom(2).Padding(8).Text("IpAddress");
          });

          int index = 1;
          foreach (var log in logsDto)
          {
            table.Cell().BorderBottom(2).Padding(8).Text(index.ToString());
            table.Cell().BorderBottom(2).Padding(8).Text(log.UserName ?? "Unknown");
            table.Cell().BorderBottom(2).Padding(8).Text(log.Status);
            table.Cell().BorderBottom(2).Padding(8).Text(log.OperationType);
            table.Cell().BorderBottom(2).Padding(8).Text(log.Description);
            table.Cell().BorderBottom(2).Padding(8).Text(log.Timestamp.ToString());
            table.Cell().BorderBottom(2).Padding(8).Text(log.IpAddress);
            index++;
          }
        });
      });
    }).GeneratePdf(stream);
    stream.Position = 0;
    return stream;
  }
}
