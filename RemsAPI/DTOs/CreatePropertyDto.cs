using System.ComponentModel.DataAnnotations;

namespace RemsAPI.DTOs;

public class CreatePropertyDto
{
  [Required(ErrorMessage = "Parcel number is required")]
  public string? ParcelNumber { get; set; }
  [Required(ErrorMessage = "Lot number is required")]
  public string? LotNumber { get; set; }
  [Required(ErrorMessage = "Address is required")]
  public string Address { get; set; } = "";
  [Required(ErrorMessage = "Geometry is required")]
  public string Geometry { get; set; } = "";
  [Required(ErrorMessage = "Property type is required")]
  public string PropertyType { get; set; } = "";
  [Required(ErrorMessage = "NeighborhoodId is required")]
  public string NeighborhoodId { get; set; } = "";
  public string? ImagePath { get; set; }

}
