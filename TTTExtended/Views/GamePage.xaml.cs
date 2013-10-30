using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TTTExtended.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace TTTExtended.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : TTTExtended.Common.LayoutAwarePage
    {

        Dictionary<int, UIElement> pages;
        Size? pageSize;
        Rect? imageableRect;
        PrintDocument document;

        public GamePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode =
                Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.OnRegister();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigationParameter = e.Parameter;
            if (navigationParameter is StorageFile)
            {
                var currentVM = this.DataContext as GameViewModel;

                var file = navigationParameter as StorageFile;
                var text = await Windows.Storage.FileIO.ReadTextAsync(file);
                var boards = await JsonConvert.DeserializeObjectAsync<ObservableCollection<SingleBoardViewModel>>(text);
                currentVM.Boards = boards;
                this.DataContext = currentVM;
            }
        }

        protected async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var currentVM = this.DataContext as GameViewModel;
            var request = args.Request;
            request.Data.Properties.Title = "Playing TTT Ultimate";

            request.Data.Properties.Description = "Playing a little bit of Tic Tac Toe Ultimate";

            if (currentVM.IsWon)
            {
                request.Data.SetText(string.Format("{0} just beat {1}.", currentVM.Winner, currentVM.looser));
            }
            else if (currentVM.IsGameOver)
            {
                request.Data.SetText(string.Format("The game between {0} and {1} is tie.", currentVM.PlayerOne.Name, currentVM.PlayerTwo.Name));
            }
            else
            {
                request.Data.SetText(string.Format("{0} and {1} are playin and the game is tough.", currentVM.PlayerOne.Name, currentVM.PlayerTwo.Name));
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

            if (pageState != null)
            {
                if (pageState.ContainsKey("currentInput"))
                {
                    var boardsSerialized = (string)pageState["currentInput"];

                    var boardsDeserialized = await JsonConvert.DeserializeObjectAsync<ObservableCollection<SingleBoardViewModel>>(boardsSerialized);

                    var curentVM = this.DataContext as GameViewModel;
                    curentVM.Boards = boardsDeserialized;
                }
            }

            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

            var curentVM = this.DataContext as GameViewModel;
            var boards = curentVM.Boards;
            var boardsSerialized = JsonConvert.SerializeObjectAsync(boards).Result;

            pageState["currentInput"] = boardsSerialized;

            //var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

            //var gameToSaveFile = await roamingFolder.CreateFileAsync(SavedStateFileName,
            //        CreationCollisionOption.ReplaceExisting);

            //await Windows.Storage.FileIO.WriteTextAsync(gameToSaveFile, boardsSerialized);

            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }
        
        void OnRegister()
        { 
            PrintManager manager = PrintManager.GetForCurrentView();

            manager.PrintTaskRequested += OnPrintTaskRequested;
        }
        void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            PrintTask printTask = args.Request.CreatePrintTask(
              "My Print Job",
              OnPrintTaskSourceRequestedHandler);

            printTask.Completed += OnPrintTaskCompleted;

            deferral.Complete();
        }

        void OnPrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            this.pageSize = null;
            this.imageableRect = null;
            this.document = null;
            this.pages = null;
        } 

        async void OnPrintTaskSourceRequestedHandler(PrintTaskSourceRequestedArgs args)
        {
            var deferral = args.GetDeferral();

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              {
                  this.document = new PrintDocument();

                  this.document.Paginate += OnPaginate;
                  this.document.GetPreviewPage += OnGetPreviewPage;
                  this.document.AddPages += OnAddPages;

                  args.SetSource(this.document.DocumentSource);
              }
            );
            deferral.Complete();
        }

        void OnAddPages(object sender, AddPagesEventArgs e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              {
                  this.document.AddPage(this.pages[1]);
                  this.document.AddPagesComplete();
              }
            );
        }

        void OnGetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              {
                  if (this.pages == null)
                  {
                      this.pages = new Dictionary<int, UIElement>();
                  }
                  if (!this.pages.ContainsKey(e.PageNumber))
                  {
                      this.pages[e.PageNumber] = GameField;
                  }

                  this.document.SetPreviewPage(e.PageNumber,
                    this.pages[e.PageNumber]);
              }
            );
        }

        void OnPaginate(object sender, PaginateEventArgs e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
              () =>
              { 
                  this.GetPageSize(e);
                  this.document.SetPreviewPageCount(1, PreviewPageCountType.Final);
              }
            );
        }

        void GetPageSize(PaginateEventArgs e)
        {
            if (this.pageSize == null)
            {
                PrintPageDescription description = e.PrintTaskOptions.GetPageDescription(
                  (uint)e.CurrentPreviewPageNumber);

                this.pageSize = description.PageSize;
                this.imageableRect = description.ImageableRect;
            }
        }  
    }
}
