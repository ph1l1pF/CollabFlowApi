using LiteDB;

public static class UserRepository
{
    private const string DbPath = "App_Data/users.db";

    public static Task<User> FindOrCreateAsync(string appleSub)
    {
        using var db = new LiteDatabase(DbPath);
        var col = db.GetCollection<User>("users");

        var user = col.FindOne(u => u.AppleSub == appleSub);
        if (user == null)
        {
            user = new User { Id = Guid.NewGuid().ToString(), AppleSub = appleSub };
            col.Insert(user);
        }
        return Task.FromResult(user);
    }
}

public class User
{
    public string Id { get; set; }
    public string AppleSub { get; set; }
}