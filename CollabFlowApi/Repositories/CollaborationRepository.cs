using LiteDB;

namespace CollabFlowApi.Repositories;

public class CollaborationRepository : ICollaborationRepository
{
    private readonly ILiteCollection<Collaboration> _collection;

    public CollaborationRepository(ILiteDatabase db)
    {
        _collection = db.GetCollection<Collaboration>("collaborations");
        _collection.EnsureIndex(x => x.Id, true);
    }

    public Task<IEnumerable<Collaboration>> GetAll(string? userId)
    {
        var items = _collection.Find(c => c.UserId == userId);
        return Task.FromResult(items);
    }

    public Task<Collaboration?> GetById(string user, string id) =>
        Task.FromResult(_collection.FindOne(c => c.Id == id && c.UserId == user));

    public Task AddOrUpdate(Collaboration collab, string userId)
    {
        collab.UserId = userId;
        _collection.Upsert(collab);
        return Task.CompletedTask;
    }

    public async Task<bool> Delete(string id, string userId)
    {
        var existing = await GetById(userId, id);
        if (existing == null) return false;
        if (existing.UserId != userId)
        {
            throw new ArgumentException("User does not belong to this Collaboration", nameof(userId));
        }
        return _collection.DeleteMany(c => c.Id == id) > 0;
    }
}