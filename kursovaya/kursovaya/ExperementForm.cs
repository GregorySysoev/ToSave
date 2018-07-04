using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimSort;
using static kursovaya.Comparers;

namespace kursovaya
{
    public static class Comparers
    {
        public class ComparerInt : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                return x - y;
            }
        }
        public class ComparerString : StringComparer
        {
            public override int Compare(string x, string y)
            {
                return String.Compare(x, y);
            }

            public override bool Equals(string x, string y)
            {
                return x == y;
            }

            public override int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
   
    public struct Terzia
    {
        public int compares;
        public int changes;
        public int time;
        public Terzia(int comp, int chan, int tm)
        {
            compares = comp;
            changes = chan;
            time = tm;
        }
    }

    public partial class ExperementForm : Form
    {
        public ExperementForm(OpenFileDialog openFileDialog1)
        {
            double countFiles = 0;
            InitializeComponent();
            chart1.Series.Clear();
            chart2.Series.Clear();
            chart3.Series.Clear();
            
            foreach (String file in openFileDialog1.FileNames)
            {
                int R_c = 0;
                int B_c = 0;
                int G_c = 0;

            this.tabPage4.Text = "Пересылки";
                Terzia TimTerz = new Terzia(0, 0, 0);
                Terzia MergTerz = new Terzia(0, 0, 0);

                using (var sr = new StreamReader(file))
                {
                    int[] arr = new int[File.ReadAllLines(file).Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                       arr[i] = Convert.ToInt32(sr.ReadLine());
                    }

                    int[] tsv = new int[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        tsv[i] = arr[i];
                    }
                    TimSortExtender.TimSort<int>(arr, ref TimTerz.changes, ref TimTerz.compares, ref TimTerz.time);
                    MergeSortAlgorithm.MergeSort(arr, ref MergTerz);
                    
                    string fileName = file.Substring(43);
                    string f1 = fileName + "TimSort";
                    string f2 = fileName + "MergeSort";
                    int midTimeTsort = 0;
                    int midTimeMsort = 0;
                    int[] qwerty = new int[15];
                    int MX = 0, MN = 0;
                    int MX_index = 0, MN_index = 0;

                    Random rnd = new Random();

                    R_c = rnd.Next(256);
                    B_c = 0;
                    G_c = rnd.Next(256);

                    chart3.Series.Add(f2);
                    chart3.Series[f2].Points.AddXY(countFiles, MergTerz.changes);
                    chart2.Series.Add(f2);
                    chart2.Series[f2].Points.AddXY(countFiles, MergTerz.compares);
                    chart1.Series.Add(f2);

                    for (int uio = 0; uio < 15; uio++)
                    {
                        MergeSortAlgorithm.MergeSort(arr, ref MergTerz);
                        qwerty[uio] = MergTerz.time;
                    }

                    MX = 0;
                    for (int uio = 0; uio < 15; uio++)
                    {
                        if (MX <= qwerty[uio])
                        {
                            MX = qwerty[uio];
                            MX_index = uio;
                        }
                    }
                    qwerty[MX_index] = 0;
                    MN = MX;
                    for (int uio = 0; uio < 15; uio++)
                    {
                        if (MN >= qwerty[uio])
                        {
                            MN = qwerty[uio];
                            MN_index = uio;
                        }
                    }
                    qwerty[MN_index] = 0;
                    for (int uio = 0; uio < 15; uio++) midTimeMsort += qwerty[uio];

                    MergTerz.time = midTimeMsort / 13;
                    chart1.Series[f2].Points.AddXY(countFiles, MergTerz.time);

                    chart3.Series[f2].Color = Color.FromArgb(R_c, B_c, G_c);
                    chart2.Series[f2].Color = Color.FromArgb(R_c, B_c, G_c);
                    chart1.Series[f2].Color = Color.FromArgb(R_c, B_c, G_c);

                    R_c = rnd.Next(256);
                    B_c = rnd.Next(256);
                    G_c = 0;

                    chart3.Series.Add(f1);
                    chart3.Series[f1].Points.AddXY(countFiles, TimTerz.changes);
                    chart2.Series.Add(f1);
                    chart2.Series[f1].Points.AddXY(countFiles, TimTerz.compares);
                    chart1.Series.Add(f1);
                    
                    for (int uio = 0; uio < 15; uio++)
                    {
                        TimSortExtender.TimSort<int>(arr, ref TimTerz.changes, ref TimTerz.compares, ref TimTerz.time);
                        qwerty[uio] = TimTerz.time;
                        for(int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = tsv[i];
                        }
                    }

                    MX = 0;
                    for (int uio = 0; uio < 15; uio++)
                    {
                        if (MX <= qwerty[uio])
                        {
                            MX = qwerty[uio];
                            MX_index = uio;
                        }
                    }
                    qwerty[MX_index] = 0;
                    MN = MX;
                    for (int uio = 0; uio < 15; uio++)
                    {
                        if (MN >= qwerty[uio])
                        {
                            MN = qwerty[uio];
                            MN_index = uio;
                        }
                    }
                    qwerty[MN_index] = 0;
                    for (int uio = 0; uio < 15; uio++) midTimeTsort += qwerty[uio];

                    TimTerz.time = midTimeTsort / 13;
                    chart1.Series[f1].Points.AddXY(countFiles, TimTerz.time);

                    chart3.Series[f1].Color = Color.FromArgb(R_c, B_c, G_c);
                    chart2.Series[f1].Color = Color.FromArgb(R_c, B_c, G_c);
                    chart1.Series[f1].Color = Color.FromArgb(R_c, B_c, G_c);

                    sr.Close();
                }
            }
        }

        private void ExperementForm_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

    }
}
