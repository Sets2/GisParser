using GisParser.Data;
using System.Data;

namespace GisParser.Auth;

public class UserRepositoryAuth : IUserRepositoryAuth
{
	public async Task<UserDto?> GetUser(UserManager<User> _userManager, SignInManager<User> _signInManager, UserDto user)
	{
        var userEF = await _userManager.FindByNameAsync(user.UserName);
        if (userEF != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(userEF, user.Password, false);

            if (result.Succeeded)
            {
                user.ConcurrencyStamp= userEF.ConcurrencyStamp;
                user.Roles = (List<string>) await _userManager.GetRolesAsync(userEF);

                return user;
            }
            else return null;
        }
        else return null;
    }
    
    }
