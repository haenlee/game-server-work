using Common;
using System.Net;
using System.Net.Sockets;

namespace SockerServer
{
    public class Listener
    {
        private readonly Socket _serverSocket;

        public Listener()
        {
            _serverSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Run()
        {
            // 소켓 연결
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], Config.PORT);
            
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(Config.MAX_BACK_LOG);

            // 클라이언트 연결 상태 시작
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                // 클라이언트 연결
                Socket clientSocket = _serverSocket.EndAccept(result);
                Session session = new Session(clientSocket);
                session.Start();
                session.OnConnected();

                // 클라이언트 연결 상태 재시작
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
