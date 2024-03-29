﻿using FinancialChat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FinancialChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message) =>
            await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
