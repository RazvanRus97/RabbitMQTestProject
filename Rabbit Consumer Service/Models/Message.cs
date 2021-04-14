using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rabbit_Consumer_Service.Models
{
    public class Message
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
