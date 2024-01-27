using System;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private const int FieldHeight = 3;
        private const int FieldWidth = 3;

        public enum Cell
        {
            X,
            O,
            empty
        };
        public enum PlayerSide
        {
            X,
            O
        }
        // -10  : выиграл игрок
        // 10   : выиграл компьютер
        // 1    : игра продолжается
        // 0    : ничья
        public enum GameState
        {
            Lose = -10,
            Draw = 0,
            IsGoing = 1,
            Win = 10
        }

        private Cell[,] field = new Cell[FieldHeight, FieldWidth];
        private PlayerSide playerSide = PlayerSide.X;
        private bool isGame = true;

        public MainWindow()
        {
            InitializeComponent();
            ClearField();

            for (int i = 0; i < FieldHeight; i++)
                for (int j = 0; j < FieldWidth; j++)
                {
                    var button = (Button)FindName("Button_" + i + "_" + j);
                    button.Click += FieldButton_OnClicked;
                }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            isGame = true;
            ClearField();
            ChangePlayerSide();
            LabelGameResult.Content = "Крестики-нолики";
        }

        private void ClearField()
        {
            for (int i = 0; i < FieldHeight; i++)
                for (int j = 0; j < FieldWidth; j++)
                {
                    field[i, j] = Cell.empty;
                    var button = (Button)FindName("Button_" + i + "_" + j);
                    button.Content = string.Empty;
                }
        }

        private void ChangePlayerSide()
        {
            playerSide = (PlayerSide)(((int)playerSide + 1) % 2);

            if (playerSide == PlayerSide.O)
            {
                var (_i, _j) = StepComputer();
                var _button = (Button)FindName("Button_" + _i + "_" + _j);
                SetValueToCell(_i, _j, _button, false);
            }
        }

        private void FieldButton_OnClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (isGame && button.Content.ToString() == string.Empty)
            {
                var i = int.Parse(button.Name.Split('_')[1]);
                var j = int.Parse(button.Name.Split('_')[2]);
                SetValueToCell(i, j, button, true);

                var (_i, _j) = StepComputer();
                var _button = (Button)FindName("Button_" + _i + "_" + _j);
                SetValueToCell(_i, _j, _button, false);

                var gameState = GetGameState();
                if (gameState != GameState.IsGoing)
                {
                    isGame = false;
                    switch (gameState)
                    {
                        case GameState.Lose:
                            LabelGameResult.Content = "Вы победили!!!";
                            break;
                        case GameState.Draw:
                            LabelGameResult.Content = "Ничья";
                            break;
                        case GameState.Win:
                            LabelGameResult.Content = "Вы проиграли!";
                            break;
                        default:
                            throw new Exception("Incorrect Game State!");
                    }
                }
            }
        }

        private void SetValueToCell(int i, int j, Button button, bool isPlayerStep)
        {
            if (i >= 0 && i <= 3 && j >= 0 && j <= 3)
            {
                var activeSide = isPlayerStep ? playerSide : (PlayerSide)(((int)playerSide + 1) % 2);
                field[i, j] = (Cell)(int)activeSide;
                button.Content = activeSide.ToString();
            }
        }
        private GameState GetGameState()
        {
            // проверяем выиграл ли крестик
            if (field[0, 0] == field[0, 1] && field[0, 1] == field[0, 2] && field[0, 0] == Cell.X
                || field[1, 0] == field[1, 1] && field[1, 1] == field[1, 2] && field[1, 0] == Cell.X
                || field[2, 0] == field[2, 1] && field[2, 1] == field[2, 2] && field[2, 0] == Cell.X
                || field[0, 0] == field[1, 0] && field[1, 0] == field[2, 0] && field[0, 0] == Cell.X
                || field[0, 1] == field[1, 1] && field[1, 1] == field[2, 1] && field[0, 1] == Cell.X
                || field[0, 2] == field[1, 2] && field[1, 2] == field[2, 2] && field[0, 2] == Cell.X
                || field[0, 0] == field[1, 1] && field[1, 1] == field[2, 2] && field[0, 0] == Cell.X
                || field[0, 2] == field[1, 1] && field[1, 1] == field[2, 0] && field[0, 2] == Cell.X)
                return playerSide == PlayerSide.X ? GameState.Lose : GameState.Win;
            // проверяем выиграл ли нолик
            else if (field[0, 0] == field[0, 1] && field[0, 1] == field[0, 2] && field[0, 0] == Cell.O
                || field[1, 0] == field[1, 1] && field[1, 1] == field[1, 2] && field[1, 0] == Cell.O
                || field[2, 0] == field[2, 1] && field[2, 1] == field[2, 2] && field[2, 0] == Cell.O
                || field[0, 0] == field[1, 0] && field[1, 0] == field[2, 0] && field[0, 0] == Cell.O
                || field[0, 1] == field[1, 1] && field[1, 1] == field[2, 1] && field[0, 1] == Cell.O
                || field[0, 2] == field[1, 2] && field[1, 2] == field[2, 2] && field[0, 2] == Cell.O
                || field[0, 0] == field[1, 1] && field[1, 1] == field[2, 2] && field[0, 0] == Cell.O
                || field[0, 2] == field[1, 1] && field[1, 1] == field[2, 0] && field[0, 2] == Cell.O)
                return playerSide == PlayerSide.X ? GameState.Win : GameState.Lose;
            // проверяем ничья ли
            bool draw = true;
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    if (field[i, j] == Cell.empty)
                        draw = false;
            return draw ? GameState.Draw : GameState.IsGoing;
        }

        private (int, int) StepComputer()
        {
            // заводим переменную максимума, ищущую лучший ход, для компьютера
            int max_step = -100;
            // заводим переменные индекса этого максимума
            int max_i = -1, max_j = -1;
            // пробегаемся по всем клеткам поля
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    // если очередная клетка пустая
                    if (field[i, j] == Cell.empty)
                    {
                        // то делаем в неё потенциальный ход компьютером
                        field[i, j] = playerSide == PlayerSide.X ? Cell.O : Cell.X;
                        // смотрим, насколько это выгодно для компьютера
                        int mmax = minimax(playerSide == PlayerSide.X ? Cell.X : Cell.O);
                        // если ход в данную клетку лучше, чем текущий максимум
                        if (mmax > max_step)
                        {
                            // то запоминаем эту клетку
                            max_step = mmax;
                            max_i = i;
                            max_j = j;
                        }
                        // очистили за собой клетку, куда сделали потенциальный ход
                        field[i, j] = Cell.empty;
                    }
            return (max_i, max_j);
        }

        private int minimax(Cell _step)
        {
            int res = (int)GetGameState();
            if (res != 1) return res;
            if (_step == Cell.O)
            {
                int max_step = -100;
                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 3; ++j)
                        if (field[i, j] == Cell.empty)
                        {
                            field[i, j] = Cell.O;
                            int mmax = minimax(Cell.X);
                            if (mmax > max_step)
                                max_step = mmax;
                            field[i, j] = Cell.empty;
                        }
                return max_step;
            }
            if (_step == Cell.X)
            {
                int min_step = 100;
                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 3; ++j)
                        if (field[i, j] == Cell.empty)
                        {
                            field[i, j] = Cell.X;
                            int mmax = minimax(Cell.O);
                            if (mmax < min_step)
                                min_step = mmax;
                            field[i, j] = Cell.empty;
                        }
                return min_step;
            }
            return 0; // ни на что не влияющий return, пишем его для того, чтобы компилятор точно знал, 
            // что мы вернем что-то из этой функции
        }
    }
}