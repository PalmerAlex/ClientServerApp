using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ClientServerApp.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> roomConnections = new Dictionary<string, string>();

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            RemoveClientFromRoom(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        
        #region MessageSending
        public async Task SendMessage(string message)
        {
            // This method gets called from chat.js when the send message button is pressed

            string? targetRoomId = await SearchRooms(Context.ConnectionId);
            // Finds the room the sender client is currently in

            if (targetRoomId != null)
            {
                // Only sends message if the target room has clients connected

                await Clients.Group(targetRoomId).SendAsync("ReceiveMessage", Context.ConnectionId[..6], message);
                // Calls ReceieveMessage on all the clients in the sender clients room
            }

        }
        public async Task SendGif()
        {
            // This method gets called from chat.js when the send message button is pressed

            string? targetRoomId = await SearchRooms(Context.ConnectionId);
            // Finds the room the sender client is currently in

            if (targetRoomId != null)
            {
                // Only sends message if the target room has clients connected

                await Clients.Group(targetRoomId).SendAsync("ReceiveGif", Context.ConnectionId[..6]);
                // Calls ReceieveGif on all the clients in the sender clients room
            }

        }

        #endregion
        #region RoomFunctionality
        public async Task<bool> RoomConnect(string portId)
        {
            // This method gets called from chat.js when the connect room button is pressed

            await AddClientToRoom(Context.ConnectionId, portId);
            return true;

        }
        private async Task<IResult> AddClientToRoom(string clientId, string roomId)
        {
            await RemoveClientFromRoom(clientId);
            // Disconnect client from their current room before adding them to a new one

            await Groups.AddToGroupAsync(clientId, roomId);
            roomConnections.Add(clientId, roomId);
            // If a client tries to connect to a non-existant room, it creates one and adds them to it
            return Results.Ok();
        }
        private async Task<IResult> RemoveClientFromRoom(string clientId)
        {
            string? groupId = await SearchRooms(clientId);
            if (groupId != null)
            {
                await Groups.RemoveFromGroupAsync(clientId, groupId);
                roomConnections.Remove(clientId);
            }
            return Results.Ok();
        }
        private async Task<string?> SearchRooms(string targetClientId)
        {
            string? groupId;
            roomConnections.TryGetValue(targetClientId, out groupId);
            return groupId;
        }
        #endregion
    }

}