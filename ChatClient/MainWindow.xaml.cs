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
        TcpClient client;

        public MainWindow()
        {
            InitializeComponent();

           
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {

          
            try
            {
                client = new TcpClient();

                (sender as Button).IsEnabled = false;
                buttonDisconnect.IsEnabled = true;
                listBoxOutput.Items.Clear();

                IPAddress ipAdress = IPAddress.Parse(textBoxIpAddress.Text);
                int port = Convert.ToInt32(textBoxPort.Text);
                client.Connect(ipAdress, port);

                //BinaryReader reader = new BinaryReader(client.GetStream());
                //string x = reader.ReadString();

                if(client.Connected)

                listBoxOutput.Items.Add("Connected");

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }

            (sender as Button).IsEnabled = false;
            buttonConnect.IsEnabled = true;

            listBoxOutput.Items.Add("Serwer wyłączony");
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            string message = textBoxInput.Text;

            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                client.Client.Send(buffer);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
