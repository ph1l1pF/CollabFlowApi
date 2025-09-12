using Microsoft.EntityFrameworkCore;

namespace CollabFlowApi.Repositories;

public class CollaborationRepository : ICollaborationRepository
{
    private readonly AppDbContext _context;

    public CollaborationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Collaboration>> GetAll(string userId)
    {
        var query = _context.Collaborations.AsQueryable();
        query = query.Where(c => c.UserId == userId);
        return await query.ToListAsync();
    }

    public Task<Collaboration?> GetById(string userId, string id)
    {
        return _context.Collaborations
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task AddOrUpdate(Collaboration collab, string userId)
    {
        collab.UserId = userId;

        var existing = await _context.Collaborations
            .FirstOrDefaultAsync(c => c.Id == collab.Id && c.UserId == userId);

        if (existing == null)
        {
            _context.Collaborations.Add(collab);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(collab);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> Delete(string id, string userId)
    {
        var entity = await _context.Collaborations
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (entity == null) return false;

        _context.Collaborations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(string userId)
    {
        var entities = await _context.Collaborations
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!entities.Any()) return false;

        _context.Collaborations.RemoveRange(entities);
        await _context.SaveChangesAsync();
        return true;
    }
}