namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameServer gameServer = new GameServer();
            gameServer.Init();
            gameServer.Start();
        }
    }
}