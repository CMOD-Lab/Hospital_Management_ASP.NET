using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.Interfaces;

/// <summary>Authentication service interface</summary>
public interface IAuthService
{
    Task<(bool Success, int UserId, UserType UserType, string Message)> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Success, int PatientId, string Message)> RegisterPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken cancellationToken = default);
}

/// <summary>Patient service interface</summary>
public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<BillHistoryDto> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<TreatmentHistoryDto> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}

/// <summary>Doctor service interface</summary>
public interface IDoctorService
{
    Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(string deptName, CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeactivateDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillDto>> GenerateBillsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default);
}

/// <summary>Admin service interface</summary>
public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(StaffCreateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffDto>> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffDto>> SearchStaffAsync(string query, CancellationToken cancellationToken = default);
    Task<StaffDto?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>Appointment service interface</summary>
public interface IAppointmentService
{
    Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default);
}

/// <summary>Department service interface</summary>
public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
