using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Framework.DB.MongoDB.Repository.Models
{
    public interface ITimeStampEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    

}