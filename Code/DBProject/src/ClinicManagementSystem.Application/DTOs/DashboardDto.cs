namespace ClinicManagementSystem.Application.DTOs;

public class DashboardDto
{
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
    public int StaffCount { get; set; }
    public int AppointmentCount { get; set; }
    public decimal Revenue { get; set; }
}
