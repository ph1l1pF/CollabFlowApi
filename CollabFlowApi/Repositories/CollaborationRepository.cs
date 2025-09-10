using MongoDB.Driver;

namespace CollabFlowApi.Repositories;

public class CollaborationRepository : ICollaborationRepository
{
    private readonly IMongoCollection<Collaboration> _collection;

    public CollaborationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Collaboration>("collaborations");

        // Index auf UserId + Id setzen
        var indexKeys = Builders<Collaboration>.IndexKeys
            .Ascending(c => c.Id)
            .Ascending(c => c.UserId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var model = new CreateIndexModel<Collaboration>(indexKeys, indexOptions);
        _collection.Indexes.CreateOne(model);
    }

    public Task<List<Collaboration>> GetAll(string? userId)
    {
        var filter = Builders<Collaboration>.Filter.Eq(c => c.UserId, userId);
        return _collection.Find(filter).ToListAsync();
    }

    public async Task<Collaboration?> GetById(string userId, string id)
    {
        var filter = Builders<Collaboration>.Filter.Eq(c => c.Id, id) &
                     Builders<Collaboration>.Filter.Eq(c => c.UserId, userId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdate(Collaboration collab, string userId)
    {
        collab.UserId = userId;

        var filter = Builders<Collaboration>.Filter.Eq(c => c.Id, collab.Id) &
                     Builders<Collaboration>.Filter.Eq(c => c.UserId, userId);

        await _collection.ReplaceOneAsync(
            filter,
            collab,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<bool> Delete(string id, string userId)
    {
        var filter = Builders<Collaboration>.Filter.Eq(c => c.Id, id) &
                     Builders<Collaboration>.Filter.Eq(c => c.UserId, userId);

        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}
