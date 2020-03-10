using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Framework.DB.MongoDB.Repository.Models
{
    public interface ITimeStampEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class TimeStampEntity
    {
        protected TimeStampEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}