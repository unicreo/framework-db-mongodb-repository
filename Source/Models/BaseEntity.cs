using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Framework.DB.MongoDB.Repository.Models
{
    
    [BsonDiscriminator(RootClass = true)]
    public class BaseEntity : TimeStampEntity, IBaseEntity<ObjectId>
    {
        public BaseEntity() : base()
        {
            Id = ObjectId.GenerateNewId();
        }

        [BsonId]
        public ObjectId Id { get; set; }
    }

}