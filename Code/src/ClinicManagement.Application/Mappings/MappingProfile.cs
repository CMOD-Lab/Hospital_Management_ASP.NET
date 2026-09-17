using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Mappings;

/// <summary>AutoMapper profile for mapping between domain entities and DTOs.</summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientDto>().ReverseMap();

        // Doctor mappings
        CreateMap<Doctor, DoctorDto>().ReverseMap();
        CreateMap<AddDoctorDto, Doctor>();

        // Staff mappings
        CreateMap<Staff, StaffDto>().ReverseMap();
        CreateMap<AddStaffDto, Staff>();

        // Appointment mappings
        CreateMap<Appointment, AppointmentDto>().ReverseMap();

        // Bill mappings
        CreateMap<Bill, BillDto>().ReverseMap();

        // TreatmentHistory mappings
        CreateMap<TreatmentHistory, TreatmentHistoryDto>().ReverseMap();

        // Department mappings
        CreateMap<Department, DepartmentDto>().ReverseMap();
    }
}
