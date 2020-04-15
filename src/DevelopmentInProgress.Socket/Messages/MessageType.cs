namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// Indicates the type of message being sent.
    /// </summary>
    public enum MessageType
    {
        Disconnect,
        SendToAll,
        SendToChannel,
        SendToClient,
        ServerToClient,
        SubscribeToChannel,
        UnsubscribeFromChannel        
    }
}