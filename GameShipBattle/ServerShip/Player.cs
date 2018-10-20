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

        public int count_xp { get; set; }
        public int count_hit { get; set; }

        public int[,] field { get; set; }


        public Player(TcpClient client)
        {
            Tcp = client;
            field = new int[4, 2];
            count_hit = 0;
            count_xp = 20;
        }

        public string ReadMessage()
        {
            NetworkStream str = Tcp.GetStream();
            byte[] arr = new byte[256];
            int bytes = str.Read(arr, 0, 256);
            return Encoding.Unicode.GetString(arr, 0, bytes);
        }
        public void SetMatrix(string str)
        {
            string[] items = str.Split(' ');
            int a = 1;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    field[i, j] = Convert.ToInt32(items[a]);
                    a++;
                }
            }
        }
        public bool isHit(int y, int x)
        {
            if (field[y, x] == 1)
            {
                field[y, x] = 0;
                count_hit++;
                return true;
            }
            else
                return false;
        }
        public bool isLose()
        {
            if (count_xp == 0)
                return true;
            else
                return false;
        }
        public void PrintMatrix()
        {

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(field[i,j]+" ");
                }
                Console.WriteLine();
            }
        }
        public void Write(string mes)
        {
            NetworkStream str = Tcp.GetStream();
            byte[] arr = Encoding.Unicode.GetBytes(mes);
            str.Write(arr, 0, arr.Length);
        }

    }
}
