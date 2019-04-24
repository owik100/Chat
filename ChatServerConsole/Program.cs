using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerConsole
{
    class Program
    {
        static Socket _server;
        static Dictionary<Socket, string> _clientsList = new Dictionary<Socket, string>();

        static byte[] _buffer;
        static string _serverName;
        static IPAddress _ipAdress;
        static int _port;

        static void Main(string[] args)
        {
            LoadSettings();
            SetupServer();

            Console.ReadLine();
        }

        private static void LoadSettings()
        {
            SettingsModel settingsModel = SettingsAccess.ReadData();

            _serverName = settingsModel.SerwerName;
            _ipAdress = IPAddress.Parse(settingsModel.IpAdress);
            _port = settingsModel.Port;
            _buffer = new byte[settingsModel.BufferSize];
        }

        private static void SetupServer()
        {
            Console.Title = _serverName;
            Console.WriteLine("Uruchamianie serwera...");
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(_ipAdress, _port));
            _server.Listen(3);
            _server.BeginAccept(_server.ReceiveBufferSize,new AsyncCallback(AcceptConnection),null);
            Console.WriteLine("Serwer nasłuchuje...");
        }

        private static void AcceptConnection(IAsyncResult asyncCallback)
        {
            //Pobranie nicku
            byte[] buffer = new byte[_server.ReceiveBufferSize];
            Socket socket = _server.EndAccept(out buffer, asyncCallback);
            string userName = Encoding.UTF8.GetString(buffer);

            Console.WriteLine($"Klient połączony! Nick: {userName} , Adres: {socket.RemoteEndPoint}, połączonych łącznie [{_clientsList.Count + 1 }]");

            //Wysłanie inormacji o dołączeniu do wszystkich userów
            byte[] messageToAll = Encoding.UTF8.GetBytes($"{userName} połączony!");
            foreach (var item in _clientsList)
            {
                item.Key.BeginSend(messageToAll, 0, messageToAll.Length, SocketFlags.None, new AsyncCallback(SendCallback), item.Key);
            }

            //Wysłanie informacji do połączonego usera
            byte[] message = Encoding.UTF8.GetBytes($"Jesteś połączony z serwerem '{_serverName}'");
            socket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

            _clientsList.Add(socket, userName);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
            _server.BeginAccept(_server.ReceiveBufferSize, AcceptConnection, null);
        }

        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            socket.EndSend(asyncResult);
        }

        private static void ReciveCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            try
            {
                int recived = socket.EndReceive(asyncResult);
                byte[] dataBuf = new byte[recived];
                Array.Copy(_buffer, dataBuf, recived);

                string message = Encoding.UTF8.GetString(dataBuf);

                //Rozłączenie
                if(message == "")
                {
                    DisconnectClient(socket);
                    return;
                }

                Console.WriteLine($"Otrzymana wiadomość od {_clientsList[socket]} : {message}");

                message = DateTime.Now.ToShortTimeString() + " " + _clientsList[socket] + ": " + message;
                dataBuf = Encoding.UTF8.GetBytes(message);

                //Prześlij otrzymaną wiadomość do wszystkich userów
                foreach (var item in _clientsList)
                {
                    item.Key.BeginSend(dataBuf, 0, dataBuf.Length, SocketFlags.None, new AsyncCallback(SendCallback), item.Key);
                }
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
            }
            catch (Exception ex)
            {             
                DisconnectClient(socket);
            }
        }

        private static void DisconnectClient(Socket socket)
        {
            Console.WriteLine($"Klient {_clientsList[socket]} rozłączony, pozostało połączonych [{_clientsList.Count - 1 }]");

            byte[] message = Encoding.UTF8.GetBytes($"{_clientsList[socket]} rozłączony!");
            _clientsList.Remove(socket);

            //Prześlij informacje o rozłączonym userze do wszystkich pozostałych
            foreach (var item in _clientsList)
            {
                item.Key.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallback), item.Key);
            }

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket.Dispose();
        }
    }
}
