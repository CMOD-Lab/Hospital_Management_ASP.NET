using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Mappings;
using ClinicManagementSystem.Domain.Entities;
using Xunit;

namespace ClinicManagementSystem.Application.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        configuration.AssertConfigurationIsValid();
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Constructor_CreatesValidConfiguration()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_DoctorToDto_MapsDepartmentName()
    {
        var entity = new Doctor { Id = 1, Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Cardio", Address = "Addr", ChargesPerVisit = 10m, ExperienceYears = 4, DepartmentId = 2, Department = new Department { Name = "Cardiology" } };

        var dto = _mapper.Map<DoctorDto>(entity);

        Assert.Equal("Cardiology", dto.DepartmentName);
        Assert.Equal(entity.Name, dto.Name);
    }

    [Fact]
    public void Map_DoctorToDto_WithNullDepartment_MapsNullDepartmentName()
    {
        var entity = new Doctor { Name = "Doc", Email = "a@b.com", PhoneNumber = "1", Gender = "M", Qualification = "MBBS", Specialization = "Cardio", Address = "Addr" };

        var dto = _mapper.Map<DoctorDto>(entity);

        Assert.Null(dto.DepartmentName);
    }

    [Fact]
    public void Map_AppointmentToDto_MapsRelatedNames()
    {
        var entity = new Appointment { Id = 5, Name = "Visit", PatientId = 1, DoctorId = 2, ScheduledAt = DateTime.UtcNow, Status = "Done", Patient = new Patient { Name = "Patient A" }, Doctor = new Doctor { Name = "Doctor A" } };

        var dto = _mapper.Map<AppointmentDto>(entity);

        Assert.Equal("Patient A", dto.PatientName);
        Assert.Equal("Doctor A", dto.DoctorName);
    }

    [Fact]
    public void Map_CreateDtos_SetCreatedMetadata()
    {
        var doctor = _mapper.Map<Doctor>(new DoctorCreateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 12m, 3, 1));
        var patient = _mapper.Map<Patient>(new PatientCreateDto("Pat", "p@b.com", "2", DateTime.UtcNow, "F", "Addr"));
        var appointment = _mapper.Map<Appointment>(new AppointmentCreateDto("Visit", 1, 1, DateTime.UtcNow, "Pending", null, null, null));
        var department = _mapper.Map<Department>(new DepartmentCreateDto("Dept", "Desc"));

        Assert.True(doctor.IsActive);
        Assert.True(patient.IsActive);
        Assert.True(appointment.IsActive);
        Assert.True(department.IsActive);
        Assert.InRange(doctor.CreatedDate, DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public void Map_UpdateDtos_SetModifiedDate()
    {
        var doctor = _mapper.Map<Doctor>(new DoctorUpdateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 12m, 3, 1));
        var patient = _mapper.Map<Patient>(new PatientUpdateDto("Pat", "p@b.com", "2", DateTime.UtcNow, "F", "Addr"));
        var appointment = _mapper.Map<Appointment>(new AppointmentUpdateDto("Visit", 1, 1, DateTime.UtcNow, "Pending", null, null, null));
        var department = _mapper.Map<Department>(new DepartmentUpdateDto("Dept", "Desc"));

        Assert.NotNull(doctor.ModifiedDate);
        Assert.NotNull(patient.ModifiedDate);
        Assert.NotNull(appointment.ModifiedDate);
        Assert.NotNull(department.ModifiedDate);
    }
}
