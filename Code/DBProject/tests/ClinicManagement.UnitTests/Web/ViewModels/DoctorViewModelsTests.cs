using ClinicManagement.Web.ViewModels;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Web.ViewModels;

/// <summary>
/// Unit tests for DoctorViewModels (shared and doctor-specific).
/// </summary>
public class DoctorViewModelsTests
{
    // ─── DoctorHomeViewModel ──────────────────────────────────────────────────

    [Fact]
    public void DoctorHomeViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new DoctorHomeViewModel();

        // Assert
        vm.DoctorId.Should().Be(0);
        vm.Name.Should().Be(string.Empty);
        vm.Phone.Should().BeNull();
        vm.Address.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.DepartmentName.Should().Be(string.Empty);
        vm.ChargesPerVisit.Should().Be(0.0);
        vm.WorkExperience.Should().Be(0);
        vm.Qualification.Should().BeNull();
        vm.Specialization.Should().BeNull();
    }

    [Fact]
    public void DoctorHomeViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new DoctorHomeViewModel
        {
            DoctorId = 7,
            Name = "Dr. Taylor",
            Phone = "5553334444",
            Address = "200 Maple Dr",
            Gender = "M",
            DepartmentName = "Orthopedics",
            ChargesPerVisit = 400.0,
            WorkExperience = 12,
            Qualification = "MBBS, MS",
            Specialization = "Bone Surgery"
        };

        // Assert
        vm.DoctorId.Should().Be(7);
        vm.Name.Should().Be("Dr. Taylor");
        vm.DepartmentName.Should().Be("Orthopedics");
        vm.ChargesPerVisit.Should().Be(400.0);
        vm.WorkExperience.Should().Be(12);
        vm.Qualification.Should().Be("MBBS, MS");
        vm.Specialization.Should().Be("Bone Surgery");
    }

    // ─── PendingAppointmentViewModel ─────────────────────────────────────────

    [Fact]
    public void PendingAppointmentViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PendingAppointmentViewModel();

        // Assert
        vm.Appointments.Should().NotBeNull().And.BeEmpty();
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void PendingAppointmentViewModel_SetAppointments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new PendingAppointmentViewModel();
        var appointments = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, PatientName = "Alice", Status = "Pending" },
            new() { AppointmentId = 2, PatientName = "Bob", Status = "Pending" }
        };

        // Act
        vm.Appointments = appointments;

        // Assert
        vm.Appointments.Should().HaveCount(2);
        vm.Appointments.First().PatientName.Should().Be("Alice");
    }

    [Fact]
    public void PendingAppointmentViewModel_SetMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new PendingAppointmentViewModel();

        // Act
        vm.Message = "Appointment approved.";

        // Assert
        vm.Message.Should().Be("Appointment approved.");
    }

    // ─── PatientHistoryViewModel ──────────────────────────────────────────────

    [Fact]
    public void PatientHistoryViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PatientHistoryViewModel();

        // Assert
        vm.Appointments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PatientHistoryViewModel_SetAppointments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new PatientHistoryViewModel();
        var appointments = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 10, PatientName = "Charlie", Timings = "9:00 AM" }
        };

        // Act
        vm.Appointments = appointments;

        // Assert
        vm.Appointments.Should().HaveCount(1);
        vm.Appointments.First().Timings.Should().Be("9:00 AM");
    }

    // ─── HistoryUpdateViewModel ───────────────────────────────────────────────

    [Fact]
    public void HistoryUpdateViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new HistoryUpdateViewModel();

        // Assert
        vm.AppointmentId.Should().Be(0);
        vm.DoctorId.Should().Be(0);
        vm.PatientName.Should().Be(string.Empty);
        vm.Disease.Should().Be(string.Empty);
        vm.Progress.Should().Be(string.Empty);
        vm.Prescription.Should().Be(string.Empty);
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void HistoryUpdateViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new HistoryUpdateViewModel
        {
            AppointmentId = 55,
            DoctorId = 3,
            PatientName = "David",
            Disease = "Hypertension",
            Progress = "Improving",
            Prescription = "Amlodipine 5mg",
            Message = "History updated."
        };

        // Assert
        vm.AppointmentId.Should().Be(55);
        vm.DoctorId.Should().Be(3);
        vm.PatientName.Should().Be("David");
        vm.Disease.Should().Be("Hypertension");
        vm.Progress.Should().Be("Improving");
        vm.Prescription.Should().Be("Amlodipine 5mg");
        vm.Message.Should().Be("History updated.");
    }

    // ─── BillViewModel ────────────────────────────────────────────────────────

    [Fact]
    public void BillViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new BillViewModel();

        // Assert
        vm.Appointments.Should().NotBeNull().And.BeEmpty();
        vm.Message.Should().BeNull();
    }

    [Fact]
    public void BillViewModel_SetAppointments_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new BillViewModel();
        var appointments = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, BillStatus = "Unpaid" },
            new() { AppointmentId = 2, BillStatus = "Paid" }
        };

        // Act
        vm.Appointments = appointments;

        // Assert
        vm.Appointments.Should().HaveCount(2);
        vm.Appointments.Last().BillStatus.Should().Be("Paid");
    }

    [Fact]
    public void BillViewModel_SetMessage_ReturnsCorrectValue()
    {
        // Arrange
        var vm = new BillViewModel();

        // Act
        vm.Message = "Bill marked as paid.";

        // Assert
        vm.Message.Should().Be("Bill marked as paid.");
    }

    // ─── PreviousHistoryViewModel ─────────────────────────────────────────────

    [Fact]
    public void PreviousHistoryViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new PreviousHistoryViewModel();

        // Assert
        vm.History.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PreviousHistoryViewModel_SetHistory_ReturnsCorrectCollection()
    {
        // Arrange
        var vm = new PreviousHistoryViewModel();
        var history = new List<AppointmentViewModel>
        {
            new() { AppointmentId = 1, Disease = "Diabetes", Prescription = "Metformin" }
        };

        // Act
        vm.History = history;

        // Assert
        vm.History.Should().HaveCount(1);
        vm.History.First().Disease.Should().Be("Diabetes");
    }

    // ─── DoctorViewModel (shared) ─────────────────────────────────────────────

    [Fact]
    public void DoctorViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new DoctorViewModel();

        // Assert
        vm.DoctorId.Should().Be(0);
        vm.Name.Should().Be(string.Empty);
        vm.DepartmentName.Should().Be(string.Empty);
        vm.DeptNo.Should().Be(0);
        vm.Phone.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.ChargesPerVisit.Should().Be(0.0);
        vm.WorkExperience.Should().Be(0);
        vm.Qualification.Should().BeNull();
        vm.Specialization.Should().BeNull();
        vm.ReputeIndex.Should().Be(0.0);
        vm.PatientsTreated.Should().Be(0);
        vm.Age.Should().Be(0);
        vm.Status.Should().Be(0);
    }

    [Fact]
    public void DoctorViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new DoctorViewModel
        {
            DoctorId = 15,
            Name = "Dr. Wilson",
            DepartmentName = "Dermatology",
            DeptNo = 4,
            Phone = "5557778888",
            Gender = "M",
            ChargesPerVisit = 250.0,
            WorkExperience = 8,
            Qualification = "MBBS, MD",
            Specialization = "Skin",
            ReputeIndex = 4.5,
            PatientsTreated = 300,
            Age = 38,
            Status = 1
        };

        // Assert
        vm.DoctorId.Should().Be(15);
        vm.Name.Should().Be("Dr. Wilson");
        vm.DepartmentName.Should().Be("Dermatology");
        vm.ReputeIndex.Should().Be(4.5);
        vm.PatientsTreated.Should().Be(300);
        vm.Status.Should().Be(1);
    }

    // ─── DepartmentViewModel (shared) ────────────────────────────────────────

    [Fact]
    public void DepartmentViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new DepartmentViewModel();

        // Assert
        vm.DeptNo.Should().Be(0);
        vm.DeptName.Should().Be(string.Empty);
        vm.Description.Should().BeNull();
    }

    [Fact]
    public void DepartmentViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new DepartmentViewModel
        {
            DeptNo = 2,
            DeptName = "Neurology",
            Description = "Brain and nervous system"
        };

        // Assert
        vm.DeptNo.Should().Be(2);
        vm.DeptName.Should().Be("Neurology");
        vm.Description.Should().Be("Brain and nervous system");
    }

    // ─── AppointmentViewModel (shared) ───────────────────────────────────────

    [Fact]
    public void AppointmentViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new AppointmentViewModel();

        // Assert
        vm.AppointmentId.Should().Be(0);
        vm.DoctorId.Should().Be(0);
        vm.PatientId.Should().Be(0);
        vm.DoctorName.Should().BeNull();
        vm.PatientName.Should().BeNull();
        vm.Timings.Should().BeNull();
        vm.Status.Should().Be(string.Empty);
        vm.Disease.Should().BeNull();
        vm.Progress.Should().BeNull();
        vm.Prescription.Should().BeNull();
        vm.BillStatus.Should().Be(string.Empty);
        vm.AppointmentDate.Should().BeNull();
        vm.FeedbackStatus.Should().Be(0);
    }

    [Fact]
    public void AppointmentViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15);

        // Act
        var vm = new AppointmentViewModel
        {
            AppointmentId = 100,
            DoctorId = 5,
            PatientId = 20,
            DoctorName = "Dr. Adams",
            PatientName = "Eve",
            Timings = "11:00 AM",
            Status = "Approved",
            Disease = "Cold",
            Progress = "Recovering",
            Prescription = "Paracetamol",
            BillStatus = "Paid",
            AppointmentDate = date,
            FeedbackStatus = 1
        };

        // Assert
        vm.AppointmentId.Should().Be(100);
        vm.DoctorId.Should().Be(5);
        vm.PatientId.Should().Be(20);
        vm.DoctorName.Should().Be("Dr. Adams");
        vm.PatientName.Should().Be("Eve");
        vm.Timings.Should().Be("11:00 AM");
        vm.Status.Should().Be("Approved");
        vm.Disease.Should().Be("Cold");
        vm.BillStatus.Should().Be("Paid");
        vm.AppointmentDate.Should().Be(date);
        vm.FeedbackStatus.Should().Be(1);
    }

    // ─── StaffViewModel (shared) ──────────────────────────────────────────────

    [Fact]
    public void StaffViewModel_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var vm = new StaffViewModel();

        // Assert
        vm.StaffId.Should().Be(0);
        vm.Name.Should().Be(string.Empty);
        vm.Phone.Should().BeNull();
        vm.Address.Should().BeNull();
        vm.Gender.Should().Be(string.Empty);
        vm.Designation.Should().BeNull();
        vm.Salary.Should().Be(0);
        vm.Qualification.Should().BeNull();
    }

    [Fact]
    public void StaffViewModel_SetAllProperties_AllValuesCorrect()
    {
        // Arrange & Act
        var vm = new StaffViewModel
        {
            StaffId = 8,
            Name = "Carol Nurse",
            Phone = "5554445555",
            Address = "300 Cedar Ln",
            Gender = "F",
            Designation = "Head Nurse",
            Salary = 55000,
            Qualification = "BSc Nursing"
        };

        // Assert
        vm.StaffId.Should().Be(8);
        vm.Name.Should().Be("Carol Nurse");
        vm.Designation.Should().Be("Head Nurse");
        vm.Salary.Should().Be(55000);
        vm.Qualification.Should().Be("BSc Nursing");
    }
}
