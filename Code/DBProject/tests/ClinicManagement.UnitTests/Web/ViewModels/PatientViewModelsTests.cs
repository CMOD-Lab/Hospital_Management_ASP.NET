using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Web.ViewModels;

/// <summary>
/// Unit tests for PatientViewModels.
/// </summary>
public class PatientViewModelsTests
{
    // ─── PatientHomeViewModel ─────────────────────────────────────────────────

    [Fact]
    public void PatientHomeViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PatientHomeViewModel();

        // Assert
        vm.PatientId.Should().Be(0);
        vm.Name.Should().Be(string.Empty);
        vm.Phone.Should().BeNull();
        vm.Address.Should().BeNull();
        vm.BirthDate.Should().Be(string.Empty);
        vm.Age.Should().Be(0);
        vm.Gender.Should().Be(string.Empty);
    }

    [Fact]
    public void PatientHomeViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new PatientHomeViewModel
        {
            PatientId = 42,
            Name = "Jane Doe",
            Phone = "5559876543",
            Address = "100 Elm St",
            BirthDate = "1995-07-22",
            Age = 29,
            Gender = "F"
        };

        // Assert
        vm.PatientId.Should().Be(42);
        vm.Name.Should().Be("Jane Doe");
        vm.Phone.Should().Be("5559876543");
        vm.Address.Should().Be("100 Elm St");
        vm.BirthDate.Should().Be("1995-07-22");
        vm.Age.Should().Be(29);
        vm.Gender.Should().Be("F");
    }

    // ─── ViewDoctorsViewModel ─────────────────────────────────────────────────

    [Fact]
    public void ViewDoctorsViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new ViewDoctorsViewModel();

        // Assert
        vm.Departments.Should().NotBeNull().And.BeEmpty();
        vm.Doctors.Should().NotBeNull().And.BeEmpty();
        vm.SelectedDepartment.Should().Be(string.Empty);
    }

    [Fact]
    public void ViewDoctorsViewModel_SetSelectedDepartment_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new ViewDoctorsViewModel();

        // Act
        vm.SelectedDepartment = "Cardiology";

        // Assert
        vm.SelectedDepartment.Should().Be("Cardiology");
    }

    [Fact]
    public void ViewDoctorsViewModel_SetDoctors_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new ViewDoctorsViewModel();
        var doctors = new List<DoctorViewModel>
        {
            new() { DoctorId = 1, Name = "Dr. Smith", DepartmentName = "Cardiology" },
            new() { DoctorId = 2, Name = "Dr. Jones", DepartmentName = "Neurology" }
        };

        // Act
        vm.Doctors = doctors;

        // Assert
        vm.Doctors.Should().HaveCount(2);
        vm.Doctors.First().Name.Should().Be("Dr. Smith");
    }

    // ─── DoctorProfileViewModel ───────────────────────────────────────────────

    [Fact]
    public void DoctorProfileViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new DoctorProfileViewModel();

        // Assert
        vm.DoctorId.Should().Be(0);
        vm.Name.Should().Be(string.Empty);
        vm.Phone.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.ChargesPerVisit.Should().Be(0.0);
        vm.ReputeIndex.Should().Be(0.0);
        vm.PatientsTreated.Should().Be(0);
        vm.Qualification.Should().BeNull();
        vm.Specialization.Should().BeNull();
        vm.WorkExperience.Should().Be(0);
        vm.Age.Should().Be(0);
    }

    [Fact]
    public void DoctorProfileViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new DoctorProfileViewModel
        {
            DoctorId = 10,
            Name = "Dr. Brown",
            Phone = "5551112222",
            Gender = "M",
            ChargesPerVisit = 300.0,
            ReputeIndex = 4.8,
            PatientsTreated = 500,
            Qualification = "MBBS",
            Specialization = "Orthopedics",
            WorkExperience = 15,
            Age = 45
        };

        // Assert
        vm.DoctorId.Should().Be(10);
        vm.Name.Should().Be("Dr. Brown");
        vm.ChargesPerVisit.Should().Be(300.0);
        vm.ReputeIndex.Should().Be(4.8);
        vm.PatientsTreated.Should().Be(500);
        vm.WorkExperience.Should().Be(15);
        vm.Age.Should().Be(45);
    }

    // ─── TakeAppointmentViewModel ─────────────────────────────────────────────

    [Fact]
    public void TakeAppointmentViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new TakeAppointmentViewModel();

        // Assert
        vm.Departments.Should().NotBeNull().And.BeEmpty();
        vm.SelectedDepartment.Should().Be(string.Empty);
        vm.SelectedDoctorId.Should().Be(0);
        vm.Doctors.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void TakeAppointmentViewModel_SetSelectedDoctorId_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new TakeAppointmentViewModel();

        // Act
        vm.SelectedDoctorId = 7;

        // Assert
        vm.SelectedDoctorId.Should().Be(7);
    }

    // ─── AppointmentTakerViewModel ────────────────────────────────────────────

    [Fact]
    public void AppointmentTakerViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new AppointmentTakerViewModel();

        // Assert
        vm.DoctorId.Should().Be(0);
        vm.PatientId.Should().Be(0);
        vm.FreeSlots.Should().NotBeNull().And.BeEmpty();
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void AppointmentTakerViewModel_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var vm = new AppointmentTakerViewModel
        {
            DoctorId = 5,
            PatientId = 12,
            Message = "Slot booked!"
        };

        // Assert
        vm.DoctorId.Should().Be(5);
        vm.PatientId.Should().Be(12);
        vm.Message.Should().Be("Slot booked!");
    }

    // ─── CurrentAppointmentViewModel ─────────────────────────────────────────

    [Fact]
    public void CurrentAppointmentViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new CurrentAppointmentViewModel();

        // Assert
        vm.DoctorName.Should().BeNull();
        vm.Timings.Should().BeNull();
        vm.HasAppointment.Should().BeFalse();
    }

    [Fact]
    public void CurrentAppointmentViewModel_SetHasAppointmentTrue_ReturnsTrue()
    {
        // Arrange
        var vm = new CurrentAppointmentViewModel();

        // Act
        vm.HasAppointment = true;
        vm.DoctorName = "Dr. Green";
        vm.Timings = "10:00 AM";

        // Assert
        vm.HasAppointment.Should().BeTrue();
        vm.DoctorName.Should().Be("Dr. Green");
        vm.Timings.Should().Be("10:00 AM");
    }

    // ─── BillsHistoryViewModel ────────────────────────────────────────────────

    [Fact]
    public void BillsHistoryViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new BillsHistoryViewModel();

        // Assert
        vm.Bills.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void BillsHistoryViewModel_SetBills_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new BillsHistoryViewModel();
        var bills = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, BillStatus = "Paid" },
            new() { AppointmentId = 2, BillStatus = "Unpaid" }
        };

        // Act
        vm.Bills = bills;

        // Assert
        vm.Bills.Should().HaveCount(2);
        vm.Bills.First().BillStatus.Should().Be("Paid");
    }

    // ─── TreatmentHistoryViewModel ────────────────────────────────────────────

    [Fact]
    public void TreatmentHistoryViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new TreatmentHistoryViewModel();

        // Assert
        vm.Treatments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void TreatmentHistoryViewModel_SetTreatments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new TreatmentHistoryViewModel();
        var treatments = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, Disease = "Flu", Prescription = "Rest" }
        };

        // Act
        vm.Treatments = treatments;

        // Assert
        vm.Treatments.Should().HaveCount(1);
        vm.Treatments.First().Disease.Should().Be("Flu");
    }

    // ─── PatientNotificationsViewModel ───────────────────────────────────────

    [Fact]
    public void PatientNotificationsViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PatientNotificationsViewModel();

        // Assert
        vm.DoctorName.Should().BeNull();
        vm.Timings.Should().BeNull();
        vm.HasNotification.Should().BeFalse();
    }

    [Fact]
    public void PatientNotificationsViewModel_SetHasNotificationTrue_ReturnsTrue()
    {
        // Arrange
        var vm = new PatientNotificationsViewModel
        {
            HasNotification = true,
            DoctorName = "Dr. White",
            Timings = "2:00 PM"
        };

        // Assert
        vm.HasNotification.Should().BeTrue();
        vm.DoctorName.Should().Be("Dr. White");
        vm.Timings.Should().Be("2:00 PM");
    }

    // ─── PatientFeedbackViewModel ─────────────────────────────────────────────

    [Fact]
    public void PatientFeedbackViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PatientFeedbackViewModel();

        // Assert
        vm.AppointmentId.Should().Be(0);
        vm.DoctorName.Should().BeNull();
        vm.Timings.Should().BeNull();
        vm.HasPendingFeedback.Should().BeFalse();
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void PatientFeedbackViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new PatientFeedbackViewModel
        {
            AppointmentId = 99,
            DoctorName = "Dr. Black",
            Timings = "3:30 PM",
            HasPendingFeedback = true,
            Message = "Thank you for your feedback!"
        };

        // Assert
        vm.AppointmentId.Should().Be(99);
        vm.DoctorName.Should().Be("Dr. Black");
        vm.Timings.Should().Be("3:30 PM");
        vm.HasPendingFeedback.Should().BeTrue();
        vm.Message.Should().Be("Thank you for your feedback!");
    }
}
