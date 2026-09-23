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

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _repository = new();
    private readonly IMapper _mapper;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        _service = new AppointmentService(_repository.Object, _mapper, NullLogger<AppointmentService>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Appointment> { new() { Id = 1, Name = "Visit", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Pending" } });
        var result = await _service.GetAllAsync(CancellationToken.None);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Appointment?)null);
        Assert.Null(await _service.GetByIdAsync(10, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedDto()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>())).ReturnsAsync((Appointment a, CancellationToken _) => { a.Id = 6; return a; });
        var result = await _service.CreateAsync(new AppointmentCreateDto("Visit", 1, 2, DateTime.UtcNow, "Pending", null, null, null), CancellationToken.None);
        Assert.Equal(6, result.Id);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_Rethrows()
    {
        _repository.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new AppointmentCreateDto("Visit", 1, 2, DateTime.UtcNow, "Pending", null, null, null), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityExists_UpdatesRepository()
    {
        var entity = new Appointment { Id = 1, Name = "Old", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Pending" };
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        await _service.UpdateAsync(1, new AppointmentUpdateDto("New", 3, 4, DateTime.UtcNow.AddDays(1), "Done", "Rx", "Notes", "Disease"), CancellationToken.None);
        _repository.Verify(x => x.UpdateAsync(It.Is<Appointment>(a => a.Name == "New" && a.PatientId == 3 && a.DoctorId == 4 && a.Status == "Done"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityMissing_Throws()
    {
        _repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Appointment?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, new AppointmentUpdateDto("New", 3, 4, DateTime.UtcNow, "Done", null, null, null), CancellationToken.None));
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
        _repository.Setup(x => x.SearchAsync("visit", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Appointment> { new() { Name = "Visit", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Pending" } });
        var result = await _service.SearchAsync("visit", CancellationToken.None);
        Assert.Single(result);
        Assert.Equal("Visit", result[0].Name);
    }
}
