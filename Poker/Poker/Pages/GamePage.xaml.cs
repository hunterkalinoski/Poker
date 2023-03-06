using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poker.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;

namespace Poker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        bool first;
        bool turn;
        readonly HubConnection hubConnection;
        string name;
        Deck deck;
        Hand yourHand;
        Hand oppHand;
        int yourMoney;
        int oppMoney;
        int potMoney;
        int turnCount;
        int round;
        int bet;

        public GamePage(bool first, HubConnection hubConnection, string name)
        {
            InitializeComponent();

            this.first = first;
            this.hubConnection = hubConnection;
            this.name = name;

            this.hubConnection.On("OtherDisconnected", async () =>
            {
                await OtherDisconnected();
            });
            this.hubConnection.On<int>("ReceiveInt", (int seed) =>
            {
                if (round > 0)
                {
                    InitGame();
                }
                deck = new Deck();
                deck.Shuffle(seed);
                FlipFirstCards();
                PerformStartingBet();
            });
            this.hubConnection.On("OpponentFolded", () =>
            {
                yourMoney += potMoney;
                InitGame();
            });
            this.hubConnection.On("OpponentChecked", () => 
            {
                turnCount = 1;
                turn = true;
                ShowOptions(turn);
            });
            this.hubConnection.On<int>("OpponentBetted", (betAmount) =>
            {
                //bet already exists
                if (bet != 0)
                {
                    oppMoney -= bet;
                    potMoney += bet;
                }
                oppMoney -= betAmount;
                potMoney += betAmount;
                OpponentMoneyLabel.Text = String.Format("Opponent: ${0}",oppMoney.ToString());
                PotMoneyLabel.Text = String.Format("Pot: ${0}", potMoney.ToString());
                turn = true;
                bet = betAmount;
                ShowOptions(turn);
                CurrentStateLabel.Text = String.Format("${0} to call.", betAmount);
            });
            this.hubConnection.On("OpponentCalled", () =>
            {
                if (bet > oppMoney) bet = oppMoney;
                oppMoney -= bet;
                potMoney += bet;
                OpponentMoneyLabel.Text = String.Format("Opponent: ${0}", oppMoney.ToString());
                PotMoneyLabel.Text = String.Format("Pot: ${0}", potMoney.ToString());
                turn = first;
                bet = 0;
                ShowOptions(turn);
                FlipNextCard();
            });
            this.hubConnection.On("FlipNextCard", () =>
            {
                turn = true;
                ShowOptions(turn);
                FlipNextCard();
            });
            this.hubConnection.On("OpponentWon", async () =>
            {
                oppMoney += potMoney;
                potMoney = 0;
                await InitGame();
            });
            this.hubConnection.On("OpponentLost", async () =>
            {
                yourMoney += potMoney;
                potMoney = 0;
                await InitGame();
            });
            this.hubConnection.On("OppGameLose", async () =>
            {
                yourMoney += potMoney;
                potMoney = 0;
                await DisplayAlert("You win!", "Your opponent is out of money", "OK");
                await Quit();
            });
            this.hubConnection.On("OppGameWin", async () =>
            {
                await DisplayAlert("You lose!", "You are out of money", "OK");
                await Quit();
            });
            this.hubConnection.On<int>("OppQuit", async (amount) =>
            {
                yourMoney += amount;
                await SaveResults();
                await Navigation.PopAsync();
                await hubConnection.StopAsync();
            });


            yourMoney = 100;
            oppMoney = 100;
            InitGame();
        }

        private async Task InitGame()
        {
            potMoney = 0;
            PlayerMoneyLabel.Text = String.Format("You: ${0}", yourMoney);
            PotMoneyLabel.Text = String.Format("Pot: ${0}", potMoney);
            OpponentMoneyLabel.Text = String.Format("Opponent: ${0}", oppMoney);
            PocketCardImage1.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            PocketCardImage2.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            CommunityCardImage1.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            CommunityCardImage2.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            CommunityCardImage3.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            CommunityCardImage4.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");
            CommunityCardImage5.Source = ImageSource.FromResource("Poker.Images.back_of_card_blue.png");

            yourHand = new Hand();
            oppHand = new Hand();

            turnCount = 0;
            round = 0;
            bet = 0;


            ShowOptions(first);
            //one client creates a deck and sends it to the other client
            if (first)
            {
                int seed = new Random().Next();
                deck = new Deck();
                deck.Shuffle(seed);

                await hubConnection.InvokeAsync("SendInt", seed);
                FlipFirstCards();
                PerformStartingBet();
            }
        }

        private async void QuitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("WARNING","You are about to leave.\nResults will be recorded anyway.\n" +
                                                        "Are you sure you want to quit?","YES","NO");
            if (answer)
            {
                int amount = potMoney / 2;
                yourMoney += amount;
                await Quit(amount);
            }
        }

        private async void FoldButton_Clicked(object sender, EventArgs e)
        {
            if (yourMoney ==0)
            {
                await DisplayAlert("???","Why would you fold with $0 left???\njust finish the round","sorry");
            }
            oppMoney += potMoney;
            await hubConnection.InvokeAsync("Fold");
            await InitGame();
        }

        private async void CheckButton_Clicked(object sender, EventArgs e)
        {
            //for calling, rather than checking
            if (bet != 0)
            {
                if (bet > yourMoney) bet = yourMoney;
                yourMoney -= bet;
                potMoney += bet;
                PlayerMoneyLabel.Text = String.Format("You: ${0}", yourMoney.ToString());
                PotMoneyLabel.Text = String.Format("Pot: ${0}", potMoney.ToString());
                FlipNextCard();
                await hubConnection.InvokeAsync("Call");
                turn = first;
                ShowOptions(turn);
                return;
            }
            //both players checked
            else if (turnCount == 1)
            {
                FlipNextCard();
                if (round < 3)
                    await hubConnection.InvokeAsync("FlipNext");
            }
            else
            {
                await hubConnection.InvokeAsync("Check");
            }
            turn = false;
            ShowOptions(turn);
        }

        private async void BetButton_Clicked(object sender, EventArgs e)
        {
            int betAmount;
            if (!int.TryParse(BetAmountEntry.Text, out betAmount))
            {
                await DisplayAlert("ERROR", "You must enter an amount to bet", "OK");
                return;
            }
            if (betAmount > yourMoney)
            {
                betAmount = yourMoney;
            }

            //bet already exists, this is a raise
            if (bet != 0)
            {
                if (betAmount <= bet)
                {
                    if (!(betAmount == yourMoney))
                    {
                        await DisplayAlert("ERROR", String.Format("Your bet amount is lower than the current call (${0})", bet), "OK");
                        return;
                    }
                    //else //all in
                }
            }

            yourMoney -= betAmount;
            potMoney += betAmount;
            PlayerMoneyLabel.Text = String.Format("You: ${0}",yourMoney.ToString());
            PotMoneyLabel.Text = String.Format("Pot: ${0}",potMoney.ToString());
            turnCount = 1;

            //for betting
            if (bet == 0)
                bet = betAmount;
            else //for raising
                bet = betAmount - bet;


            await hubConnection.InvokeAsync("Bet", bet);
            turn = false;
            ShowOptions(turn);
        }

        private void FlipFirstCards()
        {
            if (first)
            {
                yourHand.Add(deck.Draw());
                yourHand.Add(deck.Draw());
                oppHand.Add(deck.Draw());
                oppHand.Add(deck.Draw());
            }
            else
            {
                oppHand.Add(deck.Draw());
                oppHand.Add(deck.Draw());
                yourHand.Add(deck.Draw());
                yourHand.Add(deck.Draw());
            }
            Card c = deck.Draw();
            yourHand.Add(c);
            oppHand.Add(c);
            Card c2 = deck.Draw();
            yourHand.Add(c2);
            oppHand.Add(c2);
            Card c3 = deck.Draw();
            yourHand.Add(c3);
            oppHand.Add(c3);

            PocketCardImage1.Source = ImageSource.FromResource(yourHand.Get(0).Image);
            PocketCardImage2.Source = ImageSource.FromResource(yourHand.Get(1).Image);
            CommunityCardImage1.Source = ImageSource.FromResource(yourHand.Get(2).Image);
            CommunityCardImage2.Source = ImageSource.FromResource(yourHand.Get(3).Image);
            CommunityCardImage3.Source = ImageSource.FromResource(yourHand.Get(4).Image);
        }

        private async void FlipNextCard()
        {
            round++;
            Card c = deck.Draw();
            yourHand.Add(c);
            oppHand.Add(c);
            bet = 0;
            turnCount = 0;
            if (round == 1)
            {
                CommunityCardImage4.Source = ImageSource.FromResource(yourHand.Get(5).Image);
            }
            else if (round == 2)
            {
                CommunityCardImage5.Source = ImageSource.FromResource(yourHand.Get(6).Image);
            }
            else //all 5 cards are already revealed, at this point hands need to be evaluated
            {
                if (DidWin())
                {
                    yourMoney += potMoney;
                    potMoney = 0;

                    if (oppMoney <= 0)
                    {
                        await hubConnection.InvokeAsync("GameWin");
                        await SaveResults();
                        await DisplayAlert("Your opponent is out of money", "", "OK");
                        return;
                    }

                    await hubConnection.InvokeAsync("Win");
                }
                else
                {
                    oppMoney += potMoney;
                    potMoney = 0;

                    if (yourMoney <= 0)
                    {
                        await hubConnection.InvokeAsync("GameLose");
                        await SaveResults();
                        await DisplayAlert("You are out of money", "", "OK");
                        return;
                    }
                    await hubConnection.InvokeAsync("Lose");
                }
                await InitGame();
            }
        }

        private void PerformStartingBet()
        {
            yourMoney -= 5;
            oppMoney -= 5;
            potMoney += 10;
            PlayerMoneyLabel.Text = String.Format("You: ${0}", yourMoney);
            OpponentMoneyLabel.Text = String.Format("Opponent: ${0}", oppMoney);
            PotMoneyLabel.Text = String.Format("Pot: ${0}", potMoney);
        }

        private void ShowOptions(bool b)
        {
            if (bet != 0)
            {
                BetButton.Text = "Raise";
                CheckButton.Text = "Call";
            }
            else
            {
                BetButton.Text = "Bet";
                CheckButton.Text = "Check";
            }
            BetAmountEntry.Text = "";
            BetAmountEntry.IsVisible = b;
            BetButton.IsVisible = b;
            CheckButton.IsVisible = b;
            FoldButton.IsVisible = b;
            CurrentStateLabel.Text = b ? "Your turn!" : "Waiting for opponent...";
        }

        private async Task OtherDisconnected()
        {
            await DisplayAlert("Opponent Disconnected","Returning to Main Menu... \nThis game's results will be recorded.","OK");
            await Quit();
        }

        private async Task Quit()
        {
            await SaveResults();
            await Navigation.PopAsync();
            await hubConnection.StopAsync();
        }
        private async Task Quit(int amount)
        {
            await SaveResults();
            await hubConnection.InvokeAsync("Quit", amount);
            await Navigation.PopAsync();
            await hubConnection.StopAsync();
        }

        private async Task SaveResults()
        {
            List<LeaderboardItem> list = await GetList();

            bool exists = true;

            var record = list.FirstOrDefault<LeaderboardItem>(x => x.Name.Equals(name));

            if (list.Where<LeaderboardItem>(x => x.Name.Equals(name)).Count() == 0)
            {
                exists = false;
            }

            LeaderboardItem item = new LeaderboardItem
            {
                Name = name
            };

            if (exists)
            {
                item.Id = record.Id;
                item.Score = record.Score + yourMoney - 100;
                await PutScore(item);
            }
            else
            {
                item.Score = yourMoney - 100;
                await PostScore(item);
            }
        }

        private async Task PutScore(LeaderboardItem item)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(String.Format("http://10.0.2.2:51905/api/leaderboarditems/{0}", item.Id));

            String json = JsonConvert.SerializeObject(item);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Put;
            request.RequestUri = uri;
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("ERROR", "Unable to edit", "OK");
            }
        }

        private async Task PostScore(LeaderboardItem item)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://10.0.2.2:51905/api/leaderboarditems");

            String json = JsonConvert.SerializeObject(item);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = uri;
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
            }
            else
            {
                await DisplayAlert("ERROR", "Failed to save score", "OK");
            }
        }

        private async Task<List<LeaderboardItem>> GetList()
        {
            List<LeaderboardItem> list = new List<LeaderboardItem>();

            activityIndicator.IsVisible = true;

            var client = new HttpClient();
            Uri uri = new Uri("http://10.0.2.2:51905/api/leaderboarditems");
            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                if (content.Equals("null"))
                {
                    await DisplayAlert("Error", "Failed to save score", "OK");
                    return null;
                }
                list = JsonConvert.DeserializeObject<List<LeaderboardItem>>(content);
            }
            else
            {
                await DisplayAlert("Error", "Failed to save score", "OK");
                return null;
            }
            activityIndicator.IsVisible = false;
            return list;
        }

        private bool DidWin()
        {
            if (yourHand.Beats(oppHand))
                return true;
            else return false;
        }

    }
}