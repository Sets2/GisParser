using GisParser.Auth;
using GisParser.Data;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace GisParser.Apis;

public class AuthApi : IApi
{
	public void Register(WebApplication app)
	{
		app.MapGet("/login", [AllowAnonymous] async (HttpContext context,
			ITokenService tokenService, IUserRepositoryAuth userRepository, 
			UserManager<User> userManager, SignInManager<User> signInManager) =>
		{
			UserDto user = new UserDto(
				context.Request.Query["username"],
				context.Request.Query["password"]);

            var Result = await userRepository.GetUser(userManager, signInManager, user);
			if (Result != null)
			{
                var token = tokenService.BuildToken(app.Configuration["Jwt:Key"],
					app.Configuration["Jwt:Issuer"], user);
				return Results.Ok(token);
			}
			else return Results.Unauthorized();
        });
	}
}