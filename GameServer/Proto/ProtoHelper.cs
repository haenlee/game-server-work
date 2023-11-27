static public class ProtoHelper
{
    static public Protocol.GameProto MoveReq(Protocol.GameReq.Types.MoveReq moveReq)
    {
        return new Protocol.GameProto
        {
            GameReq = new Protocol.GameReq
            {
                MoveReq = moveReq,
            }
        };
    }
    static public Protocol.GameProto MoveRes(Protocol.GameErrorCode errorCode, Protocol.GameRes.Types.MoveRes? moveRes = null)
    {
        return new Protocol.GameProto
        {
            GameRes = new Protocol.GameRes
            {
                ErrorCode = errorCode,
                MoveRes = moveRes ?? new Protocol.GameRes.Types.MoveRes(),
            }
        };
    }
}
