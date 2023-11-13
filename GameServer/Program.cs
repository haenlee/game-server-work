using SockerServer;

namespace GameServer
{
    public class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            _listener.Run();

            while (true)
            { 
            
            }
        }
    }
}