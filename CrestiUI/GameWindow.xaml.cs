using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using CRESTI;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private readonly Client _user;
        private readonly List<Button> cells;
        private readonly Board Game;


        public GameWindow(Client user)
        {
            InitializeComponent();
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
            Cell1.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 0); };
            Cell2.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 1); };
            Cell3.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 0, 2); };
            Cell4.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 0); };
            Cell5.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 1); };
            Cell6.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 1, 2); };
            Cell7.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 0); };
            Cell8.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 1); };
            Cell9.Click += (sender, EventArgs) => { cell_click(sender, EventArgs, 2, 2); };

            _user = user;
            _user.Notify += getMessage;
            Game = new Board();
        }


        private void getMessage(object sender, ClientServerMessageEventArgs e)
        {
            //Trace.WriteLine($"Пришло сообщение {e.Message}");
            RequestHandler.ExecuteRequest(Game, new Request(e.Message));
            updateBoard();
        }


        private void setCellsDisabled()
        {
            for (var i = 0; i < cells.Count; i++)
            {
                cells[i].IsEnabled = false;
            }
        }


        private void updateBoard()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    //Trace.WriteLine($"Меняю кнопку {i} {j}");
                    Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new Action(() => cells[(i * 3) + j].Content = Game.Cells[i, j].Value)
                    );
                    //Dispatcher.BeginInvoke(new ThreadStart(delegate { cells[(i * 3) + j].Content = Game.Cells[i, j].Value; }));
                }
            }
        }


        private void setCellsEnabled()
        {
            for (var i = 0; i < cells.Count; i++)
            {
                cells[i].IsEnabled = true;
            }
        }


        private void cell_click(object sender, EventArgs e, int row, int col)
        {
            _user.SendMessage($"Mark?row={row}&col={col}");
            Game.Mark(row, col);

            (sender as Button).Content = Game.Cells[row, col].Value;
            if (Game.State == GameState.Finished)
            {
                if (Game.Winner != null)
                {
                    MessageBox.Show($"Победил {Game.Winner}");
                }
                else
                {
                    MessageBox.Show("Ничья");
                }
            }
        }


        private void GameWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _user.Disconnect();
        }
    }
}