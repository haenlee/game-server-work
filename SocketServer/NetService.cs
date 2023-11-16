using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class NetService
    {
        static NetSocketListener? _listener;

        public void Init(IPEndPoint endPoint)
        {
            _listener = new NetSocketListener(endPoint);
            _listener.SocketConnected += OnSocketConnected;
        }

        public void Start()
        {
            _listener?.Start();
        }

        public void Stop()
        {
            _listener?.Stop();
        }

        private void OnSocketConnected(object? sender, Socket socket)
        {
            // TODO: 소켓 연결 완료 후 세션 생성

            IPEndPoint? clientIP = socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine("Client[{0}]] Connected", clientIP);
        }
    }
}
