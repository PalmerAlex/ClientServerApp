using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private RoomController roomController = new();
        public async Task SendMessage(string user, string message, string room)
        {
            // This method gets called from chat.js when the send message button is pressed
            var clientId = Context.ConnectionId;
            var targetRoomList = this.roomController.SearchRooms(clientId);

            if (targetRoomList != null)
            {
                // Only sends message if the target room has clients connected
                // This should never happen as to send a message, at least yourself must be in that room
                //await Clients.Users(targetRoomList).SendAsync("ReceiveMessage", user, message, room);
                await Clients.User(targetRoomList[0]).SendAsync("ReceiveMessage", user, message, room);
            }




            // await Clients.All.SendAsync("ReceiveMessage", user, message, room);
        }

        public async Task RoomConnect(string portId)
        {
            // This method gets called from chat.js when the connect room button is pressed
            await roomController.AddClientToRoom(Context.ConnectionId, portId);
            Console.WriteLine(Context.ConnectionId);
            roomController.ShowRooms();
        }
        private class RoomController
        {
            // This is an inner class that manages the port list functionality
            private static List<Room> roomList = new();
            public async Task<IResult> AddClientToRoom(string clientId, string roomId)
            {
                if (SearchRooms(clientId) != null)
                {
                    RemoveClientFromRoom(roomId, clientId);
                }
                // Disconnects client from their current port before adding them to a new one

                foreach (Room room in roomList)
                {
                    if (room.FetchRoomId() == roomId)
                    {
                        room.ConnectUser(clientId);
                        return Results.Ok();
                        // Exits the method if the target room already exists
                    }
                }
                await CreateRoom(clientId, roomId);
                // If a client tries to connect to a non-existant room, it creates one and adds them to it
                return Results.Ok();
            }

            public List<string> SearchRooms(string targetClientId)
            {
                foreach (Room room in roomList)
                {
                    
                    if (room.CheckForClient(targetClientId))
                    {
                        return room.FetchClientIdList();
                    }
                }
                return null;
            }
            public void RemoveClientFromRoom(string roomId, string clientId)
            {
                foreach (Room room in roomList)
                {
                    if (room.FetchRoomId() == roomId)
                    {
                        room.FetchClientIdList().Remove(clientId);
                    }
                }
            }

            private async Task<IResult> CreateRoom(string clientId, string roomId)
            {
                // Makes a new room with the given Id, then adds the given client to that room
                roomList.Add(new Room(roomId, clientId));
                return Results.Ok();
            }

            public void ShowRooms(){
                foreach (Room room in roomList)
                {
                    Console.WriteLine(room.ToString());
                }
            }
            private class Room
            {
                // This is an inner class that manages individual room behaviour
                private string roomId;
                private List<string> clientIdList = new();

                public Room(string roomId, string clientId)
                {
                    this.roomId = roomId;
                    ConnectUser(clientId);
                }
                public List<string> FetchClientIdList()
                {
                    return this.clientIdList;
                }
             
                public string FetchRoomId()
                {
                    return this.roomId;
                }
                public void ConnectUser(string clientId)
                {
                    this.clientIdList.Add(clientId);
                }
                public bool CheckForClient(string clientId)
                {
                    if (this.clientIdList.Contains(clientId))
                    {
                        return true;
                    }
                    return false;
                }
                public override string ToString()
                {
                    return "RoomId: " + this.roomId + " Clients: " + this.clientIdList.ToString();
                }
            }
        }

    }

}