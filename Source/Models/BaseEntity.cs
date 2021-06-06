using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Framework.DB.MongoDB.Repository.Models
{

    [BsonDiscriminator(RootClass = true)]
    public class BaseEntity : IBaseEntity<string>
    {
        public BaseEntity() : base()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

}