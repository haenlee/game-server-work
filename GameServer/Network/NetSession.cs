using System.Net.Sockets;

namespace Network
{
    public class NetSession
    {
        public Guid UserId { get; init; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;


        public NetSession()
        {
            UserId = Guid.NewGuid();
        }
    }
}
