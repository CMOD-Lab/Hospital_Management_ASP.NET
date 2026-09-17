namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for doctor data display.</summary>
public class DoctorDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public float ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Age { get; set; }
}

/// <summary>DTO for adding a new doctor.</summary>
public class AddDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

/// <summary>DTO for staff data display.</summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Salary { get; set; }
}

/// <summary>DTO for adding a new staff member.</summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>DTO for department data.</summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>DTO for admin dashboard statistics.</summary>
public class AdminDashboardDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal TotalIncome { get; set; }
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>DTO for updating prescription.</summary>
public class UpdatePrescriptionDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}
