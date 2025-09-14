
using Abcmoney_Transfer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Abcmoney_Transfer.Controllers
{
    [Route("api/")]
    [ApiExplorerSettings(GroupName = "access-control")]
    public class AccountController : BaseApiController
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<Userlogin> _userManager;
        private readonly SignInManager<Userlogin> _signInManager;
        private readonly RoleManager<Identity> _roleManager;

        public AccountController(TokenService tokenService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [AllowAnonymous]
        // Endpoint to login and get both Access and Refresh Tokens
        [HttpPost("login")]
        public async Task<ResponseModel> Login(LoginInputVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Your login logic here (e.g., validate user credentials)
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user == null)
                    {
                        return new ResponseModel(500, "Invalid username or password !", model);
                    }
                    var result = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (!result)
                    {
                        return new ResponseModel(400, "Invalid username or password !!!");
                    }
                    if (user != null && result == true)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        /*      var token = _tokenService.GenerateToken(user,roles);
                          }*/

                        // Generate tokens
                        var (accessToken, refreshToken) = _tokenService.GenerateTokens(user, roles);

                        // Store the refresh token in a HttpOnly cookie
                        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // Use 'Secure' flag for production to enforce HTTPS
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddDays(7) // Set the cookie expiration
                        });

                        return new ResponseModel(200, "Loged in Successfully", new { AccessToken = accessToken });
                    }
                    else
                    {
                        return new ResponseModel(400, "Invalid username or password !!!");
                    }
                }
                else
                {
                    return new ResponseModel(500, "Invalid Model", GetModelErrors(ModelState));
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel(StatusCodes.Status500InternalServerError, ex.Message, model);
            }
        }

        [AllowAnonymous]
        [HttpPost("user/register")]
        public async Task<ResponseModel> Register(RegisterInputVM model)
        {
            if (!ModelState.IsValid)
            {

                return new ResponseModel(500, "Invalid Model", GetModelErrors(ModelState)); ; // Returns validation errors
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new ResponseModel(400, "User already exists !!!");
            }

            // Ensure the SuperAdmin role exists
            var role = await _roleManager.FindByNameAsync("User");
            if (role == null)
            {
                var roleCreationResult = await _roleManager.CreateAsync(new AppRole { Name = "User" });
                if (!roleCreationResult.Succeeded)
                {
                    // Handle role creation failure (e.g., log or throw an exception)
                    return new ResponseModel(400, $"Failed to create role: {"User"}");
                }
            }

            var user = new AppUser
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleAssignmentResult.Succeeded)
                {
                    // Handle role assignment failure
                    return new ResponseModel(400, "Failed to assign  role to the user.");
                }

                return new ResponseModel(200, "User Registered Successfully !", result);
            }

            return new ResponseModel(400, "Unable to create User !!!");

        }
    }
}
