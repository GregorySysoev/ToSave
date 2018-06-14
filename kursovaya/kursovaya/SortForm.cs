using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using kursovaya;
namespace kursovaya
{
    public partial class SortForm : Form
    {
        bool radioButton1_WasClicked = false;
        bool radioButton2_WasClicked = false;
        bool radioButton3_WasClicked = false;

        public SortForm()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = false;
            radioButton2_WasClicked = false;
            radioButton3_WasClicked = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = false;
            radioButton2_WasClicked = true;
            radioButton3_WasClicked = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = true;
            radioButton2_WasClicked = false;
            radioButton3_WasClicked = false;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GenerateFile abc = new GenerateFile();
            if (radioButton1_WasClicked) abc.CreateFile(1,10);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
