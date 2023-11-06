using System.Text.RegularExpressions;
using ClientServerApp.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private RoomController roomController;

        public ChatHub()
        {
            this.roomController = new(Groups);
        }
        public async Task SendMessage(string user, string message, string room)
        {
            // This method gets called from chat.js when the send message button is pressed

            string? targetRoomId = this.roomController.SearchRooms(Context.ConnectionId);
            // Finds the room the sender client is currently in
           
            if (targetRoomId != null)
            {
                // Only sends message if the target room has clients connected
                
                await Clients.Group(targetRoomId).SendAsync("ReceiveMessage", user, message, room);
                // Calls ReceieveMessage on all the clients in the sender clients room
            }
            
        }

        public async Task RoomConnect(string portId)
        {
            // This method gets called from chat.js when the connect room button is pressed

            await roomController.AddClientToRoom(Context.ConnectionId, portId);
        }

        private class RoomController
        {
            // This is an inner class that manages the SignalR Group functionality
            private IGroupManager groupManager;
            private static Dictionary<string,string> roomConnections = new Dictionary<string, string>();
            
            public RoomController(IGroupManager manager)
            {
                this.groupManager = manager;
            }
            public async Task<IResult> AddClientToRoom(string clientId, string roomId)
            {
                string? groupId = SearchRooms(clientId);
                if(groupId != null){
                    await groupManager.RemoveFromGroupAsync(clientId,groupId);
                    roomConnections.Remove(clientId);
                }
                // Disconnect client from their current port before adding them to a new one

                await groupManager.AddToGroupAsync(clientId,roomId);
                roomConnections.Add(clientId,roomId);
                // If a client tries to connect to a non-existant room, it creates one and adds them to it
                return Results.Ok();
            }

            public string? SearchRooms(string targetClientId)
            {
                string? groupId;
                roomConnections.TryGetValue(targetClientId, out groupId);
                return groupId;
            }
        }

    }

}