using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Controls;

namespace BetterTyper
{
    public partial class mainForm : Form
    {
        public StreamWriter _swLog;

        public int eStop = 0;
        public int progressPart = 0;
        public int progressWhole = 0;

        public string versionNumber = "v3.5";
        public string logFilePath = "C:\\Auto Typer\\latest_log.txt";

        public mainForm()
        {
            Text = "Auto Typer " + versionNumber;
            InitializeComponent();
            ListView();
            richTextBox1.ReadOnly = true;
            richTextBox2.ReadOnly = true;
            richTextBox3.ReadOnly = true;

            stopButton.Enabled = false;

            bool exists = Directory.Exists(logFilePath);

            try
            {
                _swLog = new StreamWriter(logFilePath);

                //if (!File.Exists(logFilePath))
                //{
                //    Directory.CreateDirectory(logFilePath);
                //    Directory.CreateDirectory(logFilePath);
                //}

                StreamReader sr = new StreamReader("C:\\Auto Typer\\save.txt");
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line.ToString()))
                {
                    sr.Close();
                    return;
                }

                filePathTextBox.Text = line.ToString();

                sr.Close();
            }
            catch
            {
                //if (!File.Exists(logFilePath))
                //{
                //    Directory.CreateDirectory(logFilePath);
                //}
                return;
            }
        }

        public void StopProgram()
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;
            eStop = 1;
            Thread.Sleep(1000);
            filePathTextBox.ReadOnly = false;
            richTextBox3.Text = "Cancelled";
        }

        public void ListView()
        {
            ColumnHeader header = new ColumnHeader();
            header.Text = "";
            header.Name = "col1";
            header.Width = this.Width - 100;
            consoleDisplay.Columns.Add(header);
            consoleDisplay.View = View.Details;
            consoleDisplay.HeaderStyle = ColumnHeaderStyle.None;
        }

        public void GetProgressWhole()
        {
            progressWhole = 0;

            try
            {
                string filePath = filePathTextBox.Text.ToString();

                StreamReader sr = new StreamReader(filePath);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    progressWhole++;
                }
            }
            catch
            {
                return;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            int coolDown = (int)coolDownRaw.Value;
            eStop = 0;
            progressPart = 0;
            GetProgressWhole();

            startButton.Enabled = false;
            stopButton.Enabled = true;
            saveButton.Enabled = false;
            filePathTextBox.ReadOnly = true;

            progressBar.Maximum = progressWhole;
            richTextBox3.Text = "Progress";

            if (string.IsNullOrWhiteSpace(filePathTextBox.Text))
            {
                consoleDisplay.Items.Add("Error: No set file location");
                _swLog.WriteLine("Error: No set file location");
                _swLog.Flush();
                return;
            }

            Thread.Sleep(1500);

            consoleDisplay.Items.Add("Info: Running...");
            _swLog.WriteLine("Info: Running...");
            _swLog.Flush();

            var list = new List<string>();

            string filePath = filePathTextBox.Text.ToString();

            using (var sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (eStop != 1)
                    {
                        list.Add(line);
                        SendKeys.SendWait("{ENTER}");

                        consoleDisplay.Items.Add("Sending: " + line);
                        _swLog.WriteLine("Sending: " + line);
                        _swLog.Flush();

                        SendKeys.SendWait(line + "{ENTER}");
                        Thread.Sleep(Convert.ToInt32(coolDown));

                        progressPart++;
                        progressBar.Value = progressPart;

                        consoleDisplay.EnsureVisible(consoleDisplay.Items.Count - 1);
                    }
                    else
                    {
                        StopProgram();
                        return;
                    }
                }

                StopProgram();
                richTextBox3.Text = "Done";

            }

        }

        public void Save()
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Auto Typer\\save.txt");

                string filePathString = filePathTextBox.Text.ToString();
                sw.WriteLine(filePathString);

                sw.Close();

                StreamReader sr = new StreamReader("C:\\Auto Typer\\save.txt");
                string line = sr.ReadLine();

                if (line == filePathString)
                {
                    consoleDisplay.Items.Add("Info: Successfully Saved");
                    _swLog.WriteLine("Info: Successfully Saved");
                    _swLog.Flush();
                    sr.Close();
                    return;
                }
                else
                {
                    consoleDisplay.Items.Add("Error: File Not Saved Properly");
                    _swLog.WriteLine("Error: File Not Saved Properly");
                    _swLog.Flush();
                    sr.Close();
                    return;
                }
            }
            catch
            {
                Directory.CreateDirectory("C:\\Auto Typer");
                consoleDisplay.Items.Add("Error: File not found. Creating directory");
                consoleDisplay.Items.Add("Info: New directory created. Try saving again");
                _swLog = new StreamWriter(logFilePath);
                _swLog.WriteLine("Error: File not found. Creating directory");
                _swLog.WriteLine("Info: New directory created. Try saving again");
                _swLog.Flush();
            }
        }

        public void ChangeLogText()
        {
            MessageBox.Show("Current Changelog for Auto Typer " + versionNumber +
                "\r\n\r\nAdded Main Strip" +
                "\r\nAdded ChangeLog button" +
                "\r\nAdded latest_log.txt" +
                "\r\nAdded About button" +
                "\r\nAdded Exit button" +
                "\r\nAdded Progress bar" +
                "\r\n\r\nImproved save feature");
        }

        public void Help()
        {
            MessageBox.Show("To save file location for easier use go to your C: drive and create folder called 'Auto Typer'. The program will auto-complete the rest." +
                "\r\n\r\n'Start' Starts typing.\r\n\r\n'Stop' Stops typing." +
                "\r\n\r\n'Cool Down' Time between each line typed. For applications with spam protected");
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopProgram();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Auto Typer by Kyle (TheGuitarleader) " + versionNumber);
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLogText();
        }
    }
}
