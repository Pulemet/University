using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using University.Models.Dto;

namespace University.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string groupId, MessageDto msg)
        {
            Clients.Group(groupId).sendMessage(msg);
        }

        public void AddMembers(string groipId, DialogDto dialog)
        {
            Clients.Group(groipId).addMembers(dialog);
        }

        public void Connect(string groupId)
        {
            var id = Context.ConnectionId;

            Groups.Add(id, groupId);
        }

        public void OnDisconnected(string groupId)
        {
            var id = Context.ConnectionId;
            Groups.Remove(id, groupId);
        }
    }
}