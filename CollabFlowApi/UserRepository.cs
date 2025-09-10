using MongoDB.Driver;

public class UserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");

        // Index auf AppleSub (eindeutig, da pro Apple User nur ein Eintrag existiert)
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.AppleSub);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var model = new CreateIndexModel<User>(indexKeys, indexOptions);
        _collection.Indexes.CreateOne(model);
    }

    public async Task<User> FindOrCreateAsync(string appleSub)
    {
        var filter = Builders<User>.Filter.Eq(u => u.AppleSub, appleSub);
        var user = await _collection.Find(filter).FirstOrDefaultAsync();

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid().ToString(),
                AppleSub = appleSub
            };

            await _collection.InsertOneAsync(user);
        }

        return user;
    }
}

public class User
{
    public string Id { get; set; } = default!;
    public string AppleSub { get; set; } = default!;
}