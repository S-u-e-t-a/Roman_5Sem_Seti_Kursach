using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using CRESTI;

using CrestiUI.Game;
using CrestiUI.net;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly Board _game;

        private bool _isUserTurn;
        private readonly bool isPlayer;
        private readonly List<Button> cells;
        private readonly LocalLobby _lobby;


        private bool isUserTurn
        {
            get { return _isUserTurn; }
            set
            {
                _isUserTurn = value;
                if (value)
                {
                    setCellsEnabled();
                }
                else
                {
                    setCellsDisabled();
                }
            }
        }


        public GameWindow(LocalLobby lobby)
        {
            _lobby = lobby;
            InitializeComponent();

            _game = new Board();

            cells = new List<Button>();
            cells.Add(Cell1);
            cells.Add(Cell2);
            cells.Add(Cell3);
            cells.Add(Cell4);
            cells.Add(Cell5);
            cells.Add(Cell6);
            cells.Add(Cell7);
            cells.Add(Cell8);
            cells.Add(Cell9);

            isPlayer = _lobby.isPlayerUser();
            isUserTurn = _lobby.isXPlayer();

            if (isPlayer)
            {
                #region CellsInitialization

                Cell1.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 0); };
                Cell2.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 1); };
                Cell3.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 2); };
                Cell4.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 0); };
                Cell5.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 1); };
                Cell6.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 2); };
                Cell7.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 0); };
                Cell8.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 1); };
                Cell9.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 2); };

                #endregion
            }

            if (_lobby.isXPlayer())
            {
                isUserTurn = true;
            }

            if (!_lobby.IsServer)
            {
                RestartButton.Visibility = Visibility.Hidden;
            }

            _lobby.CellMarked += (sender, args, row, col) =>
            {
                markCell(row, col);
                checkWin();
            };
            _lobby.WritedToChat += (sender, args, username, message) => Dispatcher.Invoke(() => ChatBlock.Text = _lobby.ChatHistory);
            _lobby.GameRestartedHandler += (sender, args) => restartGame();
            Dispatcher.Invoke(() => ChatBlock.Text = _lobby.ChatHistory);
        }


        private void restartGame()
        {
            _game.Restart();
            if (_lobby.isXPlayer())
            {
                isUserTurn = true;
            }
            else
            {
                isUserTurn = false;
            }

            updateBoard();
        }


        private void checkWin()
        {
            if (_game.State == GameState.Finished)
            {
                var mes = string.Empty;
                var winnerType = _game.Winner;
                if (winnerType != null)
                {
                    var winnerName = string.Empty;
                    if (winnerType == PlayerType.O)
                    {
                        winnerName = _lobby.Oplayer.Name;
                    }
                    else if (winnerType == PlayerType.O)
                    {
                        winnerName = _lobby.Xplayer.Name;
                    }

                    mes = $"Победил {winnerName}";
                }
                else
                {
                    mes = "ничья!";
                }

                setCellsDisabled();
                MessageBox.Show(mes);
            }
        }


        private void markCell(int row, int col)
        {
            _game.Mark(row, col);
            updateBoard();
            changeTurn();
        }


        private void changeTurn()
        {
            if (isPlayer)
            {
                isUserTurn = !isUserTurn;
            }
        }


        private void setCellsDisabled()
        {
            for (var i = 0; i < cells.Count; i++)
            {
                Dispatcher.Invoke
                (
                    () => cells[i].IsEnabled = false
                );
            }
        }


        private void updateBoard()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    Dispatcher.Invoke(
                        () => cells[(i * 3) + j].Content = _game.Cells[i, j].Value
                    );
                }
            }
        }


        private void setCellsEnabled()
        {
            for (var i = 0; i < cells.Count; i++)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action
                    (
                        () => cells[i].IsEnabled = true)
                );
            }
        }


        private void cell_click(object sender, EventArgs e, int row, int col)
        {
            var request = new Request("POST", RequestCommands.POSTUserMark, new Dictionary<string, string>
            {
                {"row", row.ToString()},
                {"col", col.ToString()}
            });
            _lobby.SendMessageToServer(request.ToJsonString());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _lobby.WriteToChat(MessageTextBox.Text);
            MessageTextBox.Text = string.Empty;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var request = new Request("POST", RequestCommands.POSTRestartGame, null);
            _lobby.SendMessageToServer(request.ToJsonString());
        }
    }
}