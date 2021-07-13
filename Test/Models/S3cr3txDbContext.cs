using System;
using Microsoft.EntityFrameworkCore;
using TestS3cr3txApp.Models;

namespace TestS3cr3txApp.Models
{
    public class S3cr3txDbContext : DbContext
    {
        public S3cr3txDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<S3cr3tx> s3cr3tx { get; set; }
    }
}
