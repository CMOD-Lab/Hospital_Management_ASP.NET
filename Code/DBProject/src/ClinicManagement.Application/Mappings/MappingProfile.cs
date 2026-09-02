using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mappings;

/// <summary>
/// Manual mapping helpers between domain entities and DTOs.
/// </summary>
public static class MappingProfile
{
    public static PatientDto ToDto(Patient patient)
    {
        return new PatientDto
        {
            PatientId = patient.PatientId,
            Name = patient.Name,
            Phone = patient.Phone,
            Address = patient.Address,
            BirthDate = patient.BirthDate,
            Gender = patient.Gender.ToString()
        };
    }

    public static DoctorDto ToDto(Doctor doctor)
    {
        return new DoctorDto
        {
            DoctorId = doctor.DoctorId,
            Name = doctor.Name,
            Phone = doctor.Phone,
            Address = doctor.Address,
            BirthDate = doctor.BirthDate,
            Gender = doctor.Gender.ToString(),
            DepartmentName = doctor.Department?.DeptName ?? string.Empty,
            DeptNo = doctor.DeptNo,
            ChargesPerVisit = doctor.ChargesPerVisit,
            WorkExperience = doctor.WorkExperience,
            Salary = doctor.Salary,
            Qualification = doctor.Qualification,
            Specialization = doctor.Specialization,
            ReputeIndex = doctor.ReputeIndex,
            PatientsTreated = doctor.PatientsTreated,
            Status = doctor.Status
        };
    }

    public static AppointmentDto ToDto(Appointment appointment)
    {
        return new AppointmentDto
        {
            AppointmentId = appointment.AppointmentId,
            DoctorId = appointment.DoctorId,
            PatientId = appointment.PatientId,
            DoctorName = appointment.Doctor?.Name,
            PatientName = appointment.Patient?.Name,
            Timings = appointment.Timings,
            Status = appointment.Status,
            Disease = appointment.Disease,
            Progress = appointment.Progress,
            Prescription = appointment.Prescription,
            BillStatus = appointment.BillStatus,
            AppointmentDate = appointment.AppointmentDate,
            FeedbackStatus = appointment.FeedbackStatus
        };
    }

    public static DepartmentDto ToDto(Department department)
    {
        return new DepartmentDto
        {
            DeptNo = department.DeptNo,
            DeptName = department.DeptName,
            Description = department.Description
        };
    }

    public static StaffDto ToDto(OtherStaff staff)
    {
        return new StaffDto
        {
            StaffId = staff.StaffId,
            Name = staff.Name,
            Phone = staff.Phone,
            Address = staff.Address,
            BirthDate = staff.BirthDate,
            Gender = staff.Gender.ToString(),
            Designation = staff.Designation,
            Salary = staff.Salary,
            Qualification = staff.Qualification
        };
    }
}
