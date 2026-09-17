namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a doctor in the clinic system.</summary>
public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public int Status { get; set; } = 1;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
