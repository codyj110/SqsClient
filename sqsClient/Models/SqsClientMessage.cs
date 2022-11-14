namespace sqsClient.Models
{
    public class SqsClientMessage
    {
        public string Id { get; set; }
        public string ReceiptHandle { get; set; }
        public string QueName { get; set; }
        public string Region { get; set; }
        public string Account { get; set; }
        public string Body { get; set; }
    }
}