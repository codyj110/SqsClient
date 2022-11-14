using System.Collections.Generic;
using System.Linq;
using sqsClient.Models;

namespace sqsClient
{
    public class SqsMessageCacheService
    {
        private static SqsMessageCacheService _instance;
        private Dictionary<string, SqsClientMessage> _loadedMessages = new Dictionary<string, SqsClientMessage>();
        public string RegionParam { get; set; }
        public string AccountParam { get; set; }
        public string QueNameParam { get; set; }
        public int MaxMessagesParam { get; set; }

        public List<SqsClientMessage> SqsMessages
        {
            get
            {
                return _loadedMessages.Select(msg => msg.Value).ToList();
            }
            set
            {
                _loadedMessages.Clear();
                value.ForEach( msg => _loadedMessages.Add(msg.Id, msg));
            }
        }

        public void Reset()
        {
            _loadedMessages.Clear();
            RegionParam = default;
            AccountParam = default;
            QueNameParam = default;
            MaxMessagesParam = default;
        }

        public SqsMessageCacheService()
        {
            
        }

        public static SqsMessageCacheService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SqsMessageCacheService();
                return _instance;
            }

            return _instance;
        }

        public List<SqsClientMessage> GetMessages(List<string> messageIds)
        {
            return _loadedMessages
                .Where(msg => messageIds.Exists(msgId => msgId == msg.Key))
                .Select(m => m.Value).ToList();
        }
    }
}