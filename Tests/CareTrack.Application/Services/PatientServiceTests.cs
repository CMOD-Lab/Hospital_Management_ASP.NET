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

namespace CareTrack.Application.Services.Tests
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepo;
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockAppointmentRepo = new Mock<IAppointmentRepository>();
            _mockDepartmentRepo = new Mock<IDepartmentRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockLogger = new Mock<ILogger<PatientService>>();

            _service = new PatientService(
                _mockPatientRepo.Object,
                _mockAppointmentRepo.Object,
                _mockDepartmentRepo.Object,
                _mockDoctorRepo.Object,
                _mockLogger.Object);
        }

        // GetPatientInfoAsync Tests
        [Fact]
        public async Task GetPatientInfoAsync_WithValidId_ReturnsPatient()
        {
            // Arrange
            var patient = new Patient { PatientId = 1, Name = "John Doe" };
            _mockPatientRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            // Act
            var result = await _service.GetPatientInfoAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public async Task GetPatientInfoAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockPatientRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patient?)null);

            // Act
            var result = await _service.GetPatientInfoAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPatientInfoAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockPatientRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPatientInfoAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetBillHistoryAsync Tests
        [Fact]
        public async Task GetBillHistoryAsync_WithValidPatientId_ReturnsBills()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, BillAmount = 100.0, BillStatus = "Paid" },
                new Appointment { AppointId = 2, BillAmount = 200.0, BillStatus = "Unpaid" }
            };
            _mockAppointmentRepo.Setup(r => r.GetBillHistoryByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetBillHistoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBillHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetBillHistoryByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetBillHistoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // GetCurrentAppointmentAsync Tests
        [Fact]
        public async Task GetCurrentAppointmentAsync_WithActiveAppointment_ReturnsAppointment()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 5, PatientId = 1, AppointmentStatus = 1 };
            _mockAppointmentRepo.Setup(r => r.GetCurrentByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.GetCurrentAppointmentAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.AppointId);
        }

        [Fact]
        public async Task GetCurrentAppointmentAsync_WithNoActiveAppointment_ReturnsNull()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetCurrentByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.GetCurrentAppointmentAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentAppointmentAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetCurrentByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetCurrentAppointmentAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetTreatmentHistoryAsync Tests
        [Fact]
        public async Task GetTreatmentHistoryAsync_WithHistory_ReturnsAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, Disease = "Flu", AppointmentStatus = 3 }
            };
            _mockAppointmentRepo.Setup(r => r.GetTreatmentHistoryByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetTreatmentHistoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTreatmentHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetTreatmentHistoryByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetTreatmentHistoryAsync(1);

            // Assert
            Assert.Empty(result);
        }

        // GetDepartmentsAsync Tests
        [Fact]
        public async Task GetDepartmentsAsync_ReturnsDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { DeptNo = 1, DeptName = "Cardiology" },
                new Department { DeptNo = 2, DeptName = "Neurology" }
            };
            _mockDepartmentRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(departments);

            // Act
            var result = await _service.GetDepartmentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
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

        // GetDoctorsByDepartmentAsync Tests
        [Fact]
        public async Task GetDoctorsByDepartmentAsync_WithValidDept_ReturnsDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, Name = "Dr. Smith" }
            };
            _mockDoctorRepo.Setup(r => r.GetByDepartmentAsync("Cardiology", It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctors);

            // Act
            var result = await _service.GetDoctorsByDepartmentAsync("Cardiology");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDoctorsByDepartmentAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetByDepartmentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDoctorsByDepartmentAsync("Cardiology");

            // Assert
            Assert.Empty(result);
        }

        // GetDoctorProfileAsync Tests
        [Fact]
        public async Task GetDoctorProfileAsync_WithValidId_ReturnsDoctor()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, Name = "Dr. Smith" };
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            // Act
            var result = await _service.GetDoctorProfileAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dr. Smith", result.Name);
        }

        [Fact]
        public async Task GetDoctorProfileAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            // Act
            var result = await _service.GetDoctorProfileAsync(999);

            // Assert
            Assert.Null(result);
        }

        // GetFreeSlotsAsync Tests
        [Fact]
        public async Task GetFreeSlotsAsync_WithValidIds_ReturnsSlots()
        {
            // Arrange
            var slots = new List<Appointment>
            {
                new Appointment { AppointId = 1, Date = DateTime.Now.AddDays(1) }
            };
            _mockAppointmentRepo.Setup(r => r.GetFreeSlotsByDoctorAndPatientAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(slots);

            // Act
            var result = await _service.GetFreeSlotsAsync(1, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetFreeSlotsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetFreeSlotsByDoctorAndPatientAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetFreeSlotsAsync(1, 2);

            // Assert
            Assert.Empty(result);
        }

        // BookAppointmentAsync Tests
        [Fact]
        public async Task BookAppointmentAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment { AppointId = 1 });

            // Act
            var (success, message) = await _service.BookAppointmentAsync(1, 2, 3);

            // Assert
            Assert.True(success);
            Assert.Contains("successfully", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task BookAppointmentAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var (success, message) = await _service.BookAppointmentAsync(1, 2, 3);

            // Assert
            Assert.False(success);
            Assert.NotEmpty(message);
        }

        // GetNotificationAsync Tests
        [Fact]
        public async Task GetNotificationAsync_WithNotification_ReturnsAppointment()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, PatientNotification = 2 };
            _mockAppointmentRepo.Setup(r => r.GetNotificationByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.GetNotificationAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PatientNotification);
        }

        [Fact]
        public async Task GetNotificationAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetNotificationByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetNotificationAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetPendingFeedbackAsync Tests
        [Fact]
        public async Task GetPendingFeedbackAsync_WithPendingFeedback_ReturnsAppointment()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, FeedbackStatus = 2 };
            _mockAppointmentRepo.Setup(r => r.GetPendingFeedbackByPatientIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.GetPendingFeedbackAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.FeedbackStatus);
        }

        [Fact]
        public async Task GetPendingFeedbackAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetPendingFeedbackByPatientIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPendingFeedbackAsync(1);

            // Assert
            Assert.Null(result);
        }

        // SubmitFeedbackAsync Tests
        [Fact]
        public async Task SubmitFeedbackAsync_WithValidAppointment_ReturnsTrue()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, FeedbackStatus = 2 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SubmitFeedbackAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SubmitFeedbackAsync_WithNullAppointment_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.SubmitFeedbackAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SubmitFeedbackAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.SubmitFeedbackAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SubmitFeedbackAsync_UpdatesFeedbackStatusToGiven()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, FeedbackStatus = 2 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SubmitFeedbackAsync(1);

            // Assert
            Assert.Equal(1, appointment.FeedbackStatus);
        }
    }
}
