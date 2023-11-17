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
            // TODO: 세션 매니저에서 관리
            NetSession session = new NetSession(socket);
            session.OnSocketDisconnected += OnSocketDisconnected;

            IPEndPoint? clientIP = socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine("Client[{0}]] Connected", clientIP);
        }

        private void OnSocketDisconnected(object? sender, NetSession e)
        {
            // TODO: 세션 매니저에서 제거
        }
    }
}
