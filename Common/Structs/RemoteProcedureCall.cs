namespace Common.Structs
{
    public readonly struct RemoteProcedureCall
    {
        public const string RECEIVE_USER_ACTIVITY = "rpc_receive-user-activity";

        public const string RECEIVE_MESSAGE = "rpc_receive-message";

        public const string SEND_MESSAGE = "rpc_send-message";

        public const string SEND_BROADCAST = "rpc_send-broadcast-message";

        public const string RECEIVE_BROADCAST = "rpc_receive-broadcast-message";

        public const string GET_USERS = "rpc_get-users";
    }
}
