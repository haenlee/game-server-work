using Network;
using System.Net;
using System.Net.Sockets;

namespace BotClient
{
    public class BotClient
    {
        private readonly IPEndPoint _endPoint;
        private readonly Socket _botSocket;

        public BotClient(IPEndPoint endPoint) 
        {
            _endPoint = endPoint;
            _botSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnSocketConnected;
            args.RemoteEndPoint = _endPoint;
            BeginConnect(args);
        }

        private void BeginConnect(SocketAsyncEventArgs args)
        {
            _botSocket.ConnectAsync(args);
        }

        private void OnSocketConnected(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                BotSession session = new BotSession();
                NetConnection connection = new NetConnection(_botSocket, session, OnSocketDisconnected, 
                    (connection, proto) => 
                    {
                        Console.WriteLine(proto.GameRes.ToString());
                    });

                Console.WriteLine("OnSocketConnected.");
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
        }

        private void OnSocketDisconnected(object? sender, NetSession e)
        {
            Console.WriteLine("OnSocketDisconnected.");
        }
    }
}
