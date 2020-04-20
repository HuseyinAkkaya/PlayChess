using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Chess.Entities;
using Chess.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace Chess.Hubs
{
    public class ChessHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
        UserConnectionService userConnection = new UserConnectionService();
        [Authorize]
        public void SendChatMessage(string receiverId, string message)
        {
            List<UserConnection> userConnections = userConnection.GetConnections(receiverId);

            if (userConnections == null)
            {
                Clients.Caller.showErrorMessage("Could not find that user.");
            }
            else
            {
                userConnections.ForEach(e =>
                {
                    Clients.Client(e.Connectionid).addChatMessage(receiverId, message);
                });

            }

        }
        [Authorize]
        public override Task OnConnected()
        {

            var userId = Context.User.Identity.GetUserId();

            var ConnectionID = Context.ConnectionId;

            userConnection.AddConnection(userId, ConnectionID);
            return base.OnConnected();
        }
        [Authorize]
        public override Task OnDisconnected(bool stopCalled)
        {
            userConnection.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            var userId = Context.User.Identity.GetUserId();
            var ConnectionID = Context.ConnectionId;
            List<UserConnection> userConnections = userConnection.GetConnections(userId);

            if (!userConnections.Any(e => e.Connectionid == ConnectionID && e.Userid == userId))
            {
                userConnection.AddConnection(userId, ConnectionID);
            }


            return base.OnReconnected();
        }

    }
}