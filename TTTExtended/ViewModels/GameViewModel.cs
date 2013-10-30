using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TTTExtended.Behavior;
using TTTExtended.Common;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls;

namespace TTTExtended.ViewModels
{
    public class GameViewModel : BindableBase
    {
        private readonly StorageFolder roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
        private readonly string[] winnables =
            new string[] { "012", "345", "678", "036", "147", "258", "048", "246" };

        ObservableCollection<SingleBoardViewModel> boards = new ObservableCollection<SingleBoardViewModel>();
        private ObservableCollection<PlayerViewModel> players;

        private ObservableCollection<StorageFile> savedGames;
        private IReadOnlyList<StorageFile> savedGamesList;

        private PlayerViewModel currentPlayer;
        private PlayerViewModel playerOne;
        private PlayerViewModel playerTwo;

        private ICommand makeMoveCommand;
        private ICommand startNewGame;
        private ICommand saveGame;
        private ICommand changePlayersNames;


        private string savedBoards;
        private bool isWon;
        private bool isGameOver;
        private string winner;
        private string gameOverMessage;
        public string looser;

        public string Looser
        {
            get
            {
                return this.looser;
            }
        }

        public string Winner
        {
            get
            {
                return this.winner;
            }
        }

        public GameViewModel()
        {
            InitPlayers();
            InitNewGame();
        }

        public bool IsGameOver
        {
            get
            {
                return this.isGameOver;
            }
            set
            {
                if (this.isGameOver != value)
                {
                    this.isGameOver = value;
                    this.OnPropertyChanged("IsGameOver");
                }
            }
        }

        public bool IsWon
        {
            get
            {
                return this.isWon;
            }
            set
            {
                if (this.isWon != value)
                {
                    this.isWon = value;
                    this.OnPropertyChanged("IsWon");
                }
            }
        }

        public IEnumerable<PlayerViewModel> Players
        {
            get
            {
                if (this.players == null)
                {
                    this.players = new ObservableCollection<PlayerViewModel>();
                }
                return this.players;
            }
            set
            {
                if (this.players == null)
                {
                    this.players = new ObservableCollection<PlayerViewModel>();
                }
                this.SetObservableValues(this.players, value);
            }
        }

        public IEnumerable<SingleBoardViewModel> Boards
        {
            get
            {
                if (this.boards == null)
                {
                    this.boards = new ObservableCollection<SingleBoardViewModel>();
                }
                return this.boards;
            }
            set
            {
                if (this.boards == null)
                {
                    this.boards = new ObservableCollection<SingleBoardViewModel>();
                }
                this.SetObservableValues(this.boards, value);
            }
        }

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

        public PlayerViewModel PlayerOne
        {
            get
            {
                return this.playerOne;
            }
            set
            {
                if (this.playerOne != value)
                {
                    this.playerOne = value;
                    this.OnPropertyChanged("PlayerOne");
                }
            }
        }

        public PlayerViewModel PlayerTwo
        {
            get
            {
                return this.playerTwo;
            }
            set
            {
                if (this.playerTwo != value)
                {
                    this.playerTwo = value;
                    this.OnPropertyChanged("PlayerTwo");
                }
            }
        }

        public PlayerViewModel CurrentPlayer
        {
            get
            {
                return this.currentPlayer;
            }
            set
            {
                if (this.currentPlayer != value)
                {
                    this.currentPlayer = value;
                    this.OnPropertyChanged("CurrentPlayer");
                }
            }
        }

        public ICommand MakeMove
        {
            get
            {
                if (this.makeMoveCommand == null)
                {
                    this.makeMoveCommand = new RelayCommand<Point>(this.HandleMakeMoveCommand);
                }

                return this.makeMoveCommand;
            }
        }

        public ICommand StartNewGame
        {
            get
            {
                if (this.startNewGame == null)
                {
                    this.startNewGame = new RelayCommand<object>(this.HandleStartNewGameCommand);
                }

                return this.startNewGame;
            }
        }

        public ICommand SaveGame
        {
            get
            {
                if (this.saveGame == null)
                {
                    this.saveGame = new RelayCommand<object>(this.HandleSaveGameCommand);
                }

                return this.saveGame;
            }
        }

        public ICommand ChangePlayersNames
        {
            get
            {
                if (this.changePlayersNames == null)
                {
                    this.changePlayersNames = new RelayCommand<object>(this.HandleChangePlayersNamesCommand);
                }

                return this.changePlayersNames;
            }
        }

