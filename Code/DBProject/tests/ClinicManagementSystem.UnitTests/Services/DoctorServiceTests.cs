using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Mappings;
using ClinicManagementSystem.Application.Services;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ClinicManagementSystem.UnitTests.Services;

public sealed class DoctorServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedDoctors()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = config.CreateMapper();
        var repository = new Mock<IDoctorRepository>();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Doctor> { new() { Id = 1, Name = "Dr. Test", Email = "test@example.com", PhoneNumber = "123", Gender = "M", Qualification = "MBBS", Specialization = "General", Address = "Address", DepartmentId = 1 } });
        var service = new DoctorService(repository.Object, mapper, new NullLogger<DoctorService>());

        var result = await service.GetAllAsync(CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Name.Should().Be("Dr. Test");
    }
}
