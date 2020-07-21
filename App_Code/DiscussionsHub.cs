using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

public class DiscussionsHub : Hub
{
    [HubMethodName("sendMessages")]
    public static void Show()
    {
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DiscussionsHub>();
        context.Clients.All.updateMessages();
    }
}
