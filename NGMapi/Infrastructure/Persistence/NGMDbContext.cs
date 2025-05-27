using Microsoft.EntityFrameworkCore;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;

namespace NGMapi.Infrastructure.Persistence;

public class NGMDbContext : DbContext
{
    public NGMDbContext(DbContextOptions<NGMDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> users { get; set; }
    public DbSet<Server> servers { get; set; }
    public DbSet<Chat> chats { get; set; }
    public DbSet<Role> roles { get; set; }
    public DbSet<Friendship> friendships { get; set; }
    public DbSet<Member> members { get; set; }
    public DbSet<Message> messages { get; set; }
}