using BgServicex.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BgServicex.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityUser> GetUserAsync(Guid id);
        Task<IdentityUser> GetUserAsync(string email);
        Task<bool> IsUserInRoleAsync(IdentityUser user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}