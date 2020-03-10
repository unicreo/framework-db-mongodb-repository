using System;

namespace Framework.DB.MongoDB.Repository
{
    public interface ICollectionNameProvider
    {
        public string GetCollectionName(Type entityType);
    }
}