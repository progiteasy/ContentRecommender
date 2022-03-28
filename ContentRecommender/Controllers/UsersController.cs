using ContentRecommender.Data.Models;
using ContentRecommender.Extensions;
using ContentRecommender.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentRecommender.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
        }

        private async Task<IActionResult> ChangeStatusAsync(IEnumerable<UserViewModel> userModels, bool isActiveStatus)
        {
            var selectedUserModels = userModels.Where(userModel => userModel.IsSelected);

            foreach (var userModel in selectedUserModels)
            {
                var user = await _userManager.FindByIdAsync(userModel.Id);

                if (user == null)
                    continue;

                if ((isActiveStatus && !user.IsActive) || (!isActiveStatus && user.IsActive))
                {
                    if (!isActiveStatus)
                        await _userManager.UpdateSecurityStampAsync(user);
                    await _userManager.UpdateStatusAsync(user, isActiveStatus);
                }
            }

            return Redirect("~/users");
        }

        private async Task<IActionResult> ChangeRoleAsync(IEnumerable<UserViewModel> userModels, string roleName)
        {
            var selectedUserModels = userModels.Where(userModel => userModel.IsSelected);

            foreach (var userModel in selectedUserModels)
            {
                var user = await _userManager.FindByIdAsync(userModel.Id);

                if (user == null)
                    continue;

                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    var oldRoleName = (roleName == "Admin") ? "User" : "Admin";

                    await _userManager.RemoveFromRoleAsync(user, oldRoleName);
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return Redirect("~/users");
        }

        [HttpGet("~/users"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersViewAsync()
        {
            var loggedInUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedInUser == null || !loggedInUser.IsActive)
                return Redirect("~/account/login");

            var users = await _userManager.Users.ToListAsync();
            var userModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRole = (await _userManager.GetRolesAsync(user)).First();

                userModels.Add(new UserViewModel()
                {
                    IsSelected = false,
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate.ToString(),
                    Status = user.IsActive ? "Active" : "Blocked",
                    Role = userRole
                });
            }

            return View("UsersView", userModels);
        }

        [HttpPost("~/users/block"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUsersAsync(IEnumerable<UserViewModel> userModels)
            => await ChangeStatusAsync(userModels, false);

        [HttpPost("~/users/unblock"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockUsersAsync(IEnumerable<UserViewModel> userModels)
            => await ChangeStatusAsync(userModels, true);

        [HttpPost("~/users/delete"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsersAsync(IEnumerable<UserViewModel> userModels)
        {
            var selectedUserModels = userModels.Where(userModel => userModel.IsSelected);
            
            foreach (var userModel in selectedUserModels)
            {
                var user = await _userManager.FindByIdAsync(userModel.Id);

                if (user == null)
                    continue;

                await _userManager.DeleteAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return Redirect("~/users");
        }

        [HttpPost("~/users/assign-as-admins"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignAsAdminsAsync(IEnumerable<UserViewModel> userModels)
            => await ChangeRoleAsync(userModels, "Admin");

        [HttpPost("~/users/assign-as-users"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignAsUsersAsync(IEnumerable<UserViewModel> userModels)
            => await ChangeRoleAsync(userModels, "User");
    }
}
