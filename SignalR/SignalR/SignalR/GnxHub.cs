using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;



namespace SignalR
{
    public class GnxHub : Hub
    {
        static List<string> users = new List<string>();


        public class MessageObject
        {
            public string roomId;
            public dynamic data;
        }

        public void Hello(dynamic data)
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }


        public void GadgetDropped(MessageObject data)
        {
            //Clients.All.joinedRoom("joindedGroup", "xxxxxxxxxxxxxx");
            //Clients.Client(Context.ConnectionId).gadgetDropped(data);
            //Clients.Group(data.roomId).joinedRoom("gadgetDropped", data);
            Clients.Group(data.roomId).gadgetDropped("gadgetDropped", data);
            //Clients.All.gadgetDropped("gadgetDropped", data);
        }

        public async Task JoinRoom(string roomName, string clientName)
        {
            //return Groups.Add(Context.ConnectionId, roomName);
            await Groups.Add(Context.ConnectionId, roomName);

            // method on client will be called - to all in group
            string eventName = "joindedGroup";
            Clients.Group(roomName).joinedRoom(eventName, clientName);
            
            //Clients.All.joinedRoom(eventName, clientName);
            //Clients.Client(Context.ConnectionId).joinedRoom(eventName, clientName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }



        public void UserLoggedIn(dynamic model)
        {
            string roomId = model.roomId;
            Clients.Group(roomId).userLoggedInSuccess("userLoggedInSuccess", model);
            Clients.All.userLoggedInSuccess("userLoggedInSuccess", model);
        }


        public void UserLoggedOff(dynamic model)
        {

            string roomId = model.roomId;

            if (!string.IsNullOrEmpty(roomId))
            {
                Clients.Group(roomId).userLoggedOffSuccess("UserLoggedOffSuccess", model);
            }
            Clients.All.userLoggedOffSuccess("UserLoggedOffSuccess", model);
        }



        /// <summary>
        /// Overwrite for OnConnected event
        /// Pass back to the client its SignalR id (uuid)
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnConnected()
        {
            string clientId = GetClientId();

        
            if (users.IndexOf(clientId) == -1)
            {
                users.Add(clientId);
            }

            ShowUsersOnLine();

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            if (users.IndexOf(clientId) == -1)
            {
                users.Add(clientId);
            }

            ShowUsersOnLine();

            return base.OnReconnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string clientId = GetClientId();

            if (users.IndexOf(clientId) > -1)
            {
                users.Remove(clientId);
            }

            ShowUsersOnLine();

            return base.OnDisconnected();
        }


        private string GetClientId()
        {
            string clientId = "";
            if (!(Context.QueryString["clientId"] == null))
            {
                //clientId passed from application 
                clientId = Context.QueryString["clientId"].ToString();
            }

            if (clientId.Trim() == "")
            {
                //default clientId: connectionId 
                clientId = Context.ConnectionId;
            }
            return clientId;

        }
        

        public void ShowUsersOnLine()
        {
            Clients.All.showUsersOnLine(users.Count);
        }
    }
}