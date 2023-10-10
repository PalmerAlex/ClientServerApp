using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // This method gets called from chat.js when the send message button is pressed
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}