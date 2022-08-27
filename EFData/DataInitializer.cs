using GisParser.Constants;
using GisParser.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace GisParser.EFData;

public class DataInitializer
{
    public static async Task InitMS(ApplicationDbContext dbContext)
    {
        if (!dbContext.MeteorologicalStation.Any())
        {
            foreach (var m in LocationConstants.CitiesCodes)
            {
                await dbContext.MeteorologicalStation.AddAsync(new MeteorologicalStation(id: int.Parse(m.Key), name: m.Value));
            }
            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task InitUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, List<UserDto> UserLists)
    {
        if (!userManager.Users.Any())
        {
            foreach (var userDTO in UserLists)
            {
                if (await roleManager.FindByNameAsync(userDTO.Role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(userDTO.Role));
                }
                User user = new User { UserName = userDTO.UserName };
                var result = await userManager.CreateAsync(user, userDTO.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userDTO.Role);
                }
                //userManager.Users.ToString();
            }
        }
    }
}
