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

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _repository = new();
    private readonly IMapper _mapper;
    private readonly PatientService _service;

    public PatientServiceTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _service = new PatientService(_repository.Object, _mapper, NullLogger<PatientService>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient> { new() { Id = 1, Name = "Pat", Email = "p@b.com", PhoneNumber = "1", BirthDate = DateTime.UtcNow, Gender = "F", Address = "Addr" } });
        var result = await _service.GetAllAsync(CancellationToken.None);
        Assert.Single(result);
        Assert.Equal("Pat", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Patient?)null);
        Assert.Null(await _service.GetByIdAsync(10, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDto()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>())).ReturnsAsync((Patient p, CancellationToken _) => { p.Id = 5; return p; });
        var result = await _service.CreateAsync(new PatientCreateDto("Pat", "p@b.com", "1", DateTime.UtcNow, "F", "Addr"), CancellationToken.None);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_Rethrows()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new PatientCreateDto("Pat", "p@b.com", "1", DateTime.UtcNow, "F", "Addr"), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesRepository()
    {
        var entity = new Patient { Id = 1, Name = "Old", Email = "old@b.com", PhoneNumber = "1", BirthDate = DateTime.UtcNow, Gender = "M", Address = "Addr" };
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        await _service.UpdateAsync(1, new PatientUpdateDto("New", "new@b.com", "2", DateTime.UtcNow.Date, "F", "Road"), CancellationToken.None);
        _repository.Verify(x => x.UpdateAsync(It.Is<Patient>(p => p.Name == "New" && p.Email == "new@b.com"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityMissing_Throws()
    {
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Patient?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, new PatientUpdateDto("New", "new@b.com", "2", DateTime.UtcNow.Date, "F", "Road"), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        await _service.DeleteAsync(2, CancellationToken.None);
        _repository.Verify(x => x.DeleteAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedResults()
    {
        _repository.Setup(x => x.SearchAsync("pat", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Patient> { new() { Name = "Pat", Email = "p@b.com", PhoneNumber = "1", BirthDate = DateTime.UtcNow, Gender = "F", Address = "Addr" } });
        var result = await _service.SearchAsync("pat", CancellationToken.None);
        Assert.Single(result);
        Assert.Equal("Pat", result[0].Name);
    }
}
