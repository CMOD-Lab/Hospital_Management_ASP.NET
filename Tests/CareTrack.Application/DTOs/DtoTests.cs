using System;
using System.Collections.Generic;
using Xunit;
using CareTrack.Application.DTOs;

namespace CareTrack.Application.DTOs.Tests
{
    public class PatientDtoTests
    {
        [Fact]
        public void PatientDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new PatientDto();

            // Assert
            Assert.Equal(0, dto.PatientId);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.BirthDate);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(0, dto.Age);
        }

        [Fact]
        public void PatientDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new PatientDto
            {
                PatientId = 1,
                Name = "John Doe",
                Phone = "555-1234",
                Address = "123 Main St",
                BirthDate = "1990-01-15",
                Age = 34,
                Gender = "M"
            };

            // Assert
            Assert.Equal(1, dto.PatientId);
            Assert.Equal("John Doe", dto.Name);
            Assert.Equal("555-1234", dto.Phone);
            Assert.Equal("123 Main St", dto.Address);
            Assert.Equal("1990-01-15", dto.BirthDate);
            Assert.Equal(34, dto.Age);
            Assert.Equal("M", dto.Gender);
        }

        [Fact]
        public void PatientCreateDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new PatientCreateDto();

            // Assert
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.BirthDate);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Equal(string.Empty, dto.Phone);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(string.Empty, dto.Address);
        }

        [Fact]
        public void PatientCreateDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new PatientCreateDto
            {
                Name = "Jane Smith",
                BirthDate = "1995-06-20",
                Email = "jane@test.com",
                Password = "securepass",
                Phone = "555-9876",
                Gender = "F",
                Address = "456 Oak Ave"
            };

            // Assert
            Assert.Equal("Jane Smith", dto.Name);
            Assert.Equal("1995-06-20", dto.BirthDate);
            Assert.Equal("jane@test.com", dto.Email);
            Assert.Equal("securepass", dto.Password);
            Assert.Equal("555-9876", dto.Phone);
            Assert.Equal("F", dto.Gender);
            Assert.Equal("456 Oak Ave", dto.Address);
        }
    }

    public class AppointmentDtoTests
    {
        [Fact]
        public void AppointmentDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new AppointmentDto();

            // Assert
            Assert.Equal(0, dto.AppointId);
            Assert.Equal(string.Empty, dto.DoctorName);
            Assert.Equal(string.Empty, dto.PatientName);
            Assert.Equal(string.Empty, dto.Timings);
            Assert.Equal(string.Empty, dto.AppointmentStatusText);
        }

        [Fact]
        public void AppointmentDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15);

            // Act
            var dto = new AppointmentDto
            {
                AppointId = 1,
                DoctorId = 2,
                DoctorName = "Dr. Smith",
                PatientId = 3,
                PatientName = "John Doe",
                Date = date,
                Timings = "10:00 AM",
                AppointmentStatus = 1,
                AppointmentStatusText = "Approved",
                BillAmount = 150.0,
                BillStatus = "Paid",
                Disease = "Flu",
                Progress = "Recovering",
                Prescription = "Rest",
                FeedbackStatus = 1,
                PatientNotification = 2
            };

            // Assert
            Assert.Equal(1, dto.AppointId);
            Assert.Equal(2, dto.DoctorId);
            Assert.Equal("Dr. Smith", dto.DoctorName);
            Assert.Equal(3, dto.PatientId);
            Assert.Equal("John Doe", dto.PatientName);
            Assert.Equal(date, dto.Date);
            Assert.Equal("10:00 AM", dto.Timings);
            Assert.Equal(1, dto.AppointmentStatus);
            Assert.Equal("Approved", dto.AppointmentStatusText);
            Assert.Equal(150.0, dto.BillAmount);
            Assert.Equal("Paid", dto.BillStatus);
            Assert.Equal("Flu", dto.Disease);
            Assert.Equal("Recovering", dto.Progress);
            Assert.Equal("Rest", dto.Prescription);
            Assert.Equal(1, dto.FeedbackStatus);
            Assert.Equal(2, dto.PatientNotification);
        }

        [Fact]
        public void AppointmentCreateDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new AppointmentCreateDto
            {
                DoctorId = 1,
                PatientId = 2,
                FreeSlotId = 3
            };

            // Assert
            Assert.Equal(1, dto.DoctorId);
            Assert.Equal(2, dto.PatientId);
            Assert.Equal(3, dto.FreeSlotId);
        }

        [Fact]
        public void PrescriptionUpdateDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new PrescriptionUpdateDto
            {
                DoctorId = 1,
                AppointmentId = 5,
                Disease = "Hypertension",
                Progress = "Stable",
                Prescription = "Medication A"
            };

            // Assert
            Assert.Equal(1, dto.DoctorId);
            Assert.Equal(5, dto.AppointmentId);
            Assert.Equal("Hypertension", dto.Disease);
            Assert.Equal("Stable", dto.Progress);
            Assert.Equal("Medication A", dto.Prescription);
        }
    }

    public class DoctorDtoTests
    {
        [Fact]
        public void DoctorDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new DoctorDto();

            // Assert
            Assert.Equal(0, dto.DoctorId);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Gender);
            Assert.Equal(string.Empty, dto.DepartmentName);
            Assert.Equal(string.Empty, dto.Qualification);
        }

        [Fact]
        public void DoctorDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new DoctorDto
            {
                DoctorId = 1,
                Name = "Dr. Smith",
                Phone = "555-1234",
                Address = "123 Main St",
                Gender = "M",
                DepartmentName = "Cardiology",
                ChargesPerVisit = 200.0,
                ReputeIndex = 4.8,
                PatientsTreated = 500,
                Qualification = "MBBS, MD",
                Specialization = "Cardiology",
                WorkExperience = 15,
                Age = 45,
                Status = 1
            };

            // Assert
            Assert.Equal(1, dto.DoctorId);
            Assert.Equal("Dr. Smith", dto.Name);
            Assert.Equal("M", dto.Gender);
            Assert.Equal("Cardiology", dto.DepartmentName);
            Assert.Equal(200.0, dto.ChargesPerVisit);
            Assert.Equal(4.8, dto.ReputeIndex);
            Assert.Equal(500, dto.PatientsTreated);
            Assert.Equal("MBBS, MD", dto.Qualification);
            Assert.Equal("Cardiology", dto.Specialization);
            Assert.Equal(15, dto.WorkExperience);
            Assert.Equal(45, dto.Age);
            Assert.Equal(1, dto.Status);
        }

        [Fact]
        public void DoctorCreateDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new DoctorCreateDto
            {
                Name = "Dr. New",
                Email = "new@test.com",
                Password = "pass",
                BirthDate = "1980-05-10",
                DeptNo = 2,
                Phone = "555-0000",
                Gender = "M",
                Address = "789 Pine Rd",
                WorkExperience = 10,
                MonthlySalary = 6000.0,
                ChargesPerVisit = 250.0,
                Specialization = "Neurology",
                Qualification = "MBBS"
            };

            // Assert
            Assert.Equal("Dr. New", dto.Name);
            Assert.Equal("new@test.com", dto.Email);
            Assert.Equal(2, dto.DeptNo);
            Assert.Equal(10, dto.WorkExperience);
            Assert.Equal(6000.0, dto.MonthlySalary);
            Assert.Equal(250.0, dto.ChargesPerVisit);
        }
    }

    public class StaffDtoTests
    {
        [Fact]
        public void StaffDto_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new StaffDto();

            // Assert
            Assert.Equal(0, dto.StaffId);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Designation);
            Assert.Equal(string.Empty, dto.Gender);
        }

        [Fact]
        public void StaffDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new StaffDto
            {
                StaffId = 1,
                Name = "Alice Brown",
                Phone = "555-4321",
                Address = "789 Pine Rd",
                Designation = "Nurse",
                Gender = "F",
                Salary = 3500.0
            };

            // Assert
            Assert.Equal(1, dto.StaffId);
            Assert.Equal("Alice Brown", dto.Name);
            Assert.Equal("555-4321", dto.Phone);
            Assert.Equal("789 Pine Rd", dto.Address);
            Assert.Equal("Nurse", dto.Designation);
            Assert.Equal("F", dto.Gender);
            Assert.Equal(3500.0, dto.Salary);
        }

        [Fact]
        public void StaffCreateDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new StaffCreateDto
            {
                Name = "Bob Green",
                BirthDate = "1985-03-15",
                Phone = "555-7777",
                Gender = "M",
                Address = "321 Elm St",
                Salary = 2800.0,
                Qualification = "BSN",
                Designation = "Technician"
            };

            // Assert
            Assert.Equal("Bob Green", dto.Name);
            Assert.Equal("1985-03-15", dto.BirthDate);
            Assert.Equal("555-7777", dto.Phone);
            Assert.Equal("M", dto.Gender);
            Assert.Equal(2800.0, dto.Salary);
            Assert.Equal("BSN", dto.Qualification);
            Assert.Equal("Technician", dto.Designation);
        }

        [Fact]
        public void DepartmentDto_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var dto = new DepartmentDto
            {
                DeptNo = 1,
                DeptName = "Cardiology",
                Description = "Heart care",
                DoctorCount = 5
            };

            // Assert
            Assert.Equal(1, dto.DeptNo);
            Assert.Equal("Cardiology", dto.DeptName);
            Assert.Equal("Heart care", dto.Description);
            Assert.Equal(5, dto.DoctorCount);
        }
    }
}
