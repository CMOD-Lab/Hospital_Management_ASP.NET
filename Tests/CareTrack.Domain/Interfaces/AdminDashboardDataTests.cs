using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using CareTrack.Domain.Interfaces.Services;

namespace CareTrack.Domain.Interfaces.Services.Tests
{
    public class AdminDashboardDataTests
    {
        [Fact]
        public void AdminDashboardData_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var data = new AdminDashboardData();

            // Assert
            Assert.Equal(0, data.TotalDoctors);
            Assert.Equal(0, data.TotalPatients);
            Assert.Equal(0.0, data.TotalIncome);
            Assert.NotNull(data.DepartmentStats);
            Assert.NotNull(data.AppointmentStats);
        }

        [Fact]
        public void AdminDashboardData_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var deptStats = new List<DepartmentStat>
            {
                new DepartmentStat { DeptName = "Cardiology", DoctorCount = 3 }
            };
            var apptStats = new List<AppointmentStat>
            {
                new AppointmentStat { AppointId = 1, PatientName = "John", DoctorName = "Dr. Smith", Status = "Approved" }
            };

            // Act
            var data = new AdminDashboardData
            {
                TotalDoctors = 10,
                TotalPatients = 50,
                TotalIncome = 5000.0,
                DepartmentStats = deptStats,
                AppointmentStats = apptStats
            };

            // Assert
            Assert.Equal(10, data.TotalDoctors);
            Assert.Equal(50, data.TotalPatients);
            Assert.Equal(5000.0, data.TotalIncome);
            Assert.Single(data.DepartmentStats);
            Assert.Single(data.AppointmentStats);
        }

        [Fact]
        public void DepartmentStat_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var stat = new DepartmentStat();

            // Assert
            Assert.Equal(string.Empty, stat.DeptName);
            Assert.Equal(0, stat.DoctorCount);
        }

        [Fact]
        public void DepartmentStat_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var stat = new DepartmentStat
            {
                DeptName = "Neurology",
                DoctorCount = 5
            };

            // Assert
            Assert.Equal("Neurology", stat.DeptName);
            Assert.Equal(5, stat.DoctorCount);
        }

        [Fact]
        public void AppointmentStat_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var stat = new AppointmentStat();

            // Assert
            Assert.Equal(0, stat.AppointId);
            Assert.Equal(string.Empty, stat.PatientName);
            Assert.Equal(string.Empty, stat.DoctorName);
            Assert.Null(stat.Date);
            Assert.Equal(string.Empty, stat.Status);
        }

        [Fact]
        public void AppointmentStat_SetProperties_ReturnsCorrectValues()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15);

            // Act
            var stat = new AppointmentStat
            {
                AppointId = 1,
                PatientName = "Jane Doe",
                DoctorName = "Dr. Jones",
                Date = date,
                Status = "Completed"
            };

            // Assert
            Assert.Equal(1, stat.AppointId);
            Assert.Equal("Jane Doe", stat.PatientName);
            Assert.Equal("Dr. Jones", stat.DoctorName);
            Assert.Equal(date, stat.Date);
            Assert.Equal("Completed", stat.Status);
        }

        [Fact]
        public void AdminDashboardData_DepartmentStats_CanBeEmpty()
        {
            // Arrange & Act
            var data = new AdminDashboardData
            {
                DepartmentStats = new List<DepartmentStat>()
            };

            // Assert
            Assert.Empty(data.DepartmentStats);
        }

        [Fact]
        public void AdminDashboardData_AppointmentStats_CanBeEmpty()
        {
            // Arrange & Act
            var data = new AdminDashboardData
            {
                AppointmentStats = new List<AppointmentStat>()
            };

            // Assert
            Assert.Empty(data.AppointmentStats);
        }

        [Fact]
        public void AdminDashboardData_TotalIncome_CanBeZero()
        {
            // Arrange & Act
            var data = new AdminDashboardData { TotalIncome = 0.0 };

            // Assert
            Assert.Equal(0.0, data.TotalIncome);
        }

        [Fact]
        public void AdminDashboardData_TotalIncome_CanBeHighValue()
        {
            // Arrange & Act
            var data = new AdminDashboardData { TotalIncome = 999999.99 };

            // Assert
            Assert.Equal(999999.99, data.TotalIncome);
        }
    }
}
