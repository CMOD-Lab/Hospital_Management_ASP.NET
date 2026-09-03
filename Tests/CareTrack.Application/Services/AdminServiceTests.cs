using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using CareTrack.Application.Services;
using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Domain.Interfaces.Services;

namespace CareTrack.Application.Services.Tests
{
    public class AdminServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IStaffRepository> _mockStaffRepo;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepo;
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
        private readonly Mock<IAuthRepository> _mockAuthRepo;
        private readonly Mock<ILogger<AdminService>> _mockLogger;
        private readonly AdminService _service;

        public AdminServiceTests()
        {
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockStaffRepo = new Mock<IStaffRepository>();
            _mockDepartmentRepo = new Mock<IDepartmentRepository>();
            _mockAppointmentRepo = new Mock<IAppointmentRepository>();
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockLogger = new Mock<ILogger<AdminService>>();

            _service = new AdminService(
                _mockDoctorRepo.Object,
                _mockPatientRepo.Object,
                _mockStaffRepo.Object,
                _mockDepartmentRepo.Object,
                _mockAppointmentRepo.Object,
                _mockAuthRepo.Object,
                _mockLogger.Object);
        }

        // GetDashboardDataAsync Tests
        [Fact]
        public async Task GetDashboardDataAsync_ReturnsValidDashboardData()
        {
            // Arrange
            var doctors = new List<Doctor> { new Doctor { DoctorId = 1, Name = "Dr. Smith", DeptNo = 1 } };
            var patients = new List<Patient> { new Patient { PatientId = 1, Name = "John" } };
            var departments = new List<Department> { new Department { DeptNo = 1, DeptName = "Cardiology" } };
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, BillStatus = "Paid", BillAmount = 200.0, Patient = new Patient { Name = "John" } }
            };

            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(doctors);
            _mockPatientRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(patients);
            _mockDepartmentRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);
            _mockAppointmentRepo.Setup(r => r.GetByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetDashboardDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalDoctors);
            Assert.Equal(1, result.TotalPatients);
            Assert.Equal(200.0, result.TotalIncome);
        }

        [Fact]
        public async Task GetDashboardDataAsync_WhenRepositoryThrows_ReturnsEmptyData()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDashboardDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalDoctors);
            Assert.Equal(0, result.TotalPatients);
        }

        [Fact]
        public async Task GetDashboardDataAsync_CalculatesTotalIncome()
        {
            // Arrange
            var doctors = new List<Doctor> { new Doctor { DoctorId = 1, DeptNo = 1 } };
            var patients = new List<Patient>();
            var departments = new List<Department>();
            var appointments = new List<Appointment>
            {
                new Appointment { BillStatus = "Paid", BillAmount = 100.0 },
                new Appointment { BillStatus = "Paid", BillAmount = 150.0 },
                new Appointment { BillStatus = "Unpaid", BillAmount = 200.0 }
            };

            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(doctors);
            _mockPatientRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(patients);
            _mockDepartmentRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);
            _mockAppointmentRepo.Setup(r => r.GetByDoctorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetDashboardDataAsync();

            // Assert
            Assert.Equal(250.0, result.TotalIncome);
        }

        // GetDoctorsAsync Tests
        [Fact]
        public async Task GetDoctorsAsync_WithEmptyQuery_ReturnsAllDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, Name = "Dr. Smith" },
                new Doctor { DoctorId = 2, Name = "Dr. Jones" }
            };
            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(doctors);

            // Act
            var result = await _service.GetDoctorsAsync(string.Empty);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetDoctorsAsync_WithSearchQuery_ReturnsFilteredDoctors()
        {
            // Arrange
            var doctors = new List<Doctor> { new Doctor { DoctorId = 1, Name = "Dr. Smith" } };
            _mockDoctorRepo.Setup(r => r.SearchAsync("Smith", It.IsAny<CancellationToken>())).ReturnsAsync(doctors);

            // Act
            var result = await _service.GetDoctorsAsync("Smith");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDoctorsAsync_WithNullQuery_ReturnsAllDoctors()
        {
            // Arrange
            var doctors = new List<Doctor> { new Doctor { DoctorId = 1, Name = "Dr. Smith" } };
            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(doctors);

            // Act
            var result = await _service.GetDoctorsAsync(null!);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDoctorsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDoctorsAsync(string.Empty);

            // Assert
            Assert.Empty(result);
        }

        // GetPatientsAsync Tests
        [Fact]
        public async Task GetPatientsAsync_WithEmptyQuery_ReturnsAllPatients()
        {
            // Arrange
            var patients = new List<Patient>
            {
                new Patient { PatientId = 1, Name = "John" },
                new Patient { PatientId = 2, Name = "Jane" }
            };
            _mockPatientRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(patients);

            // Act
            var result = await _service.GetPatientsAsync(string.Empty);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPatientsAsync_WithSearchQuery_ReturnsFilteredPatients()
        {
            // Arrange
            var patients = new List<Patient> { new Patient { PatientId = 1, Name = "John" } };
            _mockPatientRepo.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>())).ReturnsAsync(patients);

            // Act
            var result = await _service.GetPatientsAsync("John");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetPatientsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockPatientRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPatientsAsync(string.Empty);

            // Assert
            Assert.Empty(result);
        }

        // GetStaffAsync Tests
        [Fact]
        public async Task GetStaffAsync_WithEmptyQuery_ReturnsAllStaff()
        {
            // Arrange
            var staff = new List<OtherStaff>
            {
                new OtherStaff { StaffId = 1, Name = "Alice" }
            };
            _mockStaffRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(staff);

            // Act
            var result = await _service.GetStaffAsync(string.Empty);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetStaffAsync_WithSearchQuery_ReturnsFilteredStaff()
        {
            // Arrange
            var staff = new List<OtherStaff> { new OtherStaff { StaffId = 1, Name = "Alice" } };
            _mockStaffRepo.Setup(r => r.SearchAsync("Alice", It.IsAny<CancellationToken>())).ReturnsAsync(staff);

            // Act
            var result = await _service.GetStaffAsync("Alice");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetStaffAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockStaffRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetStaffAsync(string.Empty);

            // Assert
            Assert.Empty(result);
        }

        // AddDoctorAsync Tests
        [Fact]
        public async Task AddDoctorAsync_WithNewEmail_ReturnsSuccess()
        {
            // Arrange
            var doctor = new Doctor { Name = "Dr. New" };
            var login = new LoginTable { LoginId = 10, Email = "new@test.com" };

            _mockAuthRepo.Setup(r => r.EmailExistsAsync("new@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.CreateLoginAsync(It.IsAny<LoginTable>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);
            _mockDoctorRepo.Setup(r => r.AddAsync(It.IsAny<Doctor>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Doctor { DoctorId = 10, Name = "Dr. New" });

            // Act
            var (success, message) = await _service.AddDoctorAsync(doctor, "new@test.com", "password");

            // Assert
            Assert.True(success);
            Assert.Contains("successfully", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddDoctorAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            var doctor = new Doctor { Name = "Dr. Existing" };
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var (success, message) = await _service.AddDoctorAsync(doctor, "existing@test.com", "password");

            // Assert
            Assert.False(success);
            Assert.Contains("already exists", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddDoctorAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            // Arrange
            var doctor = new Doctor { Name = "Dr. Error" };
            _mockAuthRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var (success, message) = await _service.AddDoctorAsync(doctor, "error@test.com", "password");

            // Assert
            Assert.False(success);
            Assert.NotEmpty(message);
        }

        // AddStaffAsync Tests
        [Fact]
        public async Task AddStaffAsync_WithValidStaff_ReturnsSuccess()
        {
            // Arrange
            var staff = new OtherStaff { Name = "New Staff" };
            _mockStaffRepo.Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OtherStaff { StaffId = 1, Name = "New Staff" });

            // Act
            var (success, message) = await _service.AddStaffAsync(staff);

            // Assert
            Assert.True(success);
            Assert.Contains("successfully", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddStaffAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            // Arrange
            var staff = new OtherStaff { Name = "Error Staff" };
            _mockStaffRepo.Setup(r => r.AddAsync(It.IsAny<OtherStaff>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var (success, message) = await _service.AddStaffAsync(staff);

            // Assert
            Assert.False(success);
            Assert.NotEmpty(message);
        }

        // DeleteDoctorAsync Tests
        [Fact]
        public async Task DeleteDoctorAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.SoftDeleteAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteDoctorAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDoctorAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.SoftDeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.DeleteDoctorAsync(1);

            // Assert
            Assert.False(result);
        }

        // DeleteStaffAsync Tests
        [Fact]
        public async Task DeleteStaffAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockStaffRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteStaffAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteStaffAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockStaffRepo.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.DeleteStaffAsync(1);

            // Assert
            Assert.False(result);
        }

        // GetDoctorByIdAsync Tests
        [Fact]
        public async Task GetDoctorByIdAsync_WithValidId_ReturnsDoctor()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, Name = "Dr. Smith" };
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(doctor);

            // Act
            var result = await _service.GetDoctorByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dr. Smith", result.Name);
        }

        [Fact]
        public async Task GetDoctorByIdAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDoctorByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetStaffByIdAsync Tests
        [Fact]
        public async Task GetStaffByIdAsync_WithValidId_ReturnsStaff()
        {
            // Arrange
            var staff = new OtherStaff { StaffId = 1, Name = "Alice" };
            _mockStaffRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(staff);

            // Act
            var result = await _service.GetStaffByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public async Task GetStaffByIdAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockStaffRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetStaffByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetPatientByIdAsync Tests
        [Fact]
        public async Task GetPatientByIdAsync_WithValidId_ReturnsPatient()
        {
            // Arrange
            var patient = new Patient { PatientId = 1, Name = "John" };
            _mockPatientRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(patient);

            // Act
            var result = await _service.GetPatientByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
        }

        [Fact]
        public async Task GetPatientByIdAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockPatientRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPatientByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetDepartmentsAsync Tests
        [Fact]
        public async Task GetDepartmentsAsync_ReturnsDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { DeptNo = 1, DeptName = "Cardiology" }
            };
            _mockDepartmentRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(departments);

            // Act
            var result = await _service.GetDepartmentsAsync();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDepartmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockDepartmentRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDepartmentsAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
