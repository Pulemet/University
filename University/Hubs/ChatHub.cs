using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace University.Hubs
{
    [HubName("msg")]
    public class ChatHub : Hub
    {
        //public Task Send(dynamic message)
        //{
        //    return Clients.All.SendMessage(message);
        //}

        public void Register(long userId)
        {
            Groups.Add(Context.ConnectionId, userId.ToString(CultureInfo.InvariantCulture));

        }

        public Task Send(dynamic id, string message)
        {
            return Clients.Group(id.ToString()).SendMessage(message);
        }

    }
}