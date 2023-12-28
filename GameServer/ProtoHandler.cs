using GameServer.Service;
using Network;

namespace GameServer
{
    public class ProtoHandler
    {
        public static void HandlePacketReq(NetConnection connection, Protocol.GameProto proto)
        {
            switch (proto.GameReq.MsgCase)
            {
                case Protocol.GameReq.MsgOneofCase.MoveReq:
                    {
                        MoveReq(connection, proto.GameReq.MoveReq);
                        return;
                    }
            }
        }

        private static void MoveReq(NetConnection connection, Protocol.GameReq.Types.MoveReq recvData)
        {
            MapService.Instance.OnMove(connection, recvData);
        }
    }
}
