# Unicreo.Framework.Db.MongoDB.Repository
This library is representation of repository pattern in .net core for MongoDB using for internal need of company.

Based on MongoDb.Driver 2.10.2

[![NuGet](https://img.shields.io/nuget/v/Unicreo.Framework.Db.MongoDB.Repository)](https://www.nuget.org/packages/Unicreo.Framework.Db.MongoDB.Repository/)
## Install

- Package Manager:   `Install-Package Unicreo.Framework.Db.MongoDB.Repository -Version 1.0.5`
- .NET CLI: `dotnet add package Unicreo.Framework.Db.MongoDB.Repository --version 1.0.5`
- PackageReference `<PackageReference Include="Unicreo.Framework.Db.MongoDB.Repository" Version="1.0.5" />`
- Paket CLI `paket add Unicreo.Framework.Db.MongoDB.Repository --version 1.0.5`

## Configure

**Use DI for config.**

You have to implement [`ICollectionNameProvider`](https://github.com/unicreo/framework-db-mongodb-repository/blob/master/Source/Interfaces/ICollectionNameProvider.cs) interface that has
to implement `string GetCollectionName(Type entityType)` method which
allows to get name of collection in mongodb by type.

Also you have to set connection string to your mongodb.

And you should set logger;

**Example:**

```c#
services.AddTransient<ICollectionNameProvider, CollectionNameProvider>(); // You have to implement this interface by yourself   
services.AddTransient<IMongoDbContext>(sp =>
 {
     string connectionString = "Your connection string";
     ILogger<MongoDbContext> logger = sp.GetRequiredService<ILogger<MongoDbContext>>();
     ICollectionNameProvider collectionNameProvider = sp.GetRequiredService<ICollectionNameProvider>;
     return new MongoDbContext(connectionString, collectionNameProvider);
 });

services.AddTransient<IMongoDbDataRepository, MongoDbDataRepository>();
 ```

## Usage

All mongodb entities should be inherited from [BaseEntity](https://github.com/unicreo/framework-db-mongodb-repository/blob/master/Source/Models/BaseEntity.cs) that contain ObjectId as Id.

```c#
public class ToDo: BaseEntity {
    public string Name { get; set;}
    public bool Done { get; set; } = false; 
}
```

Use DI to provide MongoDataRepository

```c#
public class ToDoService {
    private readonly IMongoDbDataRepository _dataRepository;

    public ToDoService(IMongoDbDataRepository dataRepository) {
        _dataRepository = dataRepository;
    }
}
```

**Some examples of usage:**

Initialize collections:

```c#
var collectionNames = new List<string> {"ToDos", "Users"}; // better way is to get names from CollectionNameProvider
await _dataRepository.Initialize(collectionNames);

```

Create entity:

```c#
var todo = new ToDo { Name = "buy smth"};
await _dataRepository.AddAsync(todo);
```

Create list:

```c#
var todoList = new List<ToDo> {
    new ToDo {Name = "buy smth"},
    new ToDo {Name = "sell smth"}
};

await _dataRepository.AddListAsync(todoList);
```


Get entity:

```c#
string id = ObjectId.GenerateNewId().ToString();
var todo = await _dataRepository.GetDocumentAsync<ToDo>(id); //you should ensure that id is valid ObjectId
```

```c#
var todo = await _dataRepository.GetDocumentAsync<ToDo>(item => item.Id == ObjectId.Parse(id) && !item.Done);
```

Also in get methods you can use projection
```c#
var projection = Builders<ToDo>.Projection
    .Include(entity => entity.Name)
    .Exclude(entity => entity.Done);

var todo = await _dataRepository.GetDocumentAsync<ToDo>(id, projection);
```

It works for GetList methods too.

GetList:

with pagination

```c#
int skip = 10;
int take = 10;
var todoList = await _dataRepository.GetListAsync(skip, take, entry => !entry.Done);
```
without pagination

```c#
var todoList = await _dataRepository.GetListAsync(entry => !entry.Done);
```

Update:
```c#
var todo = await _dataRepository.GetDocumentAsync<ToDo>(id);
todo.Name = "New name";
await _dataRepository.UpdateAsync<ToDo>(todo);
```

Update one:

allow to update only one property

```c#
var todo = await _dataRepository.GetDocumentAsync<ToDo>(id);
await _dataRepository.UpdateOneAsync(todo, entity => entity.Name, "new name");
```

or you can use builders
```c#
var todo = await _dataRepository.GetDocumentAsync<ToDo>(id);
var update = Builders<ToDo>.Update.Set(entity => entity.Name, "new name");
await _dataRepository.UpdateOneAsync(todo, update);
```

Delete:

```c#
var todo = await _dataRepository.GetDocumentAsync<ToDo>(id);
await _dataRepository.DeleteAsync(todo);
```

### **Transactions**

You also can use transactions

Methods you can use in transactions:

- AddAsync
- AddListAsync
- UpdateAsync
- UpdateOneAsync
- DeleteAsync

**Note: you should pass session object in every method that you want to use in transaction.**

Example:

```c#
using (var session = _dataRepository.StartSession())
{
    session.StartTransaction();
    var todo1 = new ToDo {Name = "buy smth"};
    var todo2 = new ToDo {Name = "sell smth"};

    try {
        await _dataRepository.AddAsync(todo1, session);
        await _dataRepository.AddAsync(todo2, session);
        await session.CommitTransactionAsync();        
    }
    catch {
        await session.AbortTransactionAsync();
    }
}
```

### MongoDbContext
For deeper usage of database you can use MongoDbContext
that implements next interface:

```c#
    interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>();
        IMongoQueryable<T> GetQueryableCollection<T>();
        IMongoDatabase Database { get; }
        IMongoClient Client { get; }
        Task CreateCollectionsAsync(IEnumerable<string> collectionNames);
    }
``` 

Information about usage IMongoCollection, IMongoQueryable,
 IMongoDatabase, IMongoClient you can find [here](https://mongodb.github.io/mongo-csharp-driver/2.10).
 
