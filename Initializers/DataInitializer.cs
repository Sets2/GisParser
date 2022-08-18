using GisParser.Constants;

public class DataInitializer
{
    public async Task InitMS(ApplicationDbContext dbContext)
    {
        foreach (var m in LocationConstants.CitiesCodes)
        {
//            dbContext.MeteorologicalStations.Add(new MeteorologicalStation(id: int.Parse(m.Key), name: m.Value));
        }
        await dbContext.SaveChangesAsync();
    }

    public static async Task InitUser(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, List<UserDto> UserLists)
    {
        foreach (var userDTO in UserLists)
        {
            //var Result1 = await _roleManager.FindByNameAsync(userDTO.Role);
            //if (Result1 is Object )
            //{
            //    Console.WriteLine("!!!Ошибка2!!!");
            //    Console.WriteLine(Result1.ToString);
            //}
            if (await roleManager.FindByNameAsync(userDTO.Role) == null)
            {
                var Result2 = await roleManager.CreateAsync(new IdentityRole(userDTO.Role));
                //if (!Result2.Succeeded)
                //{
                //    Console.WriteLine("!!!Ошибка2!!!");
                //    Console.WriteLine(Result2.Errors);
                //}
            }
            if (await userManager.FindByNameAsync(userDTO.UserName) == null)
            {
                User user = new User { UserName = userDTO.UserName };
                IdentityResult result = await userManager.CreateAsync(user, userDTO.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userDTO.Role);
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
