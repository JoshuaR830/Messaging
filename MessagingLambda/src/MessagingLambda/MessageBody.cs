using System.Collections.Generic;

namespace MessagingLambda
{
    public class MessageBody
    {
        public string Action { get; set; }
        public string Message { get; set; }
        public List<string> Users { get; set; }
    }
}