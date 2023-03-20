using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Azure_Lab3.Models
{
    public class QueueService
    {
        private static readonly string connString = "DefaultEndpointsProtocol=https;AccountName=daryncevqeuestorage;AccountKey=4AJwjcqbxa2L+xRKmNbwKnIpoj3QJkq4MMsXZS8GVwrF6uOgNdc0W7VvuS9KhzFZgcl3RTYrkk0j+AStCAp6jg==;EndpointSuffix=core.windows.net";
        

        private static async Task<QueueClient> GetQueueClient()
        {
            QueueServiceClient queueServiceClient = new QueueServiceClient(connString);


            QueueClient queueClient = await queueServiceClient.CreateQueueAsync("myqueue");

            return queueClient;
        }

        public static async Task SendMessageAsync(string text)
        {
            QueueClient qClient = await GetQueueClient();

            await qClient.SendMessageAsync(text);

        }


        public static async  Task<bool> DeleteMessageById(string id)
        {
            QueueClient qClient = await GetQueueClient();

            try
            {
                if (id != null)
                {
                    var arr = (await qClient.ReceiveMessagesAsync(maxMessages: 15, visibilityTimeout: TimeSpan.FromSeconds(2))).Value;
                    var message = arr.Where(a => a.MessageId == id).First();
                    await qClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);

                return false;
            }
            


        }


        


        public static async Task<List<Lot>> GetLotsByCurrency(string currency)
        {
            var list = new List<Lot>();

            QueueClient qClient = await GetQueueClient();



            Response<PeekedMessage[]> messages = await qClient.PeekMessagesAsync(10);

            foreach (PeekedMessage message in messages.Value.Reverse())
            {
                var text = message.MessageText;


                Lot? lot = JsonSerializer.Deserialize<Lot>(text);

                if (lot.Currency.Equals(currency))
                {
                    //await Console.Out.WriteLineAsync(text);
                    lot.Id = message.MessageId;
                    list.Add(lot);
                }
            }



            return list;
        }
    }
}
