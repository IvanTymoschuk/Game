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

        int X;
        int Y;

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
                        ParseCoords(message);
                        if (message.Contains("#matrix") == true)
                        {
                            u.SetMatrix(message);
                            Print($"{u.Tcp.Client.RemoteEndPoint.ToString()} SEND MATRIX", ConsoleColor.DarkYellow);
                            u.PrintMatrix();
                        }
                        else
                        {
                            Print($"{u.Tcp.Client.RemoteEndPoint.ToString()} SEND COORDS: {message}", ConsoleColor.DarkRed);
                            foreach (var el in Players)
                            {
                                if (el.Tcp.Client.RemoteEndPoint != u.Tcp.Client.RemoteEndPoint)
                                {
                                    // message  - coords;
                                    // true - Step;
                                    // true or false - isHit = true/false;
                                    el.Write(message + " true true");
                                    Print("\t SERVER SEND " + message + " true true TO " + el.Tcp.Client.RemoteEndPoint, ConsoleColor.Cyan);
                                }
                                else
                                {
                                    // false - Step;
                                    // true or false - isHit = true/false;
                                    el.Write("false true");
                                    Print("\t SERVER SEND FALSE  TO " + el.Tcp.Client.RemoteEndPoint, ConsoleColor.DarkCyan);
                                }
                            }
                        }
                    }

                });
            }
        }
        private void ParseCoords(string coords)
        {
            string[] coord = coords.Split(' ');
            if(coord[0]!=null)
            {
                if (coord[0] == "A")
                    Y = 0;
                if (coord[0] == "B")
                    Y = 1;
                if (coord[0] == "C")
                    Y = 2;
                if (coord[0] == "D")
                    Y = 3;
                if (coord[0] == "E")
                    Y = 4;
                if (coord[0] == "F")
                    Y = 5;
                if (coord[0] == "G")
                    Y = 6;
                if (coord[0] == "H")
                    Y = 7;
                if (coord[0] == "I")
                    Y = 8;
                if (coord[0] == "J")
                    Y = 9;
                try
                {
                    X = Convert.ToInt32(coord[1]);
                }
                catch(Exception)
                {

                }
                Console.WriteLine("\t SERVER PARSE " + coords + " TO Y:" + Y + " X:" + X);
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