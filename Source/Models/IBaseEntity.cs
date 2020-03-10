using System;

namespace Framework.DB.MongoDB.Repository.Models
{
    public interface IBaseEntity<TKey> 
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}