using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTExtended.Common;
using Windows.Storage;

namespace TTTExtended.ViewModels
{
    public class SearchResultsViewModel : BindableBase
    {
        private readonly StorageFolder roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

        private string queryText;
        public string QueryText
        {
            get
            {
                return this.queryText;
            }
            set
            {
                this.queryText = value;
                this.OnPropertyChanged("QueryText");
                this.LoadResults();
            }
        }

        //private ObservableCollection<StorageFile> results;

        //public IEnumerable<StorageFile> Results
        //{
        //    get
        //    {
        //        if (this.results == null)
        //        {
        //            results = new ObservableCollection<StorageFile>();
        //        }

        //        return results;
        //    }
        //    set
        //    {
        //        this.results.Clear();

        //        foreach (var item in value)
        //        {
        //            this.results.Add(item);
        //        }
        //    }
        //}

        private ObservableCollection<StorageFile> savedGames;
        private IEnumerable<StorageFile> savedGamesList;

        public IEnumerable<StorageFile> SavedGames
        {
            get
            {
                if (this.savedGames == null)
                {
                    this.savedGames = new ObservableCollection<StorageFile>();
                    if (this.savedGamesList != null)
                    {
                        foreach (var game in savedGamesList)
                        {
                            this.savedGames.Add(game);

                        }
                    }
                }

                return this.savedGames;
            }
            set
            {
                if (this.savedGames == null)
                {
                    this.savedGames = new ObservableCollection<StorageFile>();
                }

                this.SetObservableValues(this.savedGames, value);
            }
        }

        public async void LoadResults()
        {
            this.savedGamesList = await roamingFolder.GetFilesAsync();

            this.SavedGames = this.savedGamesList.Where(x => x.DisplayName.Contains(queryText));
        }

        private void SetObservableValues<T>(ObservableCollection<T> observableCollection, IEnumerable<T> values)
        {
            if (observableCollection != values)
            {
                observableCollection.Clear();
                foreach (var item in values)
                {
                    observableCollection.Add(item);
                }
            }
        }
    }
}
