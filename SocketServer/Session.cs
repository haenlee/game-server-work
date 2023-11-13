using Common;
using System.Net.Sockets;
using System.Text;

namespace SockerServer
{
    public class Session
    {
        public Guid Id { get; init; }
        public Socket Socket { get; init; }
        public byte[] RecvBuffer { get; init; }
        public int BufferSize { get { return RecvBuffer.Length; } }
        
        public Session(Socket socket) 
        {
            Id = Guid.NewGuid();
            Socket = socket;
            RecvBuffer = new byte[Config.BUFFER_SIZE];
        }

        public void Start()
        {
            Socket.BeginReceive(RecvBuffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), this);
        }

        public void OnConnected()
        {
            Console.WriteLine("OnConnected");
        }

        public void Send()
        {
            var data = Encoding.UTF8.GetBytes("Send Test!!");
            Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), this);
        }

        #region Callback
        private void ReceiveCallback(IAsyncResult result)
        {
            int recvBytes = Socket.EndReceive(result);
            string response = Encoding.UTF8.GetString(RecvBuffer, 0, recvBytes);
            Console.WriteLine(response);

            // Recv 연결 재시작
            Array.Clear(RecvBuffer);
            Socket.BeginReceive(RecvBuffer, 0, BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), this);
        }

        private void SendCallback(IAsyncResult result)
        {
            Socket.EndSend(result);
        }
        #endregion
    }
}
