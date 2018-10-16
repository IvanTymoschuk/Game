using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerShip
{
   public class Player
    {

        public TcpClient Tcp { get; set; }
        public string Name { get; set; }
        public int Prior { get; set; }



        public Player(TcpClient client)
        {
            Tcp = client;
        }

    }
}
