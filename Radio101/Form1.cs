using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Net;
using NAudio.Wave;

namespace Radio101
{
    public partial class Form1 : Form
    {
        NAudioDemo.Mp3StreamingDemo.Mp3StreamingPanel player;

        public Form1()
        {
            InitializeComponent();

            player = new NAudioDemo.Mp3StreamingDemo.Mp3StreamingPanel();
            player.Top = 0;
            player.Left = 0;
            player.Visible = true;
            this.Controls.Add(player);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.url = "http://live.radio101.hr:9531/stream.mp3";
            player.PlayStream();
        }
    }
}
