﻿using Bot.Extensions;
using Bot.Singletons;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bot.Forms
{
    public partial class LogForm : Form
    {
        Form Launcher;
        public LogForm(Form Launcher)
        {
            InitializeComponent();
            this.Launcher = Launcher;
        }

        private void AdicionarLinha(ref RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public void Log(LogEmiter.TipoLog tipoLog, string e)
        {
            try
            {
                if (Created)
                {
                    txLog.Invoke((MethodInvoker)delegate
                    {
                        AdicionarLinha(ref txLog, $"\r\n{e}", tipoLog.CorNoDrawing);
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                MessageBox.Show("O Bot foi desligado");
            }
        }

        private void BtLimpar_Click(object sender, EventArgs e)
        {
            txLog.Text = "";
        }

        private void BtDesligar_Click(object sender, EventArgs e)
        {
            SingletonClient.Client.StopAsync().GetAwaiter().GetResult();
            SingletonClient.setNull();
            Launcher.Show();
            Close();
        }

        private void VerLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SingletonClient.Client.StopAsync().GetAwaiter().GetResult();
            SingletonClient.setNull();
            Launcher.Show();
        }

        private void SairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void LogForm_Resize(object sender, EventArgs e)
        {
            if (((Form)sender).WindowState != FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }
    }
}
