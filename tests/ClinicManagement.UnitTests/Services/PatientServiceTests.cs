using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Mappings;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.UnitTests.Services;

/// <summary>Unit tests for PatientService</summary>
public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private readonly Mock<IBillRepository> _billRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<PatientService>> _loggerMock;
    private readonly PatientService _sut;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _billRepositoryMock = new Mock<IBillRepository>();
        _loggerMock = new Mock<ILogger<PatientService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _sut = new PatientService(
            _patientRepositoryMock.Object,
            _appointmentRepositoryMock.Object,
            _billRepositoryMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientExists_ReturnsPatientDto()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = 1,
            Name = "John Doe",
            Email = "john@test.com",
            Phone = "1234567890",
            Gender = "M",
            Address = "123 Main St",
            BirthDate = new DateTime(1990, 1, 1),
            IsActive = true
        };
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(patient);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientNotFound_ReturnsNull()
    {
        // Arrange
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Patient?)null);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPatients()
    {
        // Arrange
        var patients = new List<Patient>
        {
            new Patient { PatientId = 1, Name = "Patient 1", Email = "p1@test.com", BirthDate = DateTime.UtcNow },
            new Patient { PatientId = 2, Name = "Patient 2", Email = "p2@test.com", BirthDate = DateTime.UtcNow }
        };
        _patientRepositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(patients);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBillHistoryAsync_ReturnsBillHistory()
    {
        // Arrange
        var bills = new List<Bill>
        {
            new Bill { BillId = 1, PatientId = 1, Amount = 100, IsPaid = true, BillDate = DateTime.UtcNow }
        };
        _billRepositoryMock.Setup(r => r.GetByPatientAsync(1, default)).ReturnsAsync(bills);

        // Act
        var result = await _sut.GetBillHistoryAsync(1);

        // Assert
        result.Count.Should().Be(1);
        result.Bills.Should().HaveCount(1);
    }
}
