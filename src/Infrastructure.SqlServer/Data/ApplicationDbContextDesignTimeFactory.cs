using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.Infrastructure.SqlServer.Data;

[ExcludeFromCodeCoverage]
internal class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
            .Options;

        return new ApplicationDbContext(options);
    }
}
