using ClinicManagementSystem.Application.DTOs;
using Xunit;

namespace ClinicManagementSystem.Application.DTOs;

public class DtoRecordTests
{
    [Fact]
    public void DoctorDtos_CanBeConstructed()
    {
        var dto = new DoctorDto(1, "Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 10m, 5, 2, "Dept");
        var create = new DoctorCreateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 10m, 5, 2);
        var update = new DoctorUpdateDto("Doc", "a@b.com", "1", "M", "MBBS", "Spec", "Addr", 10m, 5, 2);
        Assert.Equal("Doc", dto.Name);
        Assert.Equal(create.Name, update.Name);
    }

    [Fact]
    public void PatientDtos_CanBeConstructed()
    {
        var birthDate = DateTime.UtcNow.Date;
        var dto = new PatientDto(1, "Pat", "p@b.com", "1", birthDate, "F", "Addr");
        var create = new PatientCreateDto("Pat", "p@b.com", "1", birthDate, "F", "Addr");
        var update = new PatientUpdateDto("Pat", "p@b.com", "1", birthDate, "F", "Addr");
        Assert.Equal(birthDate, dto.BirthDate);
        Assert.Equal(create.Address, update.Address);
    }

    [Fact]
    public void DepartmentDtos_CanBeConstructed()
    {
        var dto = new DepartmentDto(1, "Dept", "Desc");
        var create = new DepartmentCreateDto("Dept", "Desc");
        var update = new DepartmentUpdateDto("Dept", "Desc");
        Assert.Equal(dto.Name, create.Name);
        Assert.Equal(create.Description, update.Description);
    }

    [Fact]
    public void AppointmentDtos_CanBeConstructed()
    {
        var scheduled = DateTime.UtcNow;
        var dto = new AppointmentDto(1, "Visit", 2, "Pat", 3, "Doc", scheduled, "Pending", "Rx", "Note", "Cold");
        var create = new AppointmentCreateDto("Visit", 2, 3, scheduled, "Pending", "Rx", "Note", "Cold");
        var update = new AppointmentUpdateDto("Visit", 2, 3, scheduled, "Pending", "Rx", "Note", "Cold");
        Assert.Equal(dto.Status, create.Status);
        Assert.Equal(create.Disease, update.Disease);
    }
}
