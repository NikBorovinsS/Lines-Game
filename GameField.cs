using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lines
{
    public partial class GameField : UserControl
    {

        GDI_Game _game ;
        Form1 main;

        public System.Windows.Forms.TextBox scoreBox;


        public GameField(Form1 main)
        {
            InitializeComponent();
            this.main = main;

            _game = new GDI_Game();

            _game.GetBall();
            _game.GetBall();
            _game.GetBall();
        }

        private void GameField_Paint(object sender, PaintEventArgs e)
        {
            _game.DrawField( e.Graphics);


        }

        private void GameField_Resize(object sender, EventArgs e)
        {
            _game.Resize(ClientRectangle);
        }

        private void GameField_MouseClick(object sender, MouseEventArgs e)
        {
            Point ptClick = new Point(e.X, e.Y);

            Graphics gr = CreateGraphics();

            bool isBall = _game.ChoosBallRect(ref ptClick, gr);

            if(!isBall && _game.choosBall._point!=new Point(-1,-1))
                _game.MoveBall(ptClick, gr);

            scoreBox.Text = _game.Scores.ToString();

            if (!_game.StatusGame)
            {
                main.ShowMessage();
                RestartGame();
                scoreBox.Text = _game.Scores.ToString();
            }
            

        }

        public void RestartGame()
        {
            Graphics Gr=CreateGraphics();

            _game._RestartGame(Gr);

            scoreBox.Text = _game.Scores.ToString();
        }
    }
}
