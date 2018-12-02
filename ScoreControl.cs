using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lines
{
    public partial class ScoreControl : UserControl
    {
        public int Scores { get; set; }

        public ScoreControl()
        {
            InitializeComponent();

            Scores = 0;
        }

        protected void DrawScores(Graphics Gr)
        {
            string score = Scores.ToString();
            FontFamily fontfam = new FontFamily(GenericFontFamilies.Monospace);
            Font font = new Font(fontfam, 30);

            Gr.DrawString(score, font, new SolidBrush(Color.White),
                new PointF(ClientRectangle.X, ClientRectangle.Y));

        }

        public void ScoreControl_Paint(object sender, PaintEventArgs e)
        {
            DrawScores(e.Graphics);
        }
    }
}
