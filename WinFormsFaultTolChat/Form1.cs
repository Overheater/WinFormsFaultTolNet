using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsFaultTolChat
{
    public partial class Form1 : Form
    {
        private Client CC;
        private System.Threading.Timer renewTimer;
        private List<string> messagesList = new List<string>();
        private int messageamount = 0;
        public Form1()
        {
            InitializeComponent();
            CC = new Client();
            List<String> messagesList;
            int messageamount = 0;
            CC.Connect();
            renewTimer = new System.Threading.Timer(new TimerCallback(updateEvent));
            renewTimer.Change(1000, 2500);
            
        }

        private void updateEvent(object state)
        {
            messagesList = CC.getmessages();
            if (messageamount != messagesList.Count())
            {
                for (int i = messageamount; i < messagesList.Count; i++)
                {
                    OutputText(messagesList[i]);
                }

                messageamount = messagesList.Count;
            }
        }
        private delegate void StringDelegate(string message);

        private void OutputText(string message)
        {
            if (Messages.InvokeRequired)
            {
                Messages.Invoke(new StringDelegate(OutputText), new object[] { message });
            }
            else
            {
                string Messagetype = message.Substring(0, 3);
                message = message.Substring(3, message.Length-3);
                if (Messagetype == "--S")
                {
                    Messages.AppendText(message + "\r\n",Color.BlueViolet);
                }
                else if (Messagetype == "--R")
                {
                    Messages.AppendText(message + "\r\n", Color.DarkTurquoise);
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Messages_TextChanged(object sender, EventArgs e)
        {

        }
        private void MessageBox_TextChanged(object sender, EventArgs e)
        {
            SetButtonVisibility();
        }
        private void SendButton_Click(object sender, System.EventArgs e)
        {
            CC.SendEncodedMessage(MessageBox.Text, CopyNumber.Text);
        }
        private void SetButtonVisibility()
        {
            if ((MessageBox.Text != String.Empty))
            {
                SendButton.Enabled = true;
            }
            else
            {
                SendButton.Enabled = false;

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CC.Closelisten();
            Thread.Sleep(300);
            CC = null;
            renewTimer.Dispose();
        }

        private void SendButton_Click_1(object sender, EventArgs e)
        {
            CC.SendEncodedMessage(MessageBox.Text,CopyNumber.Text);
            MessageBox.Clear();

        }
    }
}
public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = color;
        if (color == Color.BlueViolet)
        {
            box.SelectionAlignment = HorizontalAlignment.Right;
        }
        else if (color == Color.DarkTurquoise)
        {
            box.SelectionAlignment = HorizontalAlignment.Left;
        }

        box.AppendText(text);
        box.SelectionColor = box.BackColor;
    }
}
