using System;
using System.Collections.Generic;
using Xunit;
using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Entities.Tests
{
    public class DoctorTests
    {
        [Fact]
        public void Doctor_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var doctor = new Doctor();

            // Assert
            Assert.Equal(0, doctor.DoctorId);
            Assert.Equal(string.Empty, doctor.Name);
            Assert.Equal(string.Empty, doctor.Gender);
            Assert.Equal(string.Empty, doctor.Qualification);
            Assert.Equal(0, doctor.PatientsTreated);
            Assert.NotNull(doctor.Appointments);
        }

        [Fact]
        public void Doctor_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var doctor = new Doctor
            {
                DoctorId = 1,
                Name = "Dr. John Smith",
                Phone = "555-1234",
                Address = "123 Main St",
                BirthDate = new DateTime(1975, 5, 15),
                Gender = "M",
                DeptNo = 2,
                ChargesPerVisit = 150.0,
                MonthlySalary = 5000.0,
                ReputeIndex = 4.5,
                PatientsTreated = 100,
                Qualification = "MBBS",
                Specialization = "Cardiology",
                WorkExperience = 10,
                Status = 1
            };

            // Assert
            Assert.Equal(1, doctor.DoctorId);
            Assert.Equal("Dr. John Smith", doctor.Name);
            Assert.Equal("555-1234", doctor.Phone);
            Assert.Equal("123 Main St", doctor.Address);
            Assert.Equal(new DateTime(1975, 5, 15), doctor.BirthDate);
            Assert.Equal("M", doctor.Gender);
            Assert.Equal(2, doctor.DeptNo);
            Assert.Equal(150.0, doctor.ChargesPerVisit);
            Assert.Equal(5000.0, doctor.MonthlySalary);
            Assert.Equal(4.5, doctor.ReputeIndex);
            Assert.Equal(100, doctor.PatientsTreated);
            Assert.Equal("MBBS", doctor.Qualification);
            Assert.Equal("Cardiology", doctor.Specialization);
            Assert.Equal(10, doctor.WorkExperience);
            Assert.Equal(1, doctor.Status);
        }

        [Fact]
        public void Doctor_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var doctor = new Doctor
            {
                Phone = null,
                Address = null,
                MonthlySalary = null,
                ReputeIndex = null,
                Specialization = null,
                WorkExperience = null
            };

            // Assert
            Assert.Null(doctor.Phone);
            Assert.Null(doctor.Address);
            Assert.Null(doctor.MonthlySalary);
            Assert.Null(doctor.ReputeIndex);
            Assert.Null(doctor.Specialization);
            Assert.Null(doctor.WorkExperience);
        }

        [Fact]
        public void Doctor_NavigationProperties_CanBeSet()
        {
            // Arrange
            var department = new Department { DeptNo = 1, DeptName = "Cardiology" };
            var login = new LoginTable { LoginId = 1, Email = "doc@test.com" };
            var appointments = new List<Appointment> { new Appointment { AppointId = 1 } };

            var doctor = new Doctor
            {
                Department = department,
                Login = login,
                Appointments = appointments
            };

            // Assert
            Assert.NotNull(doctor.Department);
            Assert.Equal("Cardiology", doctor.Department.DeptName);
            Assert.NotNull(doctor.Login);
            Assert.Equal("doc@test.com", doctor.Login.Email);
            Assert.Single(doctor.Appointments);
        }

        [Fact]
        public void Doctor_AppointmentsCollection_DefaultIsEmpty()
        {
            // Arrange & Act
            var doctor = new Doctor();

            // Assert
            Assert.Empty(doctor.Appointments);
        }

        [Fact]
        public void Doctor_Status_CanBeSetToPresent()
        {
            // Arrange & Act
            var doctor = new Doctor { Status = 1 };

            // Assert
            Assert.Equal(1, doctor.Status);
        }

        [Fact]
        public void Doctor_Status_CanBeSetToLeft()
        {
            // Arrange & Act
            var doctor = new Doctor { Status = 0 };

            // Assert
            Assert.Equal(0, doctor.Status);
        }

        [Fact]
        public void Doctor_Gender_CanBeMale()
        {
            // Arrange & Act
            var doctor = new Doctor { Gender = "M" };

            // Assert
            Assert.Equal("M", doctor.Gender);
        }

        [Fact]
        public void Doctor_Gender_CanBeFemale()
        {
            // Arrange & Act
            var doctor = new Doctor { Gender = "F" };

            // Assert
            Assert.Equal("F", doctor.Gender);
        }
    }
}
