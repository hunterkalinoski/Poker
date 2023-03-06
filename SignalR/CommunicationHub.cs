using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR
{
    public class CommunicationHub : Hub
    {
        public async Task Win()
        {
            await Clients.Others.SendAsync("OpponentWon");
        }

        public async Task Lose()
        {
            await Clients.Others.SendAsync("OpponentLost");
        }

        public async Task Fold()
        {
            await Clients.Others.SendAsync("OpponentFolded");
        }

        public async Task Check()
        {
            await Clients.Others.SendAsync("OpponentChecked");
        }

        public async Task Call()
        {
            await Clients.Others.SendAsync("OpponentCalled");
        }

        public async Task Bet(int x)
        {
            await Clients.Others.SendAsync("OpponentBetted", x);
        }

        public async Task GameWin()
        {
            await Clients.Others.SendAsync("OppGameWin");
        }

        public async Task GameLose()
        {
            await Clients.Others.SendAsync("OppGameLose");
        }

        public async Task Quit(int amount)
        {
            await Clients.Others.SendAsync("OppQuit", amount);
        }

        public async Task FlipNext()
        {
            await Clients.Others.SendAsync("FlipNextCard");
        }

        public async Task SendInt(int x)
        {
            await Clients.Others.SendAsync("ReceiveInt", x);
        }

        public async Task ReadyToStart()
        {
            await Clients.All.SendAsync("StartGame");
        }

        public override Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
            this.Clients.Others.SendAsync("OtherConnection");
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            base.OnDisconnectedAsync(exception);
            this.Clients.Others.SendAsync("OtherDisconnected");
            return Task.CompletedTask;
        }
    }
}
