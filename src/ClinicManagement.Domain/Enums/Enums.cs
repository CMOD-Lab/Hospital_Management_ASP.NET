namespace ClinicManagement.Domain.Enums;

/// <summary>User type in the system.</summary>
public enum UserType
{
    Patient = 1,
    Doctor = 2,
    Admin = 3
}

/// <summary>Doctor employment status.</summary>
public enum DoctorStatus
{
    Left = 0,
    Present = 1
}

/// <summary>Appointment status values.</summary>
public enum AppointmentStatus
{
    Approved = 1,
    Pending = 2,
    Completed = 3,
    Rejected = 4
}

/// <summary>Notification read status.</summary>
public enum NotificationStatus
{
    Seen = 1,
    Unseen = 2
}

/// <summary>Feedback status for an appointment.</summary>
public enum FeedbackStatus
{
    Given = 1,
    Pending = 2
}
