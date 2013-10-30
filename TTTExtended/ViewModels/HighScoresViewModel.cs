using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace TTTExtended.ViewModels
{
    public class HighScoresViewModel
    {
        ObservableCollection<UserModel> highScores;

        public ObservableCollection<UserModel> HighScores
        {
            get
            {
                if (this.highScores == null)
                {
                    this.highScores = new ObservableCollection<UserModel>();
                }

                return this.highScores;
            }
            set
            {
                if (this.highScores == null)
                {
                    this.highScores = new ObservableCollection<UserModel>();
                }
                this.highScores.Clear();
                foreach (var item in value)
                {
                    this.highScores.Add(item);
                }
            }
        }

        public HighScoresViewModel()
        {
            this.HighScores = new ObservableCollection<UserModel>();

            this.LoadHighScores("http://tictactoeextended.apphb.com/api/games");
        }

        private async void LoadHighScores(string url)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            MessageDialog msgDlg = new MessageDialog("");

            try
            {
                var response = await client.GetAsync("");

                var responseText = await response.Content.ReadAsStringAsync();

                var users = await JsonConvert.DeserializeObjectAsync<IEnumerable<UserModel>>(responseText);

                foreach (var user in users)
                {
                    this.HighScores.Add(user);
                }
            }
            catch (Exception)
            {
                msgDlg.Content = "No connection to database. Please connect to the internet and try again.";
            }

            if (msgDlg.Content != "")
            {
                msgDlg.ShowAsync();
            }
        }
    }
}
