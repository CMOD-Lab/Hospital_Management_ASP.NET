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
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
        private readonly Mock<ILogger<DoctorService>> _mockLogger;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockAppointmentRepo = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<DoctorService>>();

            _service = new DoctorService(
                _mockDoctorRepo.Object,
                _mockAppointmentRepo.Object,
                _mockLogger.Object);
        }

        // GetDoctorInfoAsync Tests
        [Fact]
        public async Task GetDoctorInfoAsync_WithValidId_ReturnsDoctor()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, Name = "Dr. Smith" };
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(doctor);

            // Act
            var result = await _service.GetDoctorInfoAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dr. Smith", result.Name);
        }

        [Fact]
        public async Task GetDoctorInfoAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Doctor?)null);

            // Act
            var result = await _service.GetDoctorInfoAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetDoctorInfoAsync_WhenRepositoryThrows_ReturnsNull()
        {
            // Arrange
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetDoctorInfoAsync(1);

            // Assert
            Assert.Null(result);
        }

        // GetPendingAppointmentsAsync Tests
        [Fact]
        public async Task GetPendingAppointmentsAsync_WithPendingAppointments_ReturnsAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, AppointmentStatus = 2 },
                new Appointment { AppointId = 2, AppointmentStatus = 2 }
            };
            _mockAppointmentRepo.Setup(r => r.GetPendingByDoctorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetPendingAppointmentsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPendingAppointmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetPendingByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPendingAppointmentsAsync(1);

            // Assert
            Assert.Empty(result);
        }

        // GetTodaysAppointmentsAsync Tests
        [Fact]
        public async Task GetTodaysAppointmentsAsync_WithTodaysAppointments_ReturnsAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, Date = DateTime.Today }
            };
            _mockAppointmentRepo.Setup(r => r.GetTodaysByDoctorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetTodaysAppointmentsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodaysAppointmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetTodaysByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetTodaysAppointmentsAsync(1);

            // Assert
            Assert.Empty(result);
        }

        // ApproveAppointmentAsync Tests
        [Fact]
        public async Task ApproveAppointmentAsync_WithValidAppointment_ReturnsTrue()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ApproveAppointmentAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ApproveAppointmentAsync_WithNullAppointment_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.ApproveAppointmentAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ApproveAppointmentAsync_SetsStatusToApproved()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, AppointmentStatus = 2 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ApproveAppointmentAsync(1);

            // Assert
            Assert.Equal(1, appointment.AppointmentStatus);
            Assert.Equal(2, appointment.PatientNotification);
        }

        [Fact]
        public async Task ApproveAppointmentAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.ApproveAppointmentAsync(1);

            // Assert
            Assert.False(result);
        }

        // RejectAppointmentAsync Tests
        [Fact]
        public async Task RejectAppointmentAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RejectAppointmentAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RejectAppointmentAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.RejectAppointmentAsync(1);

            // Assert
            Assert.False(result);
        }

        // UpdatePrescriptionAsync Tests
        [Fact]
        public async Task UpdatePrescriptionAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdatePrescriptionAsync(1, 1, "Flu", "Recovering", "Rest");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WithNullAppointment_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.UpdatePrescriptionAsync(1, 999, "Flu", "Recovering", "Rest");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WithWrongDoctorId_ReturnsFalse()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 2 }; // Different doctor
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.UpdatePrescriptionAsync(1, 1, "Flu", "Recovering", "Rest");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_UpdatesAppointmentFields()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdatePrescriptionAsync(1, 1, "Flu", "Recovering", "Rest and fluids");

            // Assert
            Assert.Equal("Flu", appointment.Disease);
            Assert.Equal("Recovering", appointment.Progress);
            Assert.Equal("Rest and fluids", appointment.Prescription);
            Assert.Equal(3, appointment.AppointmentStatus);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_WhenRepositoryThrows_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.UpdatePrescriptionAsync(1, 1, "Flu", "Recovering", "Rest");

            // Assert
            Assert.False(result);
        }

        // GetBillableAppointmentsAsync Tests
        [Fact]
        public async Task GetBillableAppointmentsAsync_WithBillableAppointments_ReturnsAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, BillAmount = 150.0 }
            };
            _mockAppointmentRepo.Setup(r => r.GetBillableByDoctorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetBillableAppointmentsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetBillableAppointmentsAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetBillableByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetBillableAppointmentsAsync(1);

            // Assert
            Assert.Empty(result);
        }

        // MarkBillPaidAsync Tests
        [Fact]
        public async Task MarkBillPaidAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.MarkBillPaidAsync(1, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MarkBillPaidAsync_WithNullAppointment_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.MarkBillPaidAsync(1, 999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task MarkBillPaidAsync_WithWrongDoctorId_ReturnsFalse()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 2 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.MarkBillPaidAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task MarkBillPaidAsync_SetsBillStatusToPaid()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkBillPaidAsync(1, 1);

            // Assert
            Assert.Equal("Paid", appointment.BillStatus);
            Assert.Equal(3, appointment.AppointmentStatus);
            Assert.Equal(2, appointment.FeedbackStatus);
        }

        // MarkBillUnpaidAsync Tests
        [Fact]
        public async Task MarkBillUnpaidAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.MarkBillUnpaidAsync(1, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MarkBillUnpaidAsync_WithNullAppointment_ReturnsFalse()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.MarkBillUnpaidAsync(1, 999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task MarkBillUnpaidAsync_SetsBillStatusToUnpaid()
        {
            // Arrange
            var appointment = new Appointment { AppointId = 1, DoctorId = 1 };
            _mockAppointmentRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointment);
            _mockAppointmentRepo.Setup(r => r.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkBillUnpaidAsync(1, 1);

            // Assert
            Assert.Equal("Unpaid", appointment.BillStatus);
            Assert.Equal(3, appointment.AppointmentStatus);
        }

        // GetPatientHistoryAsync Tests
        [Fact]
        public async Task GetPatientHistoryAsync_WithHistory_ReturnsAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1, AppointmentStatus = 3 }
            };
            _mockAppointmentRepo.Setup(r => r.GetByDoctorIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(appointments);

            // Act
            var result = await _service.GetPatientHistoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetPatientHistoryAsync_WhenRepositoryThrows_ReturnsEmpty()
        {
            // Arrange
            _mockAppointmentRepo.Setup(r => r.GetByDoctorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.GetPatientHistoryAsync(1);

            // Assert
            Assert.Empty(result);
        }
    }
}
