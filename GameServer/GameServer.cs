using Common;
using Network;
using System.Net;

namespace GameServer
{
    class GameServer
    {
        private Thread? _thread;
        private NetService? _netService; 
        private bool _isRun = false;

        public void Init()
        {
            _thread = new Thread(new ThreadStart(Update));
            _netService = new NetService();

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, Config.PORT);
            _netService.Init(endPoint);
        }

        public void Start()
        {
            _netService?.Start();
            _thread?.Start();
            _isRun = true;

        }

        public void Stop()
        {
            _isRun = false;
            _thread?.Join();
            _netService?.Stop();
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
