using System.Threading.Tasks;

namespace WebAPI.HubConfig
{
    public interface IHubClient
    {
        Task BroadcastMessage(string email);

        Task CheckLogout(string userID);

        Task SendLogoutStatus(bool status);

    }
}
