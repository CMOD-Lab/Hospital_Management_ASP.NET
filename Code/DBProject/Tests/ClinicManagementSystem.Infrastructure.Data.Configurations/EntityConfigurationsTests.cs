using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicManagementSystem.Infrastructure.Data.Configurations;

public class EntityConfigurationsTests
{
    [Fact]
    public void Model_UsesExpectedTableNames()
    {
        var options = new DbContextOptionsBuilder<ClinicManagementDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ClinicManagementDbContext(options);
        var model = context.Model;

        Assert.Equal("doctors", model.FindEntityType(typeof(Doctor))!.GetTableName());
        Assert.Equal("patients", model.FindEntityType(typeof(Patient))!.GetTableName());
        Assert.Equal("appointments", model.FindEntityType(typeof(Appointment))!.GetTableName());
        Assert.Equal("departments", model.FindEntityType(typeof(Department))!.GetTableName());
        Assert.Equal("bills", model.FindEntityType(typeof(Bill))!.GetTableName());
        Assert.Equal("feedback", model.FindEntityType(typeof(Feedback))!.GetTableName());
    }
}
