using GisParser.Data;

namespace GisParser.Auth;

public interface IUserRepositoryAuth
{
    Task<UserDto?> GetUser(UserManager<User> _userManager, SignInManager<User> _signInManager, UserDto user);


}