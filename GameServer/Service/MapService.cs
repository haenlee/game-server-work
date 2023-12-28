using Common;
using Network;

namespace GameServer.Service
{
    class MapService : Singleton<MapService>
    {
        public void OnMove(NetConnection connection, Protocol.GameReq.Types.MoveReq recvData)
        {
            //connection.BeginSend();
        }
    }
}
