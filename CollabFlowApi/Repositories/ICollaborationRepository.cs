namespace CollabFlowApi.Repositories;

public interface ICollaborationRepository
{
    Task<List<Collaboration>> GetAll(string userId);
    Task<Collaboration?> GetById(string s, string id);
    Task AddOrUpdate(Collaboration collab, string userId);
    Task<bool> Delete(string id, string userId);
}