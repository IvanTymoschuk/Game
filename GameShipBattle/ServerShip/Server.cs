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
        public bool isGameStarted = false;

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

                    // start game 

                    if(Players.Count==2 && isGameStarted==false)
                    {
                        isGameStarted = true;
                        Random rnd = new Random();
                        int first = rnd.Next(0, 1);
                        if(first==0)
                        {
                            Players[0].Write("true");
                            Players[1].Write("false");
                            Print("\t SERVER SEND TRUE  TO " + Players[0].Tcp.Client.RemoteEndPoint, ConsoleColor.Magenta);
                            Print("\t SERVER SEND FALSE  TO " + Players[1].Tcp.Client.RemoteEndPoint, ConsoleColor.Magenta);
                        }
                        else
                        {
                            Players[1].Write("true");
                            Players[0].Write("false");
                            Print("\t SERVER SEND TRUE  TO " + Players[1].Tcp.Client.RemoteEndPoint, ConsoleColor.Magenta);
                            Print("\t SERVER SEND FALSE  TO " + Players[0].Tcp.Client.RemoteEndPoint, ConsoleColor.Magenta);
                        }
                    }
                    while (true)
                    {
                        string message = u.ReadMessage();
                        Print($"{u.Tcp.Client.RemoteEndPoint.ToString()} SEND COORDS: {message}", ConsoleColor.DarkRed);
                        foreach (var el in Players)
                        {
                            if (el.Tcp.Client.RemoteEndPoint != u.Tcp.Client.RemoteEndPoint)
                            {
                                // message  - coords;
                                // true - Step;
                                // false or true - is hit;
                                el.Write(message+" true false");
                                Print("\t SERVER SEND " + message + " true false TO " + el.Tcp.Client.RemoteEndPoint, ConsoleColor.Cyan);
                            }
                            else
                            {
                                // false - Step;
                                el.Write("false");
                                Print("\t SERVER SEND FALSE  TO " + el.Tcp.Client.RemoteEndPoint, ConsoleColor.DarkCyan);
                            }
                        }
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