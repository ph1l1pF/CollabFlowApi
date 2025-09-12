using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CollabFlowApi.Repositories;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> FindOrCreateAsync(string appleSub)
    {
        // Try to find existing user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.AppleSub == appleSub);

        if (user != null)
            return user;

        // Create new user
        user = new User
        {
            Id = Guid.NewGuid().ToString(),
            AppleSub = appleSub
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}


[Table("users")]
public class User
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string AppleSub { get; set; } = default!;
}