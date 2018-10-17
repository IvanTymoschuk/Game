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

        public string ReadMessage()
        {
            NetworkStream str = Tcp.GetStream();
            byte[] arr = new byte[256];
            int bytes = str.Read(arr, 0, 256);
            return Encoding.Unicode.GetString(arr, 0, bytes);
        }

        public void Write(string mes)
        {
            NetworkStream str = Tcp.GetStream();
            byte[] arr = Encoding.Unicode.GetBytes(mes);
            str.Write(arr, 0, arr.Length);
        }

    }
}
