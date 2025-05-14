
using Microsoft.EntityFrameworkCore;

namespace Zuricos.Folio.Data;
public class FolioDbContext(DbContextOptions<FolioDbContext> options) : DbContext(options)
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}