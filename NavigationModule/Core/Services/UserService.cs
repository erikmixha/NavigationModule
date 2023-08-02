using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using NavigationModule.Domain.Entities;
using NavigationModule.ViewModels;

public class UserService : IUserService
{
    private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;

    }

    public async Task<bool> Register(RegisterModel model)
    {

        // Validate model
        if (!IsValidRegisterModel(model))
            return false;

        var user = new User
        {
            UserName = model.Username,
            Email = model.Email,
            DailyGoalAchievementStatus = false 
        };

        //add user
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            //add role 
            var role = await _roleManager.FindByNameAsync(model.Role);
            result = await _userManager.AddToRoleAsync(user, role.Name);

        }
        return result.Succeeded;

    }

    
    public async Task<bool> Login(LoginModel model)
    {
        // Validate model
        if (!IsValidLoginModel(model))
            return false;

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return false;

        return true;

    }

    public async Task<User> GetUserByUsername(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user;
    }

    public async Task<IList<string>> GetUserRoles(User usr)
    {
        var user = await _userManager.GetRolesAsync(usr);
        return user;
    }


    private bool IsValidRegisterModel(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)
           || string.IsNullOrWhiteSpace(model.ConfirmPassword) || string.IsNullOrWhiteSpace(model.Email))
        {
            return false;
        }

        if (model.Password != model.ConfirmPassword)
        {
            return false;
        }


        return true;
    }

    private bool IsValidLoginModel(LoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            return false;
        }

        return true;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}