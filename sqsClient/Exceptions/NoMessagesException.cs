using System;

namespace sqsClient.Exceptions
{
    public class NoMessagesException: Exception
    {
        public new string Message = "No Messages";
    }
}