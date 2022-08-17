using Microsoft.AspNetCore.Identity;

public class UserRepository : IUserRepository
{
//  private readonly ApplicationDbContext _context;
  private readonly UserManager<User> _userManager;
  private readonly SignInManager<User> _signInManager;

  public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
  {
    //    _context = context;
    _userManager = userManager;
    _signInManager = signInManager;

  }

  private List<UserDto> _users => new()
    {
        new UserDto("Bob", "123","Admin"),
    };

    public UserDto GetUser(User user) => 
        _users.FirstOrDefault(u =>
            string.Equals(u.UserName, user.UserName) &&
            string.Equals(u.Password, user.Password)) ??
            throw new Exception();

}