using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;

public interface IUserService
{
    Task<bool> Register(RegisterModel model);
    Task<bool> Login(LoginModel model);
    Task<User> GetUserByUsername(string username);
    Task LogoutAsync();
    Task<IList<string>> GetUserRoles(User usr);


}