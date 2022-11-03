using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.HubConfig
{
    public class BMSHub : Hub<IHubClient>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BMSHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task CheckLogout(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            await Clients.Caller.SendLogoutStatus(user.LockoutEnabled);

        }
    }
}
