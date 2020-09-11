using System.Data;

namespace MessagingLambda
{
    public class MessageData
    {
        public string Message { get; }
        public string ConnectionId { get; }

        public MessageData(string message, string connectionId)
        {
            Message = message;
            ConnectionId = connectionId;
        }
    }
}