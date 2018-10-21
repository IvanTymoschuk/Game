using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerShip
{
    public class Server
    {
        private readonly TcpListener listener;
        public List<Player> users;

        private int X;
        private int Y;

        public Server(string ip, int port)
        {
            // MY server location
            listener = new TcpListener(IPAddress.Parse(ip), port);
            users = new List<Player>();
        }

        public void ServerStart()
        {
            listener.Start();
            Console.WriteLine("Server Started...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Player u = new Player(client);
                users.Add(u);
                Print(u.Tcp.Client.RemoteEndPoint.ToString() + " Connected..", ConsoleColor.Green);
                if (users.Count == 2)
                {
                    Task.Run(() =>
                    {
                        List<Player> Players = new List<Player>();
                        foreach (Player el in users)
                            Players.Add(el);

                        Player p = null;

                        users.Clear();

                        Random rnd = new Random();
                        int first = rnd.Next(0, 0);

                        if (first == 0)
                        {
                            Players[0].Write("true");
                            Players[1].Write("false");
                            p = Players[0];
                        }
                        else
                        {
                            Players[1].Write("true");
                            Players[0].Write("false");
                            p = Players[1];
                        }

                        string message = null;
                        bool ishit = false;
                        while (true)
                        {
                            message = p.ReadMessage();

                            if (message.Contains("#matrix") == true)
                            {
                                p.SetMatrix(message);

                                //p.PrintMatrix();
                                foreach (var el in Players)
                                {
                                    if (el != p)
                                    {
                                        p.Write("false");
                                        el.Write("true");
                                        p = el;
                                    }
                                }
                            }
                            else
                            {
                                ParseCoords(message);

                                Player temp_player = null;
                                foreach (var el in Players)
                                {
                                    if (el.Tcp.Client.RemoteEndPoint != p.Tcp.Client.RemoteEndPoint)
                                    {
                                        ishit = el.isHit(Y, X);
                                        if (el.isLose())
                                        {
                                            el.Write("LOSE");
                                            p.Write("WIN");
                                            Console.WriteLine(p.Tcp.Client.RemoteEndPoint + " is WIN");
                                        }
                                        if (ishit == true)
                                        {
                                            el.Write(message + " false " + ishit.ToString().ToLower());
                                        }
                                        if (ishit == false)
                                        {
                                            el.Write(message + " true " + ishit.ToString().ToLower());
                                            temp_player = el;
                                        }
                                    }
                                }
                                if (ishit == true)
                                    p.Write("true " + ishit.ToString().ToLower());
                                if (ishit == false)
                                {
                                    p.Write("false " + ishit.ToString().ToLower());
                                    p = temp_player;
                                }
                            }
                        }
                    });
                }
            }
        }

        private void ParseCoords(string coords)
        {
            string[] coord = coords.Split(' ');
            if (coord[0] != null)
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
                catch (Exception)
                {
                }
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