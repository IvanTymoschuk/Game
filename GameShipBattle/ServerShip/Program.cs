namespace ServerShip
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Player player1 = null;
            //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            //TcpListener server = new TcpListener(localAddr,1488);
            //server.Start();
            //Console.WriteLine("Waiting for users...");
            //Task.Run(() =>
            //{
            //    while(true)
            //    {
            //        string str = player1.ReadMessage();
            //        Console.WriteLine(str);
            //    }
            //});
            //while (true)
            //{
            //    player1= new Player(server.AcceptTcpClient());
            //    Console.WriteLine(server.LocalEndpoint.ToString()+ " connected to server!");
            //}
            Server server = new Server("127.0.0.1", 1488);
            server.ServerStart();
        }
    }
}