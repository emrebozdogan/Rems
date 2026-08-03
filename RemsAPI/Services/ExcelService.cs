using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class ExcelService(RemsDbContext context, IMapper mapper) : IExcelService
{
  public async Task<MemoryStream> ExportLogsToExcelAsync(List<LogDto> logs)
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

      foreach (var log in logs)
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

  public async Task<MemoryStream> ExportPropertiesToExcelAsync(List<PropertyDto> properties)
  {
    var stream = new MemoryStream();

    using var workbook = new XLWorkbook();
    {
      var worksheet = workbook.AddWorksheet();
      int cellNumber = 2;

      worksheet.Cell("A1").Value = "City";
      worksheet.Cell("B1").Value = "District";
      worksheet.Cell("C1").Value = "Neighborhood";
      worksheet.Cell("D1").Value = "Parcel Number";
      worksheet.Cell("E1").Value = "Lot Number";
      worksheet.Cell("F1").Value = "Address";
      worksheet.Cell("G1").Value = "Property Type";

      var headerRange = worksheet.Range("A1:G1");
      headerRange.Style.Font.Bold = true;
      headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

      foreach (var property in properties)
      {
        worksheet.Cell("A" + cellNumber).Value = property.CityName ?? "Unknown";
        worksheet.Cell("B" + cellNumber).Value = property.DistrictName ?? "Unknown";
        worksheet.Cell("C" + cellNumber).Value = property.NeighborhoodName ?? "Unknown";
        worksheet.Cell("D" + cellNumber).Value = property.ParcelNumber;
        worksheet.Cell("E" + cellNumber).Value = property.LotNumber;
        worksheet.Cell("F" + cellNumber).Value = property.Address;
        worksheet.Cell("G" + cellNumber).Value = property.PropertyType;
        cellNumber++;
      }

      worksheet.Columns().AdjustToContents();
      workbook.SaveAs(stream);
    }
    stream.Position = 0;
    return stream;
  }
}
