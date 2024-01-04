namespace Network
{
    // 캐릭터 정보 및 유저 정보
    public interface INetSession
    { 
        
    }

    public class NetSession : INetSession
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
