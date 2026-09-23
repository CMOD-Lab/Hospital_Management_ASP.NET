using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Mappings;
using ClinicManagementSystem.Application.Services;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ClinicManagementSystem.Application.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _repository = new();
    private readonly IMapper _mapper;
    private readonly DepartmentService _service;

    public DepartmentServiceTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _service = new DepartmentService(_repository.Object, _mapper, NullLogger<DepartmentService>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Department> { new() { Id = 1, Name = "Dept", Description = "Desc" } });
        var result = await _service.GetAllAsync(CancellationToken.None);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Department?)null);
        Assert.Null(await _service.GetByIdAsync(10, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDto()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>())).ReturnsAsync((Department d, CancellationToken _) => { d.Id = 8; return d; });
        var result = await _service.CreateAsync(new DepartmentCreateDto("Dept", "Desc"), CancellationToken.None);
        Assert.Equal(8, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesRepository()
    {
        var entity = new Department { Id = 1, Name = "Old" };
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        await _service.UpdateAsync(1, new DepartmentUpdateDto("New", "Desc"), CancellationToken.None);
        _repository.Verify(x => x.UpdateAsync(It.Is<Department>(d => d.Name == "New" && d.Description == "Desc"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityMissing_Throws()
    {
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Department?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, new DepartmentUpdateDto("New", "Desc"), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        await _service.DeleteAsync(3, CancellationToken.None);
        _repository.Verify(x => x.DeleteAsync(3, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedResults()
    {
        _repository.Setup(x => x.SearchAsync("dep", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Department> { new() { Name = "Dept", Description = "Desc" } });
        var result = await _service.SearchAsync("dep", CancellationToken.None);
        Assert.Single(result);
        Assert.Equal("Dept", result[0].Name);
    }
}
