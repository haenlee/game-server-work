﻿using Common;
using System.Net.Sockets;

namespace Network
{
    public class NetSession
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _recvArgs;

        public NetSession(Socket socket)
        { 
            _socket = socket;

            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceivedCompleted);
            _recvArgs.AcceptSocket = socket;
            _recvArgs.Completed += ReceivedCompleted;
            _recvArgs.SetBuffer(new byte[Config.BUFFER_SIZE], 0, Config.BUFFER_SIZE);

            BeginReceive(_recvArgs);
        }

        private void BeginReceive(SocketAsyncEventArgs args)
        {
            if (_socket.Connected)
            {
                args?.AcceptSocket?.ReceiveAsync(args);
            }
        }

        private void ReceivedCompleted(Object? sender, SocketAsyncEventArgs args)
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

            BeginReceive(args);
        }

        private void ReceiveData(byte[] data, int offset, int count)
        {
            MemoryStream stream = new MemoryStream(Config.BUFFER_SIZE);
            int readOffset = 0;

            // stream 오버플로우 체크
            if (stream.Position + count > stream.Capacity)
                return;

            // write 후, stream 포지션은 데이터 사이즈만큼 이동
            stream.Write(data, offset, data.Length);

            while (true)
            {
                // 패킷 파싱 가능 체크
                if (stream.Position < readOffset + Config.HEADER_SIZE)
                    break;

                // 패킷 사이즈만큼 도착했는지 체크
                int dataSize = BitConverter.ToInt32(stream.GetBuffer(), readOffset);
                if (stream.Position < dataSize)
                    break;

                // TODO: 패킷 메시지 파싱

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

        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            _recvArgs.Completed -= ReceivedCompleted;
        }
    }
}