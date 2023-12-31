﻿using Common;
using Google.Protobuf;
using Protocol;
using System.Net.Sockets;

namespace Network
{
    public class NetConnection
    {
        public INetSession Session;

        private event EventHandler<NetSession> _onSocketDisconnected;
        private Action<NetConnection, GameProto> _onHandleProto;

        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _sendArgs;
        private readonly SocketAsyncEventArgs _recvArgs;

        private readonly object _lock = new();

        public NetConnection(Socket socket, INetSession session, EventHandler<NetSession> onSocketDisconnected, Action<NetConnection, GameProto> onHandleProto)
        {
            Session = session;

            _onSocketDisconnected += onSocketDisconnected;
            _onHandleProto += onHandleProto;

            _socket = socket;

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += OnSendCompleted;

            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.Completed += OnReceiveCompleted;
            _recvArgs.AcceptSocket = socket;
            _recvArgs.SetBuffer(new byte[Config.BUFFER_SIZE], 0, Config.BUFFER_SIZE);

            BeginReceive();
        }

        #region Send
        public void BeginSend(GameProto proto)
        {
            BeginSend(proto.ToByteArray());
        }

        public void BeginSend(byte[] data)
        {
            lock (_lock)
            {
                if (_socket.Connected)
                {
                    _sendArgs.SetBuffer(data);
                    _socket.SendAsync(_sendArgs);
                }
            }
        }

        private void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred == 0 || args.SocketError != SocketError.Success)
            {
                Disconnect();
                return;
            }

            Console.WriteLine($"OnSendCompleted: {args.BytesTransferred}");
        }
        #endregion

        #region Receive
        private void BeginReceive()
        {
            lock (_lock)
            {
                if (_socket.Connected)
                {
                    _recvArgs.AcceptSocket?.ReceiveAsync(_recvArgs);
                }
            }
        }

        private void OnReceiveCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred == 0 || args.SocketError != SocketError.Success)
            {
                Disconnect();
                return;
            }

            if (args.Buffer != null)
            {
                byte[] data = new byte[args.BytesTransferred];
                Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);

                ReceiveData(data, 0, data.Length);
            }

            BeginReceive();
        }

        private void ReceiveData(byte[] data, int offset, int count)
        {
            MemoryStream stream = new(Config.BUFFER_SIZE);
            int readOffset = 0;

            // stream 오버플로우 체크
            if (stream.Position + count > stream.Capacity)
                return;

            // write 후, stream 포지션은 데이터 사이즈만큼 이동
            stream.Write(data, offset, count);

            while (true)
            {
                // 패킷 파싱 가능 체크
                if (stream.Position < readOffset + Config.HEADER_SIZE)
                    break;

                // 패킷 사이즈만큼 도착했는지 체크
                int dataSize = BitConverter.ToInt32(stream.GetBuffer(), readOffset);
                if (stream.Position < dataSize)
                    break;

                // 패킷 메시지 파싱
                GameProto proto = GameProto.Parser.ParseFrom(data, readOffset, dataSize);
                _onHandleProto?.Invoke(this, proto);


                readOffset += (dataSize + Config.HEADER_SIZE);

                long size = stream.Position - readOffset;
                if (readOffset < stream.Position)
                {
                    Array.Copy(stream.GetBuffer(), readOffset, stream.GetBuffer(), 0, stream.Position - readOffset);
                }

                readOffset = 0;
                stream.Position = size;
                stream.SetLength(size);
            }
        }
        #endregion

        #region Disconnect
        public void Disconnect()
        {
            lock (_lock)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();

                _sendArgs.Completed -= OnSendCompleted;
                _recvArgs.Completed -= OnReceiveCompleted;
            }
        }
        #endregion
    }
}
