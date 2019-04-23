using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClientWindowsForm
{
    public partial class Form1 : Form
    {
        Socket client;
        static byte[] _buffer = new byte[512];

        private delegate void SetLogDelegate(string text);

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                (sender as Button).Enabled = false;
               
                listBoxOutput.Items.Clear();

                RefreshControlls(true);

                IPAddress ipAdress = IPAddress.Parse(textBoxIpAddress.Text);
                int port = Convert.ToInt32(numericUpDownPort.Text);

                client.BeginConnect(ipAdress, port, new AsyncCallback(ConnectCallback), client);

                client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), client);
            }
            catch (Exception ex)
            {
                RefreshControlls(false);
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                string nick = "";
                byte[] nickBytes = new byte[100];

              
                    nick = textBoxNickName.Text;

                nickBytes = Encoding.UTF8.GetBytes(nick);
                client.BeginSend(nickBytes, 0, nickBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);

                Socket socket = asyncResult.AsyncState as Socket;
                socket.EndConnect(asyncResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        void ReciveCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            try
            {
                int recived = client.EndReceive(asyncResult);
                byte[] dataBuf = new byte[recived];
                Array.Copy(_buffer, dataBuf, recived);

                string message = Encoding.UTF8.GetString(dataBuf);

                if(message.Length>0)
                    Invoke(new SetLogDelegate(SetLogText), message);


                client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), client);
            }
            catch (Exception ex)
            {
                 //MessageBox.Show(ex.Message);
            }
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            socket.EndSend(asyncResult);
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            RefreshControlls(false);

            Invoke(new SetLogDelegate(SetLogText), "Odłączono z serwera");
        }

        private void SetLogText(string strText)
        {
            listBoxOutput.Items.Add(strText);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string message = textBoxInput.Text;
            textBoxInput.Text = "";

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void RefreshControlls(bool connected)
        {
            if(connected)
            {
                textBoxIpAddress.Enabled = false;
                numericUpDownPort.Enabled = false;
                textBoxNickName.Enabled = false;
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
                buttonSend.Enabled = true;
            }
            else
            {
                textBoxIpAddress.Enabled = true;
                numericUpDownPort.Enabled = true;
                textBoxNickName.Enabled = false;
                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
                buttonSend.Enabled = false;
            }
        }

        private void buttonClearInput_Click(object sender, EventArgs e)
        {
            listBoxOutput.Items.Clear();
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSend_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }
    }
}
