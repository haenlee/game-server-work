using Common;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class NetSocketListener : IDisposable
    {
        private readonly IPEndPoint _endPoint;
        private readonly Socket _serverSocket;
        private readonly SocketAsyncEventArgs _args;

        public event EventHandler<Socket>? SocketConnected;

        public NetSocketListener(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _serverSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _args = new SocketAsyncEventArgs();
            _args.Completed += OnSocketAccepted;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            // 소켓 연결
            _serverSocket.Bind(_endPoint);
            _serverSocket.Listen(Config.MAX_BACK_LOG);
            BeginAccept(_args);
        }

        public void Stop()
        {
            _serverSocket.Close();
        }

        private void BeginAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            _serverSocket.AcceptAsync(args);
        }

        private void OnSocketAccepted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                OnSocketConnected(args.AcceptSocket);
            }
            else
            { 
                Console.WriteLine(args.SocketError.ToString());
            }

            // 연결 재시작
            BeginAccept(args);
        }

        private void OnSocketConnected(Socket? socket)
        {
            if (socket == null)
                return;

            SocketConnected?.Invoke(this, socket);
        }
    }
}
