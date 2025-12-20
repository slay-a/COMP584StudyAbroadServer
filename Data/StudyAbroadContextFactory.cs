using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace COMP584StudyAbroadServer.Data;

public class StudyAbroadContextFactory : IDesignTimeDbContextFactory<StudyAbroadContext>
{
    public StudyAbroadContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StudyAbroadContext>();

        var connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=StudyAbroadDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        optionsBuilder.UseSqlServer(connectionString);

        return new StudyAbroadContext(optionsBuilder.Options);
    }
}
