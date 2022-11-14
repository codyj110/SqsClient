using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using sqsClient.Models;

namespace sqsClient
{
    public class DataControllerService
    {
        private static DataControllerService _instance;
        private DataStoreService _dataStoreService = new DataStoreService();
        private readonly SqsUrlServices _sqsUrlServices;
        private readonly SqsMessageCacheService _sqsMessageCacheService;
        private readonly QueueingService _queueingService;

        public delegate void MessagesUpdatedHandler(object sender, List<SqsClientMessage> messages);
        public delegate void RetrievedMessagesHandler(object sender, List<SqsClientMessage> messages);
        public delegate void MessagesDeletedHandler(object sender);
        public delegate void CallingApiHandler(object sender);
        public delegate void EntryChangeHandler(object sender, string name, bool valid, string value);

        public event MessagesUpdatedHandler MessagesUpdatedEvent;
        public event RetrievedMessagesHandler RetrievedMessagesEvent;
        public event MessagesDeletedHandler MessagesDeletedEvent;
        public event CallingApiHandler CallingApiEvent;
        public event EntryChangeHandler EntryChangeEvent;
        public DataControllerService()
        {
            _sqsUrlServices = new SqsUrlServices();
            _queueingService = new QueueingService();
            _sqsMessageCacheService = new SqsMessageCacheService();

            EntryChangeEvent += EntryChangeEventHandler;
        }
        
        public static DataControllerService  GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataControllerService();
                return _instance;
            }

            return _instance;
        }
        
        public async Task GetMessages()
        {
            CallingApiEvent?.Invoke(this);
            var url = _sqsUrlServices.buildUrl(_dataStoreService.Region, _dataStoreService.Account, _dataStoreService.QueueName);
            var prevUrl = _sqsUrlServices.buildUrl(_sqsMessageCacheService.RegionParam,
                _sqsMessageCacheService.AccountParam, _sqsMessageCacheService.QueNameParam);
            if (_sqsMessageCacheService.SqsMessages.Count == 0 || prevUrl != url)
            {
                var messages = await _queueingService.GetMessages(url, _dataStoreService.MaxMessages);
                
                UpdateAccountAndRegion(_dataStoreService.Region, _dataStoreService.Account, messages);
                UpdateMessageCache(_dataStoreService.Region, _dataStoreService.Account, _dataStoreService.QueueName, _dataStoreService.MaxMessages, messages);
                var deserializedMessages = JsonSerializer.Deserialize<List<SqsClientMessage>>(JsonSerializer.Serialize(messages));
                RetrievedMessagesEvent?.Invoke(this, deserializedMessages); 
            }
            else
            {
                RetrievedMessagesEvent?.Invoke(this, _sqsMessageCacheService.SqsMessages);
            }
        }

        private static void UpdateAccountAndRegion(string region, string account, List<SqsClientMessage> messages)
        {
            messages.ForEach(msg =>
            {
                msg.Account = account;
                msg.Region = region;
            });
        }

        public List<SqsClientMessage> GetCachedMessages()
        {
            return JsonSerializer.Deserialize<List<SqsClientMessage>>(JsonSerializer.Serialize(_sqsMessageCacheService.SqsMessages));
        }

        public void PushMessages(List<SqsClientMessage> messages)
        {
            CallingApiEvent?.Invoke(this);
            UpdateMessageCache( messages[0].Region,  messages[0].Account, messages[0].QueName, default, messages);
            MessagesUpdatedEvent?.Invoke(this, messages);
        }

        public async Task DeleteMessages()
        {
            CallingApiEvent?.Invoke(this);
            var url = _sqsUrlServices.buildUrl(_sqsMessageCacheService.RegionParam,
                _sqsMessageCacheService.AccountParam, _sqsMessageCacheService.QueNameParam);
            await _queueingService.DeleteMessages(url, _sqsMessageCacheService.SqsMessages);
            _sqsMessageCacheService.Reset();
            MessagesDeletedEvent?.Invoke(this);
        }

        public void PushEntryChange(string name, bool valid, string value)
        {
            EntryChangeEvent?.Invoke(this, name, valid,value);
        }

        private void EntryChangeEventHandler(object sender, string name, bool valid, string value)
        {
            if (name.ToLower() == "region")
            {
                if (valid)
                {
                    _dataStoreService.Region = value;
                }
            }
            
            if (name.ToLower() == "account")
            {
                if (valid)
                {
                    _dataStoreService.Account = value;
                };
            }
            
            if (name.ToLower() == "queuename")
            {
                if (valid)
                {
                    _dataStoreService.QueueName = value;
                }
            }

            if (name.ToLower() == "maxmessages")
            {
                if (valid)
                {
                    _dataStoreService.MaxMessages = Convert.ToInt32(value);
                }
            }
        }

        private void UpdateMessageCache(string region, string account, string queName, int maxMessages, List<SqsClientMessage> messages)
        {
            _sqsMessageCacheService.RegionParam = region;
            _sqsMessageCacheService.AccountParam = account;
            _sqsMessageCacheService.QueNameParam = queName;
            _sqsMessageCacheService.MaxMessagesParam = maxMessages;
            _sqsMessageCacheService.SqsMessages = messages;
        }
    }
}