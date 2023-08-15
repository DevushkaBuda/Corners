using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApplication340.Model;

namespace WindowsFormsApplication340.Controls
{
    class BoardPanel : UserControl
    {
        public Game Game { get; private set; }

        public BoardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        public void Build(Game game)
        {
            this.Game = game;
        }

        private Point hoveredCell = new Point(-1, -1);
        private Color hoveredColor;
        private Point selectedCell = new Point(-1, -1);

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Game == null) return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            var rect = ClientRectangle;

            //render field
            using (var brush = new SolidBrush(Color.FromArgb(210, 180, 140)))
                g.FillRectangle(brush, rect);
            g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            var s = (Game.SIZE + 1);

            using(var brush = new SolidBrush(Color.FromArgb(112, 83, 53)))
            for (int y = 0; y < Game.SIZE; y++)
            for (int x = 0; x < Game.SIZE; x++)
            {
                var r = CellToRect(new Point(x, y));
                if((x + y) % 2 == 0)
                    g.FillRectangle(brush, r);

                r.Inflate(-2, -2);
                if(hoveredCell.X == x && hoveredCell.Y == y)
                using(var pen = new Pen(hoveredColor))
                    g.DrawRectangle(pen, r);
                if (selectedCell.X == x && selectedCell.Y == y)
                    g.FillRectangle(Brushes.OrangeRed, r);
            }

            //render cells
            for (int y = 0; y < Game.SIZE; y++)
            for (int x = 0; x < Game.SIZE; x++)
            {
                var r = CellToRect(new Point(x, y));
                r.Inflate(-r.Width / 7, -r.Width / 7);
                if (Game.Board[x, y] == Cell.Black)
                    g.FillEllipse(Brushes.Black, r);
                if (Game.Board[x, y] == Cell.White)
                    g.FillEllipse(Brushes.White, r);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var cell = PointToCell(e.Location);
            hoveredCell = cell;
            if (Game.CanMove(Game.CurrentPlayer, selectedCell, cell))
                hoveredColor = Color.Red;
            else
                hoveredColor = Color.FromArgb(200, 200, 200, 200);

            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                var cell = PointToCell(e.Location);
                if (selectedCell == cell)
                    selectedCell = new Point(-1, -1);
                else
                if (Game[cell] == Game.CurrentPlayer)
                    selectedCell = cell;
                else
                if (Game.InBoard(selectedCell))
                {
                    if (Game.CanMove(Game.CurrentPlayer, selectedCell, cell))
                    {
                        Game.MakeMove(selectedCell, cell);
                        selectedCell = hoveredCell = new Point(-1, -1);
                    }
                }
            }

            Invalidate();
        }

        Point PointToCell(Point p)
        {
            return new Point(Game.SIZE * p.X / ClientSize.Width, Game.SIZE - Game.SIZE * p.Y / ClientSize.Height - 1);
        }

        Rectangle CellToRect(Point cell)
        {
            return new Rectangle(cell.X * ClientSize.Width / Game.SIZE, ClientSize.Height - (cell.Y + 1) * ClientSize.Height / Game.SIZE, ClientSize.Width / Game.SIZE, ClientSize.Height / Game.SIZE);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BoardPanel
            // 
            this.Name = "BoardPanel";
            this.Load += new System.EventHandler(this.BoardPanel_Load);
            this.ResumeLayout(false);

        }

        private void BoardPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
