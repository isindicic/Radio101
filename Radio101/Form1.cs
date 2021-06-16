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
using System.Reflection;

namespace Radio101
{
    public partial class Form1 : Form
    {
        bool portableMode = false;
        NAudioDemo.Mp3StreamingDemo.Mp3StreamingPanel player;


        public string SettingsFile {
            get {
                    if (this.portableMode)
                    {
                        UriBuilder ub = new UriBuilder( Assembly.GetExecutingAssembly().CodeBase ) ;
                        string appPath = Path.GetDirectoryName(Uri.UnescapeDataString(ub.Path));
                        
                        return Path.Combine(appPath, "Radio101.settings");
                    }
                    else
                        return Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) , "Radio101.settings" );  
                } 
        }


        public Form1() : this(false)
        { 
        }

        public Form1(bool portableMode)
        {
            InitializeComponent();

            this.portableMode = portableMode;
            this.LoadSettings();

            player = new NAudioDemo.Mp3StreamingDemo.Mp3StreamingPanel();
            player.Top = 0;
            player.Left = 0;
            player.Visible = true;
            player.volume = this.settings.volume;
            player.OnIcyData += (s, e) => { this.Invoke(new MethodInvoker(delegate { 
                lblStreamInfo.Text = player.StreamTitle;
                this.Text = "Radio101 - " + player.StreamTitle; 
            }));  };

            this.Controls.Add(player);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.url = "http://live.radio101.hr:9531/stream.mp3";
            player.PlayStream();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
        }


        #region Settings 

        private Settings settings;

        public class Settings
        {
            public float volume = 1.0f;
        }


        private void LoadSettings()
        {
            string sfn = this.SettingsFile;
            if (File.Exists(sfn))
            {
                // To be replaced with NewtonSoft.Json
                string json = File.ReadAllText(sfn);
                json = json.Replace("{", "")
                           .Replace("}", "")
                           .Replace("volume", "")
                           .Replace(":", "")
                           .Trim();
                this.settings = new Settings();
                try
                {
                    this.settings.volume = float.Parse(json, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch 
                { 
                }
            }
            else
                this.settings = new Settings();

        }

        private void SaveSettings()
        {
            // To be replaced with NewtonSoft.Json
            string sfn = this.SettingsFile;
            string json = "{ volume : " + player.volume.ToString(System.Globalization.CultureInfo.InvariantCulture) + "}";
            File.WriteAllText(sfn, json);
        }

        #endregion

    }
}
