using Microsoft.AspNetCore.SignalR.Client;
using Poker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Poker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        private HubConnection hubConnection;
        bool isConnected;
        bool first;

        public MainPage()
        {
            InitializeComponent();
            hubConnection = new HubConnectionBuilder().WithUrl("http://10.0.2.2:5000/communicationHub").Build();
            isConnected = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            first = false;
            hubConnection = new HubConnectionBuilder().WithUrl("http://10.0.2.2:5000/communicationHub").Build();
            isConnected = false;
        }

        private async void Play_Button_Clicked(object sender, EventArgs e)
        {
            if (!ValidName())
            {
                await DisplayAlert("Error", "Please enter a name.", "OK");
                return;
            }

            ActivityIndicator.IsVisible = true;
            CancelButton.IsVisible = true;
            ConnectionStatusLabel.IsVisible = true;
            ConnectionStatusLabel.Text = "Connecting...";

            string name = NameEntry.Text;

            //when another player connects, call hub's ReadyToStart
            hubConnection.On("OtherConnection", async () =>
            {
                first = true;
                await hubConnection.InvokeAsync("ReadyToStart");
            });

            hubConnection.On("StartGame", () =>
            {
                MoveToGamePage(name);
                ActivityIndicator.IsVisible = false;
                CancelButton.IsVisible = false;
                ConnectionStatusLabel.Text = "";
                ConnectionStatusLabel.IsVisible = false;
            });

            await Connect();
        }

        private void Leaderboard_Button_Clicked(object sender, EventArgs e)
        {
            MoveToLeaderboardPage();
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await Disconnect();
            ActivityIndicator.IsVisible = false;
            CancelButton.IsVisible = false;
            ConnectionStatusLabel.Text = "";
            ConnectionStatusLabel.IsVisible = false;
        }

        private bool ValidName()
        {
            if (NameEntry.Text == "" || NameEntry.Text == null)
            {
                return false;
            }
            return true;
        }

        private async Task Connect()
        {
            if (isConnected)
                return;

            try
            {
                await hubConnection.StartAsync();
                isConnected = true;
                ConnectionStatusLabel.Text = "Connection Success!";
                await Task.Delay(2000);
                ConnectionStatusLabel.Text = "Waiting for another user to connect...";
            }
            catch (Exception)
            {
                ConnectionStatusLabel.Text = "Connection failed... ";
                await Disconnect();
            }
        }

        private async Task Disconnect()
        {
            if (!isConnected)
                return;

            await hubConnection.StopAsync();
            isConnected = false;
            hubConnection = new HubConnectionBuilder().WithUrl("http://10.0.2.2:5000/communicationHub").Build();
        }

        private async void MoveToGamePage(string name)
        {
            await Navigation.PushAsync(new GamePage(first, hubConnection, name));
        }

        private async void MoveToLeaderboardPage()
        {
            await Navigation.PushAsync(new LeaderboardPage());
        }
    }
}
