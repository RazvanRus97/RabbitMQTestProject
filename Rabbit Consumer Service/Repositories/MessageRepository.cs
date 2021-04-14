using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Rabbit_Consumer_Service.Models;

namespace Rabbit_Consumer_Service.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _collection;
        public MessageRepository(IConfiguration configuration)
        {
            var dbClient = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var database = dbClient.GetDatabase("local");
            _collection = database.GetCollection<Message>("messages");
        }

        public void Add(Message message)
        {
            _collection.InsertOne(message);
        }

    }
}
