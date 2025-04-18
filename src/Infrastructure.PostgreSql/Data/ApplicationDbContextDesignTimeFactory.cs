﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.Infrastructure.PostgreSql.Data;

[ExcludeFromCodeCoverage]
internal class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName))
            .Options;

        return new ApplicationDbContext(options);
    }
}
