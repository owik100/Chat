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

        static byte[] _buffer = new byte[512];

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
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(ipAdress, port));
            _server.Listen(3);
            _server.BeginAccept(new AsyncCallback(AcceptConnection), null);
            Console.WriteLine("Serwer nasłuchuje...");
        }

        private static void AcceptConnection(IAsyncResult asyncCallback)
        {
            Socket socket = _server.EndAccept(asyncCallback);
            _clientsList.Add(socket);
            Console.WriteLine($"[{_clientsList.Count }] Klient połączony! , Adres : {socket.RemoteEndPoint} ");

            byte[] message = Encoding.UTF8.GetBytes("Wiadomość z serwera!");
            socket.Send(message);

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
            int recived = socket.EndReceive(asyncResult);
            byte[] dataBuf = new byte[recived];
            Array.Copy(_buffer, dataBuf, recived);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine($"Otrzymana wiadomość: {text}");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReciveCallback), socket);
        }
    }
}
