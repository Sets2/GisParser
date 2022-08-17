public record UserDto(string UserName, string Password, string Role);

public class User: IdentityUser
{
  //[Required]
  //public string UserName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}