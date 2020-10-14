using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using NeuralNetworkV1;
using System.IO;
using System.Xml.Serialization;

namespace NeuralNetDemo
{
    public partial class Form1 : Form
    {
        NeuralNet Net;
        int progres = 0;
        string currentPath;
        int count = 0;
        int num = 0;
        bool work = true;

        StreamReader reader = new StreamReader("mnist_train.csv");
        public Form1()
        {
            InitializeComponent();
        }

        private void Draw(int num, int count, int vals, Image digit, int i)
        {
            textBox2.Text = $"Accuracy {num} {(double)num / i * 100:0.0}%";
            textBox3.Text = vals.ToString();
            progressBar1.Value = i;
            pictureBox1.Image = digit;
            textBox1.Text = $"{i + 1}/60000  {(i + 1) / 600}%";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Net = new NeuralNet(784, 3, 400, 10);
            button3.Enabled = true;
            /*
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName, false);
                XmlSerializer serializer = new XmlSerializer(typeof(NeuralNet));
                serializer.Serialize(writer, Net);
                currentPath = openFileDialog1.FileName;
                button3.Enabled = true;
                writer.Flush();
                writer.Close();
            }
            else MessageBox.Show("Error", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
            */
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(openFileDialog1.FileName);
                XmlSerializer serializer = new XmlSerializer(typeof(NeuralNet));
                Net = (NeuralNet)serializer.Deserialize(reader);
                Net.speed = 0.1;
                currentPath = openFileDialog1.FileName;
                button3.Enabled = true;
                reader.Close();
            }
            else MessageBox.Show("Error", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = work;
            timer1.Enabled = work;
            if (!work) button3.Text = "Старт";
            else button3.Text = "Стоп";
            /*
                if (i % 1000 == 0 & i != 0)
                {
                    StreamWriter writer = new StreamWriter(saveFileDialog1.FileName, false);
                    XmlSerializer serializer = new XmlSerializer(typeof(NeuralNet));
                    serializer.Serialize(writer, Net);
                    writer.Flush();
                    writer.Close();
                }
                */
            work = !work;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            progres++;
            string line = reader.ReadLine();
            string[] parse = line.Split(',');
            int[] vals = new int[parse.Length];
            Bitmap digit = new Bitmap(28*2, 28*2);
            double[] input = new double[784];
            double[] expected = new double[10];

            for (int j = 0; j < parse.Length; j++)
            {
                vals[j] = int.Parse(parse[j]);
            }

            for (int j = 1; j < 785; j++)
            {
                input[j - 1] = vals[j];
            }

            expected[vals[0]] = 1;

            for (int m = 0; m < 28; m++)
            {
                for (int n = 0; n < 28; n++)
                {
                    digit.SetPixel(n * 2, m * 2, Color.FromArgb(vals[m * 28 + n + 1], vals[m * 28 + n + 1], vals[m * 28 + n + 1]));
                    digit.SetPixel(n * 2 + 1, m * 2, Color.FromArgb(vals[m * 28 + n + 1], vals[m * 28 + n + 1], vals[m * 28 + n + 1]));
                    digit.SetPixel(n * 2, m * 2 + 1, Color.FromArgb(vals[m * 28 + n + 1], vals[m * 28 + n + 1], vals[m * 28 + n + 1]));
                    digit.SetPixel(n * 2 + 1, m * 2 + 1, Color.FromArgb(vals[m * 28 + n + 1], vals[m * 28 + n + 1], vals[m * 28 + n + 1]));

                }
            }

            count++;

            //Gui
            /*
            textBox2.Text = $"Accuracy {num / count * 100}";
            textBox3.Text = vals[0].ToString();
            progressBar1.Value = i;
            pictureBox1.Image = digit;
            textBox1.Text = $"{i + 1}/60000  {(i + 1) / 600}%";
            */
            if (progres != 0)
            {
                Draw(num, count, vals[0], digit, progres);
                Invalidate();
                Refresh();
                Update();
                //.Sleep(500);
            }
            //Gui

            //Net
            double[] result = Net.Learn(input, expected);
            double max = 0;
            int no = -1;
            richTextBox1.Clear();
            for (int j = 0; j < 10; j++)
            {
                richTextBox1.AppendText($"{j} = {result[j] * 100:0.000}%\n");
                if (result[j] > max && result[j] > 0.5)
                {
                    max = result[j];
                    no = j;
                }
            }
            richTextBox1.AppendText($"Predicted answer {no}");
            if (no == vals[0])
            {
                num++;
                // = Color.Green;
            }
            else
            {
               // Form1.DefaultBackColor = Color.Red;
            }
            //Net

            //Thread.Sleep(5000);
            /*
            if (progres % 100 == 0 & progres != 0)
            {
                count = 0;
                num = 0;
            }
            */
            if (progres == 60000) timer1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Net(*.net)|*.net";
            saveFileDialog1.Filter = "Net(*.net)|*.net";
            saveFileDialog1.AddExtension = true;
        }
    }
}
