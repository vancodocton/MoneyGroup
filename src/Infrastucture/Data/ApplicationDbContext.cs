using Microsoft.EntityFrameworkCore;

namespace MoneyGroup.Infrastucture.Data;
public class ApplicationDbContext
    : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
}