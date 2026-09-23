using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicManagementSystem.Infrastructure.Data;

public class ClinicManagementDbContextTests
{
    [Fact]
    public void Constructor_CreatesDbSets()
    {
        var options = new DbContextOptionsBuilder<ClinicManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ClinicManagementDbContext(options);

        Assert.NotNull(context.Doctors);
        Assert.NotNull(context.Patients);
        Assert.NotNull(context.Appointments);
        Assert.NotNull(context.Departments);
        Assert.NotNull(context.Bills);
        Assert.NotNull(context.FeedbackEntries);
    }
}
