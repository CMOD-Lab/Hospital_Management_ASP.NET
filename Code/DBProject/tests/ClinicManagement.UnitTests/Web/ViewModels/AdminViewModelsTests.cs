using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Web.ViewModels;

/// <summary>
/// Unit tests for AdminViewModels.
/// </summary>
public class AdminViewModelsTests
{
    // ─── AdminHomeViewModel ───────────────────────────────────────────────────

    [Fact]
    public void AdminHomeViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new AdminHomeViewModel();

        // Assert
        vm.TotalPatients.Should().Be(0);
        vm.TotalDoctors.Should().Be(0);
        vm.TotalIncome.Should().Be(0.0);
        vm.Departments.Should().NotBeNull().And.BeEmpty();
        vm.RecentAppointments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void AdminHomeViewModel_SetTotalPatients_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new AdminHomeViewModel();

        // Act
        vm.TotalPatients = 150;

        // Assert
        vm.TotalPatients.Should().Be(150);
    }

    [Fact]
    public void AdminHomeViewModel_SetTotalDoctors_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new AdminHomeViewModel();

        // Act
        vm.TotalDoctors = 25;

        // Assert
        vm.TotalDoctors.Should().Be(25);
    }

    [Fact]
    public void AdminHomeViewModel_SetTotalIncome_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new AdminHomeViewModel();

        // Act
        vm.TotalIncome = 50000.75;

        // Assert
        vm.TotalIncome.Should().Be(50000.75);
    }

    [Fact]
    public void AdminHomeViewModel_SetDepartments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new AdminHomeViewModel();
        var departments = new List<DepartmentViewModel>
        {
            new() { DeptNo = 1, DeptName = "Cardiology" },
            new() { DeptNo = 2, DeptName = "Neurology" }
        };

        // Act
        vm.Departments = departments;

        // Assert
        vm.Departments.Should().HaveCount(2);
        vm.Departments.First().DeptName.Should().Be("Cardiology");
    }

    [Fact]
    public void AdminHomeViewModel_SetRecentAppointments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new AdminHomeViewModel();
        var appointments = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, DoctorName = "Dr. Smith", PatientName = "John" }
        };

        // Act
        vm.RecentAppointments = appointments;

        // Assert
        vm.RecentAppointments.Should().HaveCount(1);
        vm.RecentAppointments.First().DoctorName.Should().Be("Dr. Smith");
    }

    // ─── ManageClinicViewModel ────────────────────────────────────────────────

    [Fact]
    public void ManageClinicViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new ManageClinicViewModel();

        // Assert
        vm.SearchQuery.Should().Be(string.Empty);
        vm.Doctors.Should().NotBeNull().And.BeEmpty();
        vm.Staff.Should().NotBeNull().And.BeEmpty();
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void ManageClinicViewModel_SetSearchQuery_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new ManageClinicViewModel();

        // Act
        vm.SearchQuery = "Smith";

        // Assert
        vm.SearchQuery.Should().Be("Smith");
    }

    [Fact]
    public void ManageClinicViewModel_SetMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new ManageClinicViewModel();

        // Act
        vm.Message = "Doctor removed successfully.";

        // Assert
        vm.Message.Should().Be("Doctor removed successfully.");
    }

    // ─── DoctorRegistrationViewModel ─────────────────────────────────────────

    [Fact]
    public void DoctorRegistrationViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new DoctorRegistrationViewModel();

        // Assert
        vm.Name.Should().Be(string.Empty);
        vm.Email.Should().Be(string.Empty);
        vm.Password.Should().Be(string.Empty);
        vm.BirthDate.Should().Be(string.Empty);
        vm.DeptNo.Should().Be(0);
        vm.Phone.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.Address.Should().BeNull();
        vm.WorkExperience.Should().Be(0);
        vm.Salary.Should().Be(0);
        vm.ChargesPerVisit.Should().Be(0);
        vm.Specialization.Should().BeNull();
        vm.Qualification.Should().BeNull();
        vm.Departments.Should().NotBeNull().And.BeEmpty();
        vm.ErrorMessage.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
    }

    [Fact]
    public void DoctorRegistrationViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new DoctorRegistrationViewModel
        {
            Name = "Dr. Alice",
            Email = "alice@clinic.com",
            Password = "Doc@123",
            BirthDate = "1980-05-20",
            DeptNo = 3,
            Phone = "9876543210",
            Gender = "F",
            Address = "456 Oak Ave",
            WorkExperience = 10,
            Salary = 80000,
            ChargesPerVisit = 500,
            Specialization = "Cardiology",
            Qualification = "MBBS, MD",
            ErrorMessage = null,
            SuccessMessage = "Doctor registered!"
        };

        // Assert
        vm.Name.Should().Be("Dr. Alice");
        vm.Email.Should().Be("alice@clinic.com");
        vm.DeptNo.Should().Be(3);
        vm.WorkExperience.Should().Be(10);
        vm.Salary.Should().Be(80000);
        vm.ChargesPerVisit.Should().Be(500);
        vm.Specialization.Should().Be("Cardiology");
        vm.SuccessMessage.Should().Be("Doctor registered!");
    }

    // ─── AddStaffViewModel ────────────────────────────────────────────────────

    [Fact]
    public void AddStaffViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new AddStaffViewModel();

        // Assert
        vm.Name.Should().Be(string.Empty);
        vm.BirthDate.Should().Be(string.Empty);
        vm.Phone.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.Address.Should().BeNull();
        vm.Salary.Should().Be(0);
        vm.Qualification.Should().BeNull();
        vm.Designation.Should().BeNull();
        vm.ErrorMessage.Should().BeNull();
        vm.SuccessMessage.Should().BeNull();
    }

    [Fact]
    public void AddStaffViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new AddStaffViewModel
        {
            Name = "Bob Staff",
            BirthDate = "1985-03-10",
            Phone = "5551234567",
            Gender = "M",
            Address = "789 Pine Rd",
            Salary = 40000,
            Qualification = "BSc Nursing",
            Designation = "Nurse",
            ErrorMessage = null,
            SuccessMessage = "Staff added!"
        };

        // Assert
        vm.Name.Should().Be("Bob Staff");
        vm.BirthDate.Should().Be("1985-03-10");
        vm.Phone.Should().Be("5551234567");
        vm.Gender.Should().Be("M");
        vm.Salary.Should().Be(40000);
        vm.Qualification.Should().Be("BSc Nursing");
        vm.Designation.Should().Be("Nurse");
        vm.SuccessMessage.Should().Be("Staff added!");
    }

    [Fact]
    public void AddStaffViewModel_SetErrorMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new AddStaffViewModel();

        // Act
        vm.ErrorMessage = "Failed to add staff.";

        // Assert
        vm.ErrorMessage.Should().Be("Failed to add staff.");
    }
}
