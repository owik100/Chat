using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        TcpListener server;
        TcpClient client;

        private delegate void SetLogDelegate(string text);

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                (sender as Button).Enabled = false;
                buttonStop.Enabled = true;
                listBoxOutput.Items.Clear();

                IPAddress ipAdress = IPAddress.Parse(textBoxAdress.Text);
                int port = Convert.ToInt32(numericUpDownPort.Value);

                server = new TcpListener(ipAdress, port);
                server.Start();
                listBoxOutput.Items.Add($"Server nasłuchuje...");

                server.BeginAcceptTcpClient(new AsyncCallback(WaitForConnection), server);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        void WaitForConnection(IAsyncResult asyncResult)
        {
            if (!server.Server.IsBound || server.Server == null)
                return;

            try
            {
                client = server.EndAcceptTcpClient(asyncResult);
                string clientIP = client.Client.RemoteEndPoint.ToString();
                Invoke(new SetLogDelegate(SetLogText), $"Klient połączony! - {clientIP}");

                BinaryWriter writer = new BinaryWriter(client.GetStream());

                writer.Write($"{DateTime.Now} - Wysłano dane z serwera!");
            }
            catch (ObjectDisposedException)
            {
                return;
            }
          

        }

        private void SetLogText(string strText)
        {
            listBoxOutput.Items.Add(strText);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (client!=null)
            {
                client.Close();              
            }

            server.Stop();

           
            (sender as Button).Enabled = false;
            buttonStart.Enabled = true;

            Invoke(new SetLogDelegate(SetLogText), "Serwer wyłączony");
        }
    }
}
