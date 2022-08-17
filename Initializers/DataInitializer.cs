using GisParser.Constants;

public class DataInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DataInitializer(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitMS()
    {
        foreach (var m in LocationConstants.CitiesCodes)
        {
            //_dbContext.MeteorologicalStations.Add(new MeteorologicalStation(id: int.Parse(m.Key), name: m.Value));
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task InitUser(List<UserDto> UserLists)
    {
        foreach (var userDTO in UserLists)
        {
            if (await _roleManager.FindByNameAsync(userDTO.Role) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(userDTO.Role));
            }
            if (await _userManager.FindByNameAsync(userDTO.UserName) == null)
            {
                User user = new User { UserName = userDTO.UserName };
                IdentityResult result = await _userManager.CreateAsync(user, userDTO.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userDTO.Role);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
