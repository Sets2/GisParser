
namespace GisParser.Data;

public class UserDto {
    public string UserName="";
    public string Password ="";
    public string Role = "";
    public string ConcurrencyStamp = "";
    public List<string> Roles =null;

    public UserDto() : base() { }

    public UserDto(string userName, string password)
    {
        UserName = userName ?? "";
        Password = password ?? "";
        Role = "";
        ConcurrencyStamp = "";
        Roles = null;
    }
    public UserDto(string userName, string password, string role, string concurrencyStamp, List<string> roles) {
        UserName = userName;
        Password = password;
        Role = "";
        ConcurrencyStamp = concurrencyStamp;
        Roles = roles;
    }
}

public class User : IdentityUser
{
    public DateOnly Birthday { get; set; }
}