        private void InitPlayers()
        {
            this.PlayerOne = new PlayerViewModel("Player 1", "X", Colors.MediumVioletRed);
            this.PlayerTwo = new PlayerViewModel("Player 2", "O", Colors.Green);

            this.players = new ObservableCollection<PlayerViewModel>()
            {
                this.PlayerOne,
                this.PlayerTwo
            };
        }

        private void InitNewGame()
        {
            this.IsWon = false;
            this.IsGameOver = false;
            boards.Clear();

            boards.Add(new SingleBoardViewModel(true));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));
            boards.Add(new SingleBoardViewModel(false));


            this.CurrentPlayer = this.Players.First();
        }


        private async void HandleStartNewGameCommand(object obj)
        {
            MessageDialog startNewGameDialog = new MessageDialog("Are you sure you want to start new game.");

            startNewGameDialog.Commands.Add(new UICommand("yes"));
            startNewGameDialog.Commands.Add(new UICommand("no"));

            var label = await startNewGameDialog.ShowAsync();

            if (label.Label == "yes")
            {
                InitNewGame();
            }

        }

        private async void HandleSaveGameCommand(object obj)
        {
            savedBoards = JsonConvert.SerializeObject(boards);

            var saveDialog = new InputDialog();

            saveDialog.BackgroundStripeBrush = new SolidColorBrush(Colors.Black);
            saveDialog.Background = new SolidColorBrush(Colors.Black);
            saveDialog.ButtonStyle = new Style(typeof(Button));

            var label = await saveDialog.ShowAsync("Save Game", "Game Name", "save", "cancel");
            var text = saveDialog.InputText;

            if (label == "save")
            {
                var gameToSaveFile = await roamingFolder.CreateFileAsync(text + ".json",
                    CreationCollisionOption.GenerateUniqueName);

                await Windows.Storage.FileIO.WriteTextAsync(gameToSaveFile, savedBoards);
            }

        }

        private async void HandleChangePlayersNamesCommand(object obj)
        {
            var dlg = new InputDialog();

            dlg.BackgroundStripeBrush = new SolidColorBrush(Colors.Black);
            dlg.Background = new SolidColorBrush(Colors.Black);
            dlg.ButtonStyle = new Style(typeof(Button));

            dlg.InputText = "First Player";
            await dlg.ShowAsync("Input Name", "First Player Name", "ok", "cancel");
            this.PlayerOne.Name = dlg.InputText;
            dlg.InputText = "Second Player";
            await dlg.ShowAsync("Input Name", "Second Player Name", "ok", "cancel");
            this.PlayerTwo.Name = dlg.InputText;
        }

        private void HandleMakeMoveCommand(Point point)
        {
            SignViewModel elem = new SignViewModel();

            var x = point.X % 350;
            var y = point.Y % 260;
            int boardCol = (int)(point.X / 350);
            int boardRow = (int)(point.Y / 260);

            var elementPosition = ChooseElementPosition(x, y);
            var index = 3 * boardRow + boardCol;

            if (elementPosition <= 8)
            {
                elem = this.boards[index].Elements.ElementAt(elementPosition);

                if (string.IsNullOrEmpty(elem.Sign) && this.boards[index].CanPlayIn)
                {
                    elem.Sign = this.CurrentPlayer.Sign;
                    elem.Color = this.CurrentPlayer.Color;
                    this.boards[index].CanPlayIn = false;

                    SetNextPlayer();

                    if (CheckForWinnerOnBoard(this.boards[index]))
                    {
                        this.boards[index].IsFinished = true;
                        this.boards[index].Winner = elem.Sign;
                        CheckForWinnerInGame();
                    }
                    else if (CheckForTie(this.boards[index]))
                    {
                        this.boards[index].IsFinished = true;
                        this.boards[index].Winner = "tie";
                        CheckForTieInGame();
                    }

                    SetCanPlayPermissions(elementPosition);
                    if (this.IsGameOver)
                    {
                        ForbidPlayingInBoards();
                        PostRecordInScores();
                    }

                }
            }
        }

        private async void PostRecordInScores()
        {
            MessageDialog msgDlg = new MessageDialog(gameOverMessage);
            if (playerOne.Name == "Player 1" || playerOne.Name == "Player 2")
            {
                await msgDlg.ShowAsync();
                return;
            }
            var url = "http://tictactoeextended.apphb.com/api/games";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            string status;
            string player;
            string opponent;
            if (isGameOver && !IsWon)
            {
                status = "tie";
                player = PlayerOne.Name;
                opponent = PlayerTwo.Name;
            }
            else
            {
                status = "win";
                player = winner;
                opponent = this.Players.FirstOrDefault(x => x.Name != winner).Name;
                if (opponent == null)
                {
                    opponent = winner;
                }

            }

            var model = new GameModel()
            {
                PlayerName = player,
                Opponent = opponent,
                Status = status
            };

            string record = await JsonConvert.SerializeObjectAsync(model);
            HttpContent content = new StringContent(record);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            string message = "";
            try
            {
                var response = await client.PostAsync("", content);
                message = "Your result was send to high scores.";
            }
            catch (Exception)
            {
                message = "Can't conncect with database. Your result can't be added to highscores.";
            }

            msgDlg.Content = (gameOverMessage + message);
            await msgDlg.ShowAsync();
        }

        private void SetNextPlayer()
        {
            if (this.CurrentPlayer.Sign == "X")
            {
                this.CurrentPlayer = this.Players.Last();
            }
            else
            {
                this.CurrentPlayer = this.Players.First();
            }
        }

        private void SetCanPlayPermissions(int elementPosition)
        {
            if (this.boards[elementPosition].IsFinished)
            {
                foreach (var board in boards)
                {
                    if (!board.IsFinished)
                    {
                        board.CanPlayIn = true;
                    }
                }
            }
            else
            {
                foreach (var board in boards)
                {
                    if (!board.IsFinished)
                    {
                        board.CanPlayIn = false;
                    }
                }
                this.boards[elementPosition].CanPlayIn = true;
            }
        }

        private bool CheckForWinnerOnBoard(SingleBoardViewModel board)
        {
            var result = false;
            foreach (var position in winnables)
            {
                var pos1 = int.Parse(position[0].ToString());
                var pos2 = int.Parse(position[1].ToString());
                var pos3 = int.Parse(position[2].ToString());

                if (board.Elements.ElementAt(pos1).Sign == board.Elements.ElementAt(pos2).Sign &&
                    board.Elements.ElementAt(pos2).Sign == board.Elements.ElementAt(pos3).Sign &&
                    board.Elements.ElementAt(pos3).Sign != string.Empty)
                {
                    result = true;
                }
            }

            return result;
        }

        private bool CheckForTie(SingleBoardViewModel board)
        {
            bool result = true;
            foreach (var elem in board.Elements)
            {
                if (string.IsNullOrEmpty(elem.Sign))
                {
                    result = false;
                }
            }

            return result;
        }

        private void CheckForWinnerInGame()
        {
            foreach (var position in winnables)
            {
                var pos1 = int.Parse(position[0].ToString());
                var pos2 = int.Parse(position[1].ToString());
                var pos3 = int.Parse(position[2].ToString());

                if (boards[pos1].Winner == boards[pos2].Winner &&
                    boards[pos2].Winner == boards[pos3].Winner && boards[pos1].Winner != null)
                {
                    this.IsWon = true;
                    this.IsGameOver = true;
                    //ForbidPlayingInBoards();
                    this.winner = this.players.FirstOrDefault(p => p.Sign == boards[pos1].Winner).Name;
                    this.looser = this.players.FirstOrDefault(p => p.Sign != boards[pos1].Winner).Name;
                    gameOverMessage = winner + " wins. ";
                    return;
                }
            }
            if (CheckForTieInGame())
            {
                ForbidPlayingInBoards();
                this.IsGameOver = true;
                gameOverMessage = "The Game is tie.";
                return;
            }
        }

        private void ForbidPlayingInBoards()
        {
            foreach (var board in this.Boards)
            {
                if (!board.IsFinished)
                {
                    board.CanPlayIn = false;
                }
            }
        }

        private bool CheckForTieInGame()
        {
            bool result = true;
            foreach (var board in boards)
            {
                if (!board.IsFinished)
                {
                    result = false;
                }
            }

            return result;
        }

        private int ChooseElementPosition(double x, double y)
        {
            int row = 10;
            int column = 10;

            if (x >= 50 && x <= 150)
            {
                row = 0;
            }
            if (x > 150 && x <= 250)
            {
                row = 1;
            }
            if (x > 250 && x <= 350)
            {
                row = 2;
            }


            if (y >= 50 && y <= 120)
            {
                column = 0;
            }
            if (y > 120 && y <= 190)
            {
                column = 1;
            }
            if (y > 190 && y <= 260)
            {
                column = 2;
            }

            int elementPosition = 3 * column + row;
            return elementPosition;
        }

    }
}
