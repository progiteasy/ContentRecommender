using ContentRecommender.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContentRecommender.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task UpdateStatusAsync(this UserManager<User> userManager, User user, bool isActiveStatus)
        {
            user.IsActive = isActiveStatus;

            await userManager.UpdateAsync(user);
        }
    }
}
