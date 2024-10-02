using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Data
{
    public class RedditContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public RedditContext(DbContextOptions<RedditContext> options) : base(options)
        {
        }

    }
}

