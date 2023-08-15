using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Windows.Forms;
using System.Linq;
using WindowsFormsApplication340.Model;

namespace WindowsFormsApplication340
{
    public partial class MainForm : Form
    {
        private Game game;

        public MainForm()
        {
            InitializeComponent();

            game = new Game();
            pnBoard.Build(game);
            game.Moved += delegate { UpdateInterface(); };

            UpdateInterface();
        }

        private void UpdateInterface()
        {
            if (game.Winner != Cell.Empty)
                lbPlayer.Text = "Winner: " + game.Winner;
            else
                lbPlayer.Text = "Current player: " + game.CurrentPlayer;
        }
    }
}
