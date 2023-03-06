using Newtonsoft.Json;
using Poker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Poker.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {

        LeaderboardItem selectedItem;

        public DetailPage(int index, List<LeaderboardItem> list)
        {
            InitializeComponent();
            selectedItem = list.ElementAt(index);
            FillEntries(selectedItem);
        }

        private void FillEntries(LeaderboardItem item)
        {
            NameEntry.Text = item.Name;
            ScoreEntry.Text = item.Score.ToString();
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            await EditItem(selectedItem);
        }

        private LeaderboardItem CreateItemFromEntries()
        {
            LeaderboardItem item = new LeaderboardItem();
            item.Name = NameEntry.Text;
            int.TryParse(ScoreEntry.Text, out int score);
            item.Score = score;

            return item;
        }

        private async Task<LeaderboardItem> EditItem(LeaderboardItem item)
        {
            if (NameEntry.Text == null || NameEntry.Text.Equals(""))
            {
                await DisplayAlert("Error", "Leaderboard entries must have a name.", "OK");
            }
            else if (ScoreEntry.Text == null || ScoreEntry.Text.Equals(""))
            {
                await DisplayAlert("Error", "Leaderboard entries must have a score.", "OK");
            }
            else
            {
                LeaderboardItem newItem = CreateItemFromEntries();
                newItem.Id = selectedItem.Id;
                Put(newItem);
                return newItem;
            }
            return null;
        }

        public async void Put(LeaderboardItem item)
        {
            activityIndicator.IsVisible = true;
            this.Content.IsEnabled = false;

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
            activityIndicator.IsVisible = false;
            this.Content.IsEnabled = true;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsVisible = true;
            this.Content.IsEnabled = false;
            await Delete(selectedItem);
            activityIndicator.IsVisible = false;
            this.Content.IsEnabled = true;
        }

        private async Task Delete(LeaderboardItem item)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri(String.Format("http://10.0.2.2:51905/api/leaderboarditems/{0}",item.Id));

            HttpResponseMessage response = await client.DeleteAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("ERROR", "Unable to delete", "OK");
            }
        }
    }
}