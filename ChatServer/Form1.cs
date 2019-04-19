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

            client = server.EndAcceptTcpClient(asyncResult);
            string clientIP = client.Client.RemoteEndPoint.ToString();
            Invoke(new SetLogDelegate(SetLogText), $"Klient połączony! - {clientIP}");

            BinaryWriter writer = new BinaryWriter(client.GetStream());

            writer.Write($"{DateTime.Now} - Wysłano dane z serwera!");


            // client.Close();
            // server.Stop();
        }

        private void SetLogText(string strText)
        {
            listBoxOutput.Items.Add(strText);
        }
    }
}
