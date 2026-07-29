namespace RemsAPI.Entities;

public class User
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string Name { get; set; }
  public required string Email { get; set; }
  public required string Role { get; set; }
  public required byte[] PasswordHash { get; set; }
  public required byte[] PasswordSalt { get; set; }
  public bool IsDeleted { get; set; } = false;
  public ICollection<Property>? Properties { get; set; }
}