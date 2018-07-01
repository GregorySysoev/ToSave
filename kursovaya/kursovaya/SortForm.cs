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
        bool radioButton1_WasClicked = true;
        bool radioButton2_WasClicked = false;
        bool radioButton3_WasClicked = false;
        bool radioButton4_WasClicked = false;

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
            radioButton4_WasClicked = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = false;
            radioButton2_WasClicked = true;
            radioButton3_WasClicked = false;
            radioButton4_WasClicked = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = true;
            radioButton2_WasClicked = false;
            radioButton3_WasClicked = false;
            radioButton4_WasClicked = false;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            GenerateFile abc = new GenerateFile();
            if (radioButton1_WasClicked) abc.CreateFile(1, (int)numericUpDown1.Value);
            if (radioButton2_WasClicked) abc.CreateFile(2, (int)numericUpDown1.Value);
            if (radioButton3_WasClicked) abc.CreateFile(3, (int)numericUpDown1.Value);
            if (radioButton4_WasClicked) abc.CreateFile(4, (int)numericUpDown1.Value);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "C:/Users/User/Documents/CursWork/sequencys";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.Multiselect = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            using (myStream)
                            {
                                ExperementForm f = new ExperementForm(openFileDialog1);
                                f.ShowDialog();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_WasClicked = false;
            radioButton2_WasClicked = false;
            radioButton3_WasClicked = false;
            radioButton4_WasClicked = true;
        }

        private void SortForm_Load(object sender, EventArgs e)
        {

        }
    }
}
