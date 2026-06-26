using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vitastic.Infra.Data;

namespace Vitastic.Infra;
internal class DesignTimeContextFactory : IDesignTimeDbContextFactory<ApplicationWriteDbContext>
{
    public ApplicationWriteDbContext CreateDbContext(string[] args)
    {

        var optionBuilder = new DbContextOptionsBuilder<ApplicationWriteDbContext>();
        optionBuilder.UseNpgsql($"Host=localhost;Port=5432;Database=vitastic_db;Username=vitastic;Password=vitastic");
        return new ApplicationWriteDbContext(optionBuilder.Options);
    }
}
