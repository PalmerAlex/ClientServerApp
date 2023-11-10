using System.Text.RegularExpressions;
using ClientServerApp.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> roomConnections = new Dictionary<string, string>();

        public async Task SendMessage(string user, string message)
        {
            // This method gets called from chat.js when the send message button is pressed

            string? targetRoomId = await SearchRooms(Context.ConnectionId);
            // Finds the room the sender client is currently in

            if (targetRoomId != null)
            {
                // Only sends message if the target room has clients connected

                await Clients.Group(targetRoomId).SendAsync("ReceiveMessage", user, message, targetRoomId);
                // Calls ReceieveMessage on all the clients in the sender clients room
            }

        }
        public async Task SendGif(string user)
        {
            // This method gets called from chat.js when the send message button is pressed

            string? targetRoomId = await SearchRooms(Context.ConnectionId);
            // Finds the room the sender client is currently in

            if (targetRoomId != null)
            {
                // Only sends message if the target room has clients connected

                await Clients.Group(targetRoomId).SendAsync("ReceiveGif", user, targetRoomId);
                // Calls ReceieveGif on all the clients in the sender clients room
            }

        }

        public async Task RoomConnect(string portId)
        {
            // This method gets called from chat.js when the connect room button is pressed

            await AddClientToRoom(Context.ConnectionId, portId);
        }


        private async Task<IResult> AddClientToRoom(string clientId, string roomId)
        {
            string? groupId = await SearchRooms(clientId);
            if (groupId != null)
            {
                await Groups.RemoveFromGroupAsync(clientId, groupId);
                roomConnections.Remove(clientId);
            }
            // Disconnect client from their current port before adding them to a new one

            await Groups.AddToGroupAsync(clientId, roomId);
            roomConnections.Add(clientId, roomId);
            // If a client tries to connect to a non-existant room, it creates one and adds them to it
            return Results.Ok();
        }

        private async Task<string?> SearchRooms(string targetClientId)
        {
             string? groupId;
            roomConnections.TryGetValue(targetClientId, out groupId);
            return groupId;
        }
    }
}