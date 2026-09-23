using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Domain.Entities;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity
    {
    }

    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var before = DateTime.UtcNow.AddSeconds(-5);

        var entity = new TestEntity();

        var after = DateTime.UtcNow.AddSeconds(5);
        Assert.Equal(0, entity.Id);
        Assert.True(entity.IsActive);
        Assert.Equal("system", entity.CreatedBy);
        Assert.Null(entity.ModifiedBy);
        Assert.Null(entity.ModifiedDate);
        Assert.InRange(entity.CreatedDate, before, after);
    }

    [Fact]
    public void Properties_CanBeUpdated()
    {
        var entity = new TestEntity();
        var modifiedDate = DateTime.UtcNow;

        entity.Id = 42;
        entity.IsActive = false;
        entity.CreatedBy = "tester";
        entity.ModifiedBy = "editor";
        entity.ModifiedDate = modifiedDate;

        Assert.Equal(42, entity.Id);
        Assert.False(entity.IsActive);
        Assert.Equal("tester", entity.CreatedBy);
        Assert.Equal("editor", entity.ModifiedBy);
        Assert.Equal(modifiedDate, entity.ModifiedDate);
    }
}
