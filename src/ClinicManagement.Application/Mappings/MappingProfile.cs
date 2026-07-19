using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mappings;

/// <summary>AutoMapper profile for entity-to-DTO mappings</summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientDto>()
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate.ToString("yyyy-MM-dd")));

        // Doctor mappings
        CreateMap<Doctor, DoctorDto>()
            .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.DeptName : string.Empty));

        CreateMap<DoctorCreateDto, Doctor>()
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => DateTime.Parse(s.BirthDate)))
            .ForMember(d => d.Status, o => o.MapFrom(_ => true))
            .ForMember(d => d.CreatedDate, o => o.MapFrom(_ => DateTime.UtcNow));

        // Appointment mappings
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Name : string.Empty))
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : string.Empty))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Bill mappings
        CreateMap<Bill, BillDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : string.Empty));

        // Staff mappings
        CreateMap<OtherStaff, StaffDto>();
        CreateMap<StaffCreateDto, OtherStaff>()
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => DateTime.Parse(s.BirthDate)))
            .ForMember(d => d.IsActive, o => o.MapFrom(_ => true))
            .ForMember(d => d.CreatedDate, o => o.MapFrom(_ => DateTime.UtcNow));

        // Department mappings
        CreateMap<Department, DepartmentDto>();

        // Treatment record mappings
        CreateMap<Appointment, TreatmentRecordDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Name : string.Empty))
            .ForMember(d => d.Disease, o => o.MapFrom(s => s.Disease ?? string.Empty))
            .ForMember(d => d.Prescription, o => o.MapFrom(s => s.Prescription ?? string.Empty))
            .ForMember(d => d.Progress, o => o.MapFrom(s => s.Progress ?? string.Empty));

        // Patient history mappings (doctor view)
        CreateMap<Appointment, PatientHistoryDto>()
            .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient != null ? s.Patient.Name : string.Empty))
            .ForMember(d => d.Disease, o => o.MapFrom(s => s.Disease ?? string.Empty))
            .ForMember(d => d.Prescription, o => o.MapFrom(s => s.Prescription ?? string.Empty))
            .ForMember(d => d.Progress, o => o.MapFrom(s => s.Progress ?? string.Empty));
    }
}
