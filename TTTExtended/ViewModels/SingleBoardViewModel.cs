using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTExtended.Common;
using TTTExtended.Helpers;
using Windows.UI;

namespace TTTExtended.ViewModels
{
    public class SingleBoardViewModel : BindableBase
    {
        private TrulyObservableCollection<SignViewModel> elements;
        private bool canPlayIn;
        private bool isFinished;
        private string winner;

        public string Winner
        {
            get
            {
                return this.winner;
            }
            set
            {
                if (this.winner != value)
                {
                    this.winner = value;
                    this.OnPropertyChanged("Winner");
                }
            }
        }

        public bool CanPlayIn
        {
            get
            {
                return this.canPlayIn;
            }
            set
            {
                if (this.canPlayIn != value)
                {
                    this.canPlayIn = value;
                    this.OnPropertyChanged("CanPlayIn");
                }
            }
        }

        public bool IsFinished
        {
            get
            {
                return this.isFinished;
            }
            set
            {
                if (this.isFinished != value)
                {
                    this.isFinished = value;
                    this.OnPropertyChanged("IsFinished");
                }
            }
        }

        public SingleBoardViewModel(bool canPlayIn)
        {
            this.elements = new TrulyObservableCollection<SignViewModel>()
            {
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
                new SignViewModel(string.Empty, new Color()),
            };

            this.CanPlayIn = canPlayIn;
            this.IsFinished = false;
        }

        public IEnumerable<SignViewModel> Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new TrulyObservableCollection<SignViewModel>();
                }
                return this.elements;
            }
            set
            {
                if (this.elements == null)
                {
                    this.elements = new TrulyObservableCollection<SignViewModel>();
                }
                this.SetObservableValues(this.elements, value);
            }
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
