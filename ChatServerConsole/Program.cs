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
        static List<Socket> _clientsList = new List<Socket>();

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
            _server.BeginAccept(new AsyncCallback(AcceptConnection), null);
            Console.WriteLine("Serwer nasłuchuje...");
        }

        private static void AcceptConnection(IAsyncResult asyncCallback)
        {
            Socket socket = _server.EndAccept(asyncCallback);
            _clientsList.Add(socket);
            Console.WriteLine($"[{_clientsList.Count }] Klient połączony! , Adres : {socket.RemoteEndPoint} ");

            byte[] message = Encoding.UTF8.GetBytes($"Jesteś połączony z serwerem {_serverName}");
            socket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
            _server.BeginAccept(AcceptConnection, null);
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

                string text = Encoding.UTF8.GetString(dataBuf);
                Console.WriteLine($"Otrzymana wiadomość: {text}");

                foreach (var item in _clientsList)
                {
                    item.Send(dataBuf);
                }
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
            }
            catch (Exception ex)
            {
                _clientsList.Remove(socket);
                Console.WriteLine($"Klient rozłączony - {socket.RemoteEndPoint}");
                socket.Close();
                socket.Dispose();
            }

        }
    }
}
