namespace sqsClient
{
    public class DataStoreService
    {
        private static DataStoreService _instance;

        public int MaxMessages { get; set; }
        public string Account { get; set; }
        public string Region { get; set; }
        public string QueueName { get; set; }
        
        public static DataStoreService  GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataStoreService();
                return _instance;
            }

            return _instance;
        }
        
        
    }
}