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

public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _repository = new();
    private readonly IMapper _mapper;
    private readonly DoctorService _service;

    public DoctorServiceTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _service = new DoctorService(_repository.Object, _mapper, NullLogger<DoctorService>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor> { new() { Id = 1, Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Cardio", Address = "Addr", Department = new Department { Name = "Dept" } } });

        var result = await _service.GetAllAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Doc", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Doctor?)null);

        var result = await _service.GetByIdAsync(10, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDto()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>())).ReturnsAsync((Doctor d, CancellationToken _) => { d.Id = 7; return d; });

        var result = await _service.CreateAsync(new DoctorCreateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 11m, 2, 3), CancellationToken.None);

        Assert.Equal(7, result.Id);
        Assert.Equal("Doc", result.Name);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_Rethrows()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new DoctorCreateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 11m, 2, 3), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesRepository()
    {
        var entity = new Doctor { Id = 1, Name = "Old", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Spec", Address = "Addr" };
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        await _service.UpdateAsync(1, new DoctorUpdateDto("New", "b@b.com", "2", "F", "MD", "Neuro", "New Addr", 22m, 5, 4), CancellationToken.None);

        _repository.Verify(x => x.UpdateAsync(It.Is<Doctor>(d => d.Name == "New" && d.DepartmentId == 4), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityMissing_Throws()
    {
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Doctor?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, new DoctorUpdateDto("New", "b@b.com", "2", "F", "MD", "Neuro", "New Addr", 22m, 5, 4), CancellationToken.None));
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
        _repository.Setup(x => x.SearchAsync("card", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Doctor> { new() { Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Cardio", Address = "Addr" } });

        var result = await _service.SearchAsync("card", CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Cardio", result[0].Specialization);
    }
}
