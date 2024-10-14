using Microsoft.EntityFrameworkCore;

namespace NGM_Server.Models;

public class MyDBContext : DbContext
{
    public DbSet<User> Users { get; set; }
}