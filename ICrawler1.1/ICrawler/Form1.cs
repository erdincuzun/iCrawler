using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace ICrawler
{
    public partial class Form1 : Form
    {
        private Crawler.Crawler _c;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            

        }

        int secim;
        string dir;
        private void button1_Click(object sender, EventArgs e)
        {
            bool hata = false;
            if (textBox1.Text.Length > 1)
                if (textBox1.Text[textBox1.Text.Length - 1] != '/')
                    textBox1.Text = textBox1.Text + "/";

            dir = textBox1.Text;
            dir = dir.Replace("http://", "");
            dir = dir.Replace("www.", "");
            dir = dir.Replace("/", "");

            secim = 0;
            if (radioButton1.Checked)
                secim = 1;
            if (radioButton2.Checked)
            {
                if (!File.Exists(dir + "\\rule.xml"))
                {
                    MessageBox.Show("No rule file (rule.xml)");
                    hata = true;
                }
                secim = 2;
            }
            if (radioButton3.Checked)
                secim = 3;

            if (!hata)
            {
                bgw.RunWorkerAsync();
                BTN_Start.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            About _a = new About();
            _a.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _c = new Crawler.Crawler();         

            if(!BTN_Start.Enabled)
                _c.crawl(textBox1.Text, dir, Convert.ToInt32(textBox2.Text), secim, bgw, e, false, checkBox1.Checked);

            if(!BTN_Start2.Enabled)
                _c.crawl(textBox3.Text, dir, Convert.ToInt32(textBox2.Text), secim, bgw, e, true, checkBox1.Checked);
        }

        private void BTN_Stop_Click(object sender, EventArgs e)
        {
            bgw.CancelAsync();
            progressBar1.Value = 0;
            BTN_Start.Enabled = true;
            BTN_Start2.Enabled = true;
            progressBar1.Visible = false;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation Cancelled");
                BTN_Start.Enabled = true;
                BTN_Start2.Enabled = true;
                progressBar1.Value = 0;
            }
            else
            {
                MessageBox.Show("Operation Completed");
                BTN_Start.Enabled = true;
                BTN_Start2.Enabled = true;
            }

            progressBar1.Visible = false;
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void BTN_Start2_Click(object sender, EventArgs e)
        {
            bool hata = false;
            if (textBox1.Text.Length > 1)
                if (textBox1.Text[textBox1.Text.Length - 1] != '/')
                    textBox1.Text = textBox1.Text + "/";

            dir = textBox1.Text;
            dir = dir.Replace("http://", "");
            dir = dir.Replace("www.", "");
            dir = dir.Replace("/", "");

            secim = 0;
            if (radioButton1.Checked)
                secim = 1;
            if (radioButton2.Checked)
            {
                if (!File.Exists(dir + "\\rule.xml"))
                {
                    MessageBox.Show("No rule file (rule.xml)");
                    hata = true;
                }
                secim = 2;
            }
            if (radioButton3.Checked)
                secim = 3;

            if (!hata)
            {
                bgw.RunWorkerAsync();
                BTN_Start2.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            }
        }

        private void BTN_Stop2_Click(object sender, EventArgs e)
        {
            bgw.CancelAsync();
            progressBar1.Value = 0;
            BTN_Start2.Enabled = true;
            progressBar1.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = !textBox2.Enabled;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "RuleEditor";
            //p.StartInfo.Arguments = "argument1 argument2 argument3";
            p.Start();
        }
    }
}
