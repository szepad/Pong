namespace Pong.Net
{
    public enum PacketType
    {
        ConnectionApproved,
        ConnectionDeclined_NameIsTaken,
        Connection,

        Disconnection,

        RequestOtherPlayers,
        OtherPlayers,

        PlayerUpdate
    }
}
