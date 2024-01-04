using Common;
using System.Net;

namespace GameServer
{
    class GameServer
    {
        private Thread? _thread;
        private GameService? _gameService; 
        private bool _isRun = false;

        public void Init()
        {
            _thread = new Thread(new ThreadStart(Update));
            _gameService = new GameService();

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, Config.PORT);
            _gameService.Init(endPoint);

            Console.WriteLine("GameServer Init.");
        }

        public void Start()
        {
            _gameService?.Start();
            _thread?.Start();
            _isRun = true;

            Console.WriteLine("GameServer Start.");
        }

        public void Stop()
        {
            _isRun = false;
            _thread?.Join();
            _gameService?.Stop();
        }

        public void Update()
        {
            while (_isRun)
            { 
                Thread.Sleep(100);
            }
        }
    }
}
