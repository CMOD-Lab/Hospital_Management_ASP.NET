using System;
using System.Collections.Generic;
using Xunit;
using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Entities.Tests
{
    public class PatientTests
    {
        [Fact]
        public void Patient_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var patient = new Patient();

            // Assert
            Assert.Equal(0, patient.PatientId);
            Assert.Equal(string.Empty, patient.Name);
            Assert.Equal(string.Empty, patient.Gender);
            Assert.NotNull(patient.Appointments);
        }

        [Fact]
        public void Patient_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var patient = new Patient
            {
                PatientId = 10,
                Name = "Jane Doe",
                Phone = "555-9876",
                Address = "456 Oak Ave",
                BirthDate = new DateTime(1990, 3, 20),
                Gender = "F"
            };

            // Assert
            Assert.Equal(10, patient.PatientId);
            Assert.Equal("Jane Doe", patient.Name);
            Assert.Equal("555-9876", patient.Phone);
            Assert.Equal("456 Oak Ave", patient.Address);
            Assert.Equal(new DateTime(1990, 3, 20), patient.BirthDate);
            Assert.Equal("F", patient.Gender);
        }

        [Fact]
        public void Patient_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var patient = new Patient
            {
                Phone = null,
                Address = null
            };

            // Assert
            Assert.Null(patient.Phone);
            Assert.Null(patient.Address);
        }

        [Fact]
        public void Patient_AppointmentsCollection_DefaultIsEmpty()
        {
            // Arrange & Act
            var patient = new Patient();

            // Assert
            Assert.Empty(patient.Appointments);
        }

        [Fact]
        public void Patient_NavigationProperties_CanBeSet()
        {
            // Arrange
            var login = new LoginTable { LoginId = 5, Email = "patient@test.com" };
            var appointments = new List<Appointment>
            {
                new Appointment { AppointId = 1 },
                new Appointment { AppointId = 2 }
            };

            var patient = new Patient
            {
                Login = login,
                Appointments = appointments
            };

            // Assert
            Assert.NotNull(patient.Login);
            Assert.Equal("patient@test.com", patient.Login.Email);
            Assert.Equal(2, patient.Appointments.Count);
        }

        [Fact]
        public void Patient_Gender_CanBeMale()
        {
            // Arrange & Act
            var patient = new Patient { Gender = "M" };

            // Assert
            Assert.Equal("M", patient.Gender);
        }

        [Fact]
        public void Patient_Gender_CanBeFemale()
        {
            // Arrange & Act
            var patient = new Patient { Gender = "F" };

            // Assert
            Assert.Equal("F", patient.Gender);
        }

        [Fact]
        public void Patient_LoginNavigation_CanBeNull()
        {
            // Arrange & Act
            var patient = new Patient { Login = null };

            // Assert
            Assert.Null(patient.Login);
        }
    }
}
