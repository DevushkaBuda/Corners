using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication340.Model
{
    [Serializable]
    internal class Game
    {
        public const int SIZE = 8;
        public const int HOMESIZE = 3;

        public Cell[,] Board { get; private set; }
        public Cell CurrentPlayer { get; private set; }
        public Cell Winner { get; private set; }
        public event EventHandler Moved = delegate { };

        public Game()
        {
            InitNewGame();
        }

        private void InitNewGame()
        {
            CurrentPlayer = Cell.White;
            Board = new Cell[SIZE,SIZE];

            for (int i = 0; i < HOMESIZE; i++)
            for (int j = 0; j < HOMESIZE; j++)
            {
                Board[i, j] = Cell.White;
                Board[SIZE - i - 1, SIZE - j - 1] = Cell.Black;
            }
        }

        public Cell this[int x, int y]
        {
            get { return Board[x, y]; }
        }

        public Cell this[Point p]
        {
            get { return Board[p.X, p.Y]; }
        }

        /// <summary>
        /// Может ли ходить игрок данной шашкой в данное поле?
        /// </summary>
        public bool CanMove(Cell player, Point from, Point to)
        {
            if (Winner != Cell.Empty) return false;
            if (!InBoard(from)) return false;
            if (this[from] != player) return false;
            return GetAllowedMoves(from).Contains(to);
        }

        /// <summary>
        /// Множество допустимых ходов для шашки
        /// </summary>
        public HashSet<Point> GetAllowedMoves(Point cell)
        {
            var stack = new Stack<Point>();
            var res = new HashSet<Point>();
            stack.Push(cell);
            var allowOnlyJump = false;

            while(stack.Count > 0)
            {
                var c = stack.Pop();

                for(int dx = -1;dx<=1;dx++)
                for(int dy = -1;dy<=1;dy++)
                if(dx != 0 || dy != 0)
                {
                    var p = new Point(c.X + dx, c.Y + dy);
                    if(InBoard(p))
                    {
                        //ход на соседнюю пустую клетку
                        if (this[p] == Cell.Empty)
                        {
                            if(!allowOnlyJump)
                                res.Add(p);
                        }else
                        {
                            //прыжок через шашку
                            p = new Point(c.X + 2 * dx, c.Y + 2 * dy);
                            if(InBoard(p))
                            {
                                if (this[p] == Cell.Empty)
                                {
                                    if (res.Add(p))
                                        stack.Push(p);
                                }
                            }
                        }
                    }
                }

                allowOnlyJump = true;
            }

            return res;
        }

        /// <summary>
        /// Сделать ход 
        /// </summary>
        public void MakeMove(Point from, Point to)
        {
            var c = this[from];
            Board[from.X, from.Y] = Cell.Empty;
            Board[to.X, to.Y] = c;
            //проверяем, нет ли выигрыша?
            if (CurrentPlayer == Cell.Black)//выигрыш может наступить только после хода черных
            {
                if (CheckFillHome(Cell.Black))
                    Winner = Cell.Black;
                if (CheckFillHome(Cell.White))
                    Winner = Cell.White;
            }
            //
            CurrentPlayer = InversePlayer(CurrentPlayer);
            //
            OnMove();
        }

        protected void OnMove()
        {
            Moved(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ячейка внутри доски?
        /// </summary>
        public bool InBoard(Point cell)
        {
            if (cell.X < 0 || cell.X >= SIZE) return false;
            if (cell.Y < 0 || cell.Y >= SIZE) return false;

            return true;
        }

        Cell InversePlayer(Cell cell)
        {
            if (cell == Cell.Black) return Cell.White;
            if (cell == Cell.White) return Cell.Black;
            return cell;
        }

        /// <summary>
        /// Заполнен ли до противника ?
        /// </summary>
        bool CheckFillHome(Cell player)
        {
            for (int i = 0; i < HOMESIZE; i++)
            for (int j = 0; j < HOMESIZE; j++)
            {
                if(player == Cell.Black && Board[i, j] != Cell.Black) return false;
                if (player == Cell.White && Board[SIZE - i - 1, SIZE - j - 1] != Cell.White) return false;
            }

            return true;
        }
    }

    enum Cell
    {
        Empty = 0,
        White = 1,
        Black = 2
    }
}
