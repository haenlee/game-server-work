using Common;
using Network;

namespace GameServer.Service
{
    class MapService : Singleton<MapService>
    {
        public void OnMove(NetConnection connection, Protocol.GameReq.Types.MoveReq recvData)
        {
            var proto = ProtoHelper.MoveRes(Protocol.GameErrorCode.Success);
            connection.BeginSend(proto);
        }
    }
}
