using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Framework.DB.MongoDB.Repository.Models
{
    
    public class BaseEntity
    {
        public BaseEntity() : base()
        {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }
    }
}