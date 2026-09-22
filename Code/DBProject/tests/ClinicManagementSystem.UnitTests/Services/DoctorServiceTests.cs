using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Mappings;
using ClinicManagementSystem.Application.Services;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClinicManagementSystem.UnitTests.Services;

public class DoctorServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedDoctor()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        var repository = new Mock<IDoctorRepository>();
        repository.Setup(r => r.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>())).ReturnsAsync((Doctor d, CancellationToken _) => { d.Id = 1; return d; });
        var service = new DoctorService(repository.Object, mapper, NullLogger<DoctorService>.Instance);

        var result = await service.CreateAsync(new DoctorCreateDto("Name", "test@test.com", "123", "M", "Address", DateTime.UtcNow.Date, "MBBS", "Cardiology", 4, 1000, 100, 1));

        result.Id.Should().Be(1);
        result.Name.Should().Be("Name");
    }
}
