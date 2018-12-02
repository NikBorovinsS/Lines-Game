using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lines
{


    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            gameField1.scoreBox = textBox1;
            gameField1.scoreBox.Text = "0";

            //Icon ico = new Icon("C:\\Users\\Programmer\\Desktop\\MyLines\\MyLines_\\MyLines\\Lines\\Lines\\nikita-01.ico");

            //this.Icon = ico;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            gameField1.RestartGame();
        }

        public void ShowMessage()
        {
            ChildForm child = new ChildForm();

            child.ShowDialog();

        }
    }
}
