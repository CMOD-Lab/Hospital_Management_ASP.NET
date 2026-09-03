using System;
using Xunit;
using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Entities.Tests
{
    public class AppointmentTests
    {
        [Fact]
        public void Appointment_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var appointment = new Appointment();

            // Assert
            Assert.Equal(0, appointment.AppointId);
            Assert.Equal(0, appointment.PatientId);
            Assert.Equal(0, appointment.AppointmentStatus);
        }

        [Fact]
        public void Appointment_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var date = new DateTime(2024, 6, 15);
            var appointment = new Appointment
            {
                AppointId = 1,
                DoctorId = 2,
                PatientId = 3,
                Date = date,
                AppointmentStatus = 1,
                BillAmount = 200.0,
                BillStatus = "Paid",
                DoctorNotification = 1,
                PatientNotification = 2,
                FeedbackStatus = 2,
                Disease = "Flu",
                Progress = "Recovering",
                Prescription = "Rest and fluids"
            };

            // Assert
            Assert.Equal(1, appointment.AppointId);
            Assert.Equal(2, appointment.DoctorId);
            Assert.Equal(3, appointment.PatientId);
            Assert.Equal(date, appointment.Date);
            Assert.Equal(1, appointment.AppointmentStatus);
            Assert.Equal(200.0, appointment.BillAmount);
            Assert.Equal("Paid", appointment.BillStatus);
            Assert.Equal(1, appointment.DoctorNotification);
            Assert.Equal(2, appointment.PatientNotification);
            Assert.Equal(2, appointment.FeedbackStatus);
            Assert.Equal("Flu", appointment.Disease);
            Assert.Equal("Recovering", appointment.Progress);
            Assert.Equal("Rest and fluids", appointment.Prescription);
        }

        [Fact]
        public void Appointment_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var appointment = new Appointment
            {
                DoctorId = null,
                Date = null,
                BillAmount = null,
                BillStatus = null,
                DoctorNotification = null,
                PatientNotification = null,
                FeedbackStatus = null,
                Disease = null,
                Progress = null,
                Prescription = null
            };

            // Assert
            Assert.Null(appointment.DoctorId);
            Assert.Null(appointment.Date);
            Assert.Null(appointment.BillAmount);
            Assert.Null(appointment.BillStatus);
            Assert.Null(appointment.DoctorNotification);
            Assert.Null(appointment.PatientNotification);
            Assert.Null(appointment.FeedbackStatus);
            Assert.Null(appointment.Disease);
            Assert.Null(appointment.Progress);
            Assert.Null(appointment.Prescription);
        }

        [Fact]
        public void Appointment_AppointmentStatus_Approved()
        {
            // Arrange & Act
            var appointment = new Appointment { AppointmentStatus = 1 };

            // Assert
            Assert.Equal(1, appointment.AppointmentStatus);
        }

        [Fact]
        public void Appointment_AppointmentStatus_Pending()
        {
            // Arrange & Act
            var appointment = new Appointment { AppointmentStatus = 2 };

            // Assert
            Assert.Equal(2, appointment.AppointmentStatus);
        }

        [Fact]
        public void Appointment_AppointmentStatus_Completed()
        {
            // Arrange & Act
            var appointment = new Appointment { AppointmentStatus = 3 };

            // Assert
            Assert.Equal(3, appointment.AppointmentStatus);
        }

        [Fact]
        public void Appointment_AppointmentStatus_Rejected()
        {
            // Arrange & Act
            var appointment = new Appointment { AppointmentStatus = 4 };

            // Assert
            Assert.Equal(4, appointment.AppointmentStatus);
        }

        [Fact]
        public void Appointment_NavigationProperties_CanBeSet()
        {
            // Arrange
            var doctor = new Doctor { DoctorId = 1, Name = "Dr. Smith" };
            var patient = new Patient { PatientId = 2, Name = "John Doe" };

            var appointment = new Appointment
            {
                Doctor = doctor,
                Patient = patient
            };

            // Assert
            Assert.NotNull(appointment.Doctor);
            Assert.Equal("Dr. Smith", appointment.Doctor.Name);
            Assert.NotNull(appointment.Patient);
            Assert.Equal("John Doe", appointment.Patient.Name);
        }

        [Fact]
        public void Appointment_BillStatus_CanBePaid()
        {
            // Arrange & Act
            var appointment = new Appointment { BillStatus = "Paid" };

            // Assert
            Assert.Equal("Paid", appointment.BillStatus);
        }

        [Fact]
        public void Appointment_BillStatus_CanBeUnpaid()
        {
            // Arrange & Act
            var appointment = new Appointment { BillStatus = "Unpaid" };

            // Assert
            Assert.Equal("Unpaid", appointment.BillStatus);
        }
    }
}
