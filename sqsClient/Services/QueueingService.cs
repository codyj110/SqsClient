using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using sqsClient.Models;

namespace sqsClient
{
    public class QueueingService
    {
        public AmazonSQSClient Client { get; set; }
        public QueueingService()
        {
            Client = new AmazonSQSClient();
        }

        /// <summary>
        /// Gets all messages for given que
        /// </summary>
        /// <returns></returns>
        public async Task<List<SqsClientMessage>> GetMessages(string queUrl,int maxMessages = 0)
        {
            var retrieving = true;
            var messagesRecieved = 0;
            List<ReceiveMessageResponse> recievedMessages = new List<ReceiveMessageResponse>(); 
            while (retrieving && (maxMessages == 0 || messagesRecieved < maxMessages ))
            {
                var result = await Client.ReceiveMessageAsync(queUrl);
                // var attributesResponse = Client.GetQueueAttributesAsync(queUrl, new List<string>() { "All" });

                if (result.Messages.Count > 0)
                {
                    recievedMessages.Add(result);
                    messagesRecieved += 1;
                }
                else
                {
                    retrieving = false;
                }
            }

            var extractedMessages = ConvertToMessage(recievedMessages);
            return ConvertClientSqsMessage(extractedMessages, queUrl);
        }

        public async Task DeleteMessages(string queUrl, List<SqsClientMessage> messages)
        {
            // TODO allwo user to adjust chunksize
            var chunkSize = 10;
            var chunkedMessages = ChunkMessages(messages, chunkSize);
            // TODO test deleting one item made change to minus one to prevent out of index
            for (int i = 0; i < chunkedMessages.Count - 1; i++)
            {
                var msgs = chunkedMessages[i];
                await DeleteMessagesBatch(queUrl, msgs);
            }
        }

        private async Task DeleteMessagesBatch(string queUrl, List<SqsClientMessage> msgs)
        {
            var batchDelete = msgs.Select(msg => new DeleteMessageBatchRequestEntry()
            {
                Id = msg.Id,
                ReceiptHandle = msg.ReceiptHandle
            }).ToList();
            var response = await Client.DeleteMessageBatchAsync(queUrl, batchDelete);
            if (response.Failed.Count > 0)
            {
                throw new Exception(response.Failed.First().Message);
            }
        }

        private static List<List<SqsClientMessage>> ChunkMessages(List<SqsClientMessage> messages, int chunkSize)
        {
            var chunkedMessages = new List<List<SqsClientMessage>>();
            for (int i = 0; i < messages.Count; i += chunkSize)
            {
                chunkedMessages.Add(messages.GetRange(i, Math.Min(chunkSize, messages.Count - 1)));
            }

            return chunkedMessages;
        }


        /// <summary>
        /// Extracts the messages from each response and flattens in one list
        /// </summary>
        /// <param name="recievedMessages"></param>
        /// <returns></returns>
        private List<Message> ConvertToMessage(List<ReceiveMessageResponse> recievedMessages)
        {
            var messages = new List<Message>();
            recievedMessages.ForEach(msg => messages.AddRange(msg.Messages));
            return messages;
        }

        /// <summary>
        /// Convert to application message model
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private List<SqsClientMessage> ConvertClientSqsMessage(List<Message> messages, string queUrl)
        {
            return messages.Select(msg => new SqsClientMessage
            {
                Id = msg.MessageId,
                ReceiptHandle = msg.ReceiptHandle,
                QueName = queUrl,
                
                Body = msg.Body
            }).ToList();
        }
    }
}