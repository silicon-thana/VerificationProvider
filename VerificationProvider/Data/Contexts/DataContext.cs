using Microsoft.EntityFrameworkCore;
using VerificationProvider.Data.Entities;

namespace VerificationProvider.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<VerificationRequestEntity> VerificationRequest {  get; set; } 
}
