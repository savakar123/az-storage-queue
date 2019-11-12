using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace QueueApp
{
    class Program
    {
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=azsachin;AccountKey=R+OBvQnQoxxvxbA+qoA6q4ChPHjsux0kAfA1MPrQ1a755bVpgohBOtj3JHvXCE7CZjYj1THU6mElYds7yjlSDw==;EndpointSuffix=core.windows.net";
        // static void Main(string[] args)
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("mystoragequeue");

                    if (args.Length > 0)
                {
                    string value = String.Join(" ", args);
                    await SendMessageAsync(queue, value);
                    Console.WriteLine($"Sent: {value}");
                }
                else
                {
                    string value = await ReceiveMessageAsync(queue);
                    Console.WriteLine($"Received: {value}");
                }

                Console.Write("Press Enter...");
                Console.ReadLine();
        }
        static async Task SendMessageAsync(CloudQueue theQueue, string newMessage)
        {
            bool createdQueue = await theQueue.CreateIfNotExistsAsync();

            if (createdQueue)
            {
                Console.WriteLine("The queue was created.");
            }

            CloudQueueMessage message = new CloudQueueMessage(newMessage);
            await theQueue.AddMessageAsync(message);
        }
        
        static async Task<string> ReceiveMessageAsync(CloudQueue theQueue)
        {
            bool exists = await theQueue.ExistsAsync();

            if (exists)
            {
                CloudQueueMessage retrievedMessage = await theQueue.GetMessageAsync();

                if (retrievedMessage != null)
                {
                    string theMessage = retrievedMessage.AsString;
                    await theQueue.DeleteMessageAsync(retrievedMessage);
                    return theMessage;
                }
                else
                {
                    Console.Write("The queue is empty. Attempt to delete it? (Y/N) ");
                    string response = Console.ReadLine();

                    if (response == "Y" || response == "y")
                    {
                        await theQueue.DeleteIfExistsAsync();
                        return "The queue was deleted.";
                    }
                    else
                    {
                        return "The queue was not deleted.";
                    }
                }
            }
            else
            {
                return "The queue does not exist. Add a message to the command line to create the queue and store the message.";
            }
        }
    }
}
