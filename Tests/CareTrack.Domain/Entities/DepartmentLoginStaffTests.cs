using System.Collections.Generic;
using Xunit;
using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Entities.Tests
{
    public class DepartmentTests
    {
        [Fact]
        public void Department_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var department = new Department();

            // Assert
            Assert.Equal(0, department.DeptNo);
            Assert.Equal(string.Empty, department.DeptName);
            Assert.Null(department.Description);
            Assert.NotNull(department.Doctors);
            Assert.Empty(department.Doctors);
        }

        [Fact]
        public void Department_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var department = new Department
            {
                DeptNo = 1,
                DeptName = "Cardiology",
                Description = "Heart and cardiovascular care"
            };

            // Assert
            Assert.Equal(1, department.DeptNo);
            Assert.Equal("Cardiology", department.DeptName);
            Assert.Equal("Heart and cardiovascular care", department.Description);
        }

        [Fact]
        public void Department_Description_CanBeNull()
        {
            // Arrange & Act
            var department = new Department { Description = null };

            // Assert
            Assert.Null(department.Description);
        }

        [Fact]
        public void Department_DoctorsCollection_CanBePopulated()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, Name = "Dr. Smith" },
                new Doctor { DoctorId = 2, Name = "Dr. Jones" }
            };

            var department = new Department
            {
                DeptNo = 1,
                DeptName = "Cardiology",
                Doctors = doctors
            };

            // Assert
            Assert.Equal(2, department.Doctors.Count);
        }
    }

    public class LoginTableTests
    {
        [Fact]
        public void LoginTable_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var login = new LoginTable();

            // Assert
            Assert.Equal(0, login.LoginId);
            Assert.Equal(string.Empty, login.Password);
            Assert.Equal(string.Empty, login.Email);
            Assert.Equal(0, login.Type);
        }

        [Fact]
        public void LoginTable_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var login = new LoginTable
            {
                LoginId = 1,
                Password = "securepass",
                Email = "user@example.com",
                Type = 1
            };

            // Assert
            Assert.Equal(1, login.LoginId);
            Assert.Equal("securepass", login.Password);
            Assert.Equal("user@example.com", login.Email);
            Assert.Equal(1, login.Type);
        }

        [Fact]
        public void LoginTable_Type_PatientValue()
        {
            // Arrange & Act
            var login = new LoginTable { Type = 1 };

            // Assert
            Assert.Equal(1, login.Type);
        }

        [Fact]
        public void LoginTable_Type_DoctorValue()
        {
            // Arrange & Act
            var login = new LoginTable { Type = 2 };

            // Assert
            Assert.Equal(2, login.Type);
        }

        [Fact]
        public void LoginTable_Type_AdminValue()
        {
            // Arrange & Act
            var login = new LoginTable { Type = 3 };

            // Assert
            Assert.Equal(3, login.Type);
        }

        [Fact]
        public void LoginTable_NavigationProperties_CanBeSet()
        {
            // Arrange
            var patient = new Patient { PatientId = 1, Name = "John" };
            var doctor = new Doctor { DoctorId = 2, Name = "Dr. Smith" };

            var login = new LoginTable
            {
                Patient = patient,
                Doctor = doctor
            };

            // Assert
            Assert.NotNull(login.Patient);
            Assert.Equal("John", login.Patient.Name);
            Assert.NotNull(login.Doctor);
            Assert.Equal("Dr. Smith", login.Doctor.Name);
        }

        [Fact]
        public void LoginTable_NavigationProperties_CanBeNull()
        {
            // Arrange & Act
            var login = new LoginTable
            {
                Patient = null,
                Doctor = null
            };

            // Assert
            Assert.Null(login.Patient);
            Assert.Null(login.Doctor);
        }
    }

    public class OtherStaffTests
    {
        [Fact]
        public void OtherStaff_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var staff = new OtherStaff();

            // Assert
            Assert.Equal(0, staff.StaffId);
            Assert.Equal(string.Empty, staff.Name);
            Assert.Equal(string.Empty, staff.Designation);
            Assert.Equal(string.Empty, staff.Gender);
        }

        [Fact]
        public void OtherStaff_SetProperties_ReturnsCorrectValues()
        {
            // Arrange & Act
            var birthDate = new System.DateTime(1985, 7, 10);
            var staff = new OtherStaff
            {
                StaffId = 5,
                Name = "Alice Brown",
                Phone = "555-4321",
                Address = "789 Pine Rd",
                Designation = "Nurse",
                Gender = "F",
                BirthDate = birthDate,
                HighestQualification = "BSN",
                Salary = 3500.0
            };

            // Assert
            Assert.Equal(5, staff.StaffId);
            Assert.Equal("Alice Brown", staff.Name);
            Assert.Equal("555-4321", staff.Phone);
            Assert.Equal("789 Pine Rd", staff.Address);
            Assert.Equal("Nurse", staff.Designation);
            Assert.Equal("F", staff.Gender);
            Assert.Equal(birthDate, staff.BirthDate);
            Assert.Equal("BSN", staff.HighestQualification);
            Assert.Equal(3500.0, staff.Salary);
        }

        [Fact]
        public void OtherStaff_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var staff = new OtherStaff
            {
                Phone = null,
                Address = null,
                BirthDate = null,
                HighestQualification = null,
                Salary = null
            };

            // Assert
            Assert.Null(staff.Phone);
            Assert.Null(staff.Address);
            Assert.Null(staff.BirthDate);
            Assert.Null(staff.HighestQualification);
            Assert.Null(staff.Salary);
        }
    }
}
