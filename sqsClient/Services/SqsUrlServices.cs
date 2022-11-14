namespace sqsClient
{
    public class SqsUrlServices
    {
        private string SqsQueKey = "{QueName}";
        private string accountKey = "{Account}";
        private string RegionKey = "{region}";
        private string BaseUrl { get; set; }

        public SqsUrlServices()
        {
            BaseUrl = $@"https://sqs.{RegionKey}.amazonaws.com/{accountKey}/{SqsQueKey}";
        }

        public string buildUrl(string region, string account, string queName)
        {
            return BaseUrl.Replace(RegionKey, region)
                .Replace(accountKey, account)
                .Replace(SqsQueKey, queName);
        }
    }
}