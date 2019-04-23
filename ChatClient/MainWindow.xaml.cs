using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket client;
        static byte[] _buffer = new byte[512];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {        
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                (sender as Button).IsEnabled = false;
                buttonDisconnect.IsEnabled = true;
                listBoxOutput.Items.Clear();

                IPAddress ipAdress = IPAddress.Parse(textBoxIpAddress.Text);
                int port = Convert.ToInt32(textBoxPort.Text);

                client.BeginConnect(ipAdress, port, new AsyncCallback(ConnectCallback), client);

                client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), client);
            }
            catch (Exception ex)
            {
                (sender as Button).IsEnabled = true;
                buttonDisconnect.IsEnabled = false;
                MessageBox.Show(ex.ToString());
            }

        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                string pattern = "#NICK#";
                string nick="";
                byte[] nickBytes = new byte[100];

                this.Dispatcher.Invoke((Action)(() =>
                {
                    nick = textBoxNick.Text;
                }));
                nickBytes = Encoding.UTF8.GetBytes(pattern + nick);
                client.BeginSend(nickBytes, 0, nickBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);

                Socket socket = asyncResult.AsyncState as Socket;
                socket.EndConnect(asyncResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

                this.Dispatcher.Invoke((Action)(() =>
                {
                    listBoxOutput.Items.Add(message);
                }));

                client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), client);
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }     
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            (sender as Button).IsEnabled = false;
            buttonConnect.IsEnabled = true;

            listBoxOutput.Items.Add("Odłączono z serwera");
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            string message = textBoxInput.Text;

            if(string.IsNullOrEmpty(message))
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
        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            socket.EndSend(asyncResult);
        }
    }
}
