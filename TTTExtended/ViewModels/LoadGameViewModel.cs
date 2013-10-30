using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TTTExtended.ViewModels
{
    public class LoadGameViewModel
    {
        private readonly StorageFolder roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

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

        public LoadGameViewModel()
        {
            LoadGames();
        }

        private async void LoadGames()
        {
            this.savedGamesList = await roamingFolder.GetFilesAsync();
            this.SavedGames = savedGamesList;

            //var files = await roamingFolder.GetFilesAsync();

            //this.savedGamesList = files.Where(x => x.FileType.Equals(".json"));

            //this.SavedGames = savedGamesList;
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
