using Newtonsoft.Json;
using Poker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Poker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderboardPage : ContentPage
    {
        ObservableCollection<LeaderboardItem> list;
        HttpClient client;


        public LeaderboardPage()
        {
            InitializeComponent();
            _ = UpdateList();
        }

        protected override void OnAppearing()
        {
            _ = UpdateList();
        }

        private async void Refresh_Button_Clicked(object sender, EventArgs e)
        {
            await UpdateList();
        }

        private async void Generate_Button_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            this.Content.IsEnabled = false;

            LeaderboardItem item1 = new LeaderboardItem
            {
                Name = "Player 1",
                Score = 0
            };
            LeaderboardItem item2 = new LeaderboardItem
            {
                Name = "Player 2",
                Score = 200
            };
            await Post(item1);
            await Post(item2);
            await UpdateList();

            activityIndicator.IsVisible = false;
            this.Content.IsEnabled = true;
        }

        private void LeaderboardListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int index = e.ItemIndex;
            DetailPage detailPage = new DetailPage(index, list.ToList<LeaderboardItem>());
            Navigation.PushAsync(detailPage);
        }

        // performs a GET from the web service
        private async Task UpdateList()
        {
            activityIndicator.IsVisible = true;
            this.Content.IsEnabled = false;

            client = new HttpClient();
            Uri uri = new Uri("http://10.0.2.2:51905/api/leaderboarditems");
            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                if (content.Equals("null"))
                {
                    await DisplayAlert("Error", "Failed to retrieve data", "OK");
                }
                List<LeaderboardItem> unsortedList = JsonConvert.DeserializeObject<List<LeaderboardItem>>(content);

                list = Sort(unsortedList);
                LeaderboardListView.ItemsSource = list;
            }
            else
            {
                await DisplayAlert("Error", "Failed to retrieve data", "OK");
            }

            activityIndicator.IsVisible = false;
            this.Content.IsEnabled = true;
        }

        private async Task Post(LeaderboardItem item)
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
                await DisplayAlert("ERROR", "Unable to generate data", "OK");
            }
        }

        private ObservableCollection<LeaderboardItem> Sort(List<LeaderboardItem> list)
        {
            ObservableCollection<LeaderboardItem> sortedList = new ObservableCollection<LeaderboardItem>();

            List<LeaderboardItem> list2 = list.OrderByDescending(o => o.Score).ToList();

            foreach (LeaderboardItem item in list2)
            {
                sortedList.Add(item);
            }

            return sortedList;
        }
    }
}