using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerShip
{
    public class Server
    {
        private readonly TcpListener listener;
        public List<Player> Players;

        public Server(string ip, int port)
        {
            // MY server location
            listener = new TcpListener(IPAddress.Parse(ip), port);
            Players = new List<Player>();

        }

        public void ServerStart()
        {
            listener.Start();
            Console.WriteLine("Server Started...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();


                Task.Run(() =>
                {
                    Player u = new Player(client);

                    Players.Add(u);
                    Print(u.Tcp.Client.RemoteEndPoint.ToString() + " Connected..", ConsoleColor.Green);

                    while (true)
                    {
                        string message = u.ReadMessage();
                        Print($"{u.Tcp.Client.RemoteEndPoint.ToString()} SEND COORDS: {message}", ConsoleColor.Red);
                        foreach (var el in Players)
                            el.Write(message);
                    }

                });
            }
        }

        private void Print(string msg, ConsoleColor cc = ConsoleColor.White)
        {
            Console.ForegroundColor = cc;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }


    }
}