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
  public async Task<MemoryStream> ExportLogsToPdfAsync(List<LogDto> logs)
  {
    QuestPDF.Settings.License = LicenseType.Community;

    var stream = new MemoryStream();

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
          foreach (var log in logs)
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

  public async Task<MemoryStream> ExportPropertiesToPdfAsync(List<PropertyDto> properties)
  {
    QuestPDF.Settings.License = LicenseType.Community;

    var stream = new MemoryStream();

    Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(2, Unit.Centimetre);
        page.DefaultTextStyle(x => x.FontSize(9));

        page.Header()
          .Text("Property Records")
          .FontSize(24)
          .FontColor(Colors.Blue.Medium)
          .Bold();
        page.Content().Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(); // city
            columns.RelativeColumn(); // district
            columns.RelativeColumn(); // neighborhood
            columns.RelativeColumn(); // parcel
            columns.RelativeColumn(); // lot
            columns.RelativeColumn(3); // address
            columns.RelativeColumn(); // property type
          });

          table.Header(header =>
          {
            header.Cell().BorderBottom(2).Padding(8).Text("City");
            header.Cell().BorderBottom(2).Padding(8).Text("District");
            header.Cell().BorderBottom(2).Padding(8).Text("Neighborhood");
            header.Cell().BorderBottom(2).Padding(8).Text("Parcel No");
            header.Cell().BorderBottom(2).Padding(8).Text("Lot No");
            header.Cell().BorderBottom(2).Padding(8).Text("Address");
            header.Cell().BorderBottom(2).Padding(8).Text("Type");
          });

          foreach (var property in properties)
          {
            table.Cell().BorderBottom(1).Padding(8).Text(property.CityName ?? "Unknown");
            table.Cell().BorderBottom(1).Padding(8).Text(property.DistrictName ?? "Unknown");
            table.Cell().BorderBottom(1).Padding(8).Text(property.NeighborhoodName ?? "Unknown");
            table.Cell().BorderBottom(1).Padding(8).Text(property.ParcelNumber);
            table.Cell().BorderBottom(1).Padding(8).Text(property.LotNumber);
            table.Cell().BorderBottom(1).Padding(8).Text(property.Address);
            table.Cell().BorderBottom(1).Padding(8).Text(property.PropertyType);
          }
        });
      });
    }).GeneratePdf(stream);
    stream.Position = 0;
    return stream;
  }
}
