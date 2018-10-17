﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerShip
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener(localAddr,1488);
            server.Start();
            Console.WriteLine("Waiting for users...");
            while (true)
            {
              
                Player player1= new Player(server.AcceptTcpClient());
                Console.WriteLine(server.LocalEndpoint.ToString()+ " connected to server!");
            }


        }
    }
}
