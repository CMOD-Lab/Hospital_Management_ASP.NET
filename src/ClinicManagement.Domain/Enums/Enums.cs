namespace ClinicManagement.Domain.Enums;

/// <summary>User type enumeration</summary>
public enum UserType
{
    Patient = 1,
    Doctor = 2,
    Admin = 3
}

/// <summary>Gender enumeration</summary>
public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

/// <summary>Appointment status enumeration</summary>
public enum AppointmentStatus
{
    Pending = 0,
    Approved = 1,
    Completed = 2,
    Cancelled = 3
}
