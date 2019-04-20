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
        static TcpListener _server;
        static List<TcpClient> _clientsList = new List<TcpClient>();

        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            IPAddress ipAdress = IPAddress.Parse("127.0.0.1");
            int port = 3000;

            Console.WriteLine("Uruchamianie serwera...");
            _server = new TcpListener(ipAdress, port);
            _server.Start();
            _server.BeginAcceptTcpClient(new AsyncCallback(WaitForConnection), null);
        }

        private static void WaitForConnection(IAsyncResult asyncCallback)
        {
            TcpClient client = _server.EndAcceptTcpClient(asyncCallback);
            _clientsList.Add(client);

            Console.WriteLine($"[{_clientsList.Count }] Klient połączony! , Adres : {client.Client.RemoteEndPoint} ");

           _server.BeginAcceptSocket(WaitForConnection, null);
        }
    }
}